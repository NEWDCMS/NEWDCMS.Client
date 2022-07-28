using Acr.UserDialogs;
using DCMS.Client.Enums;
using DCMS.Client.Models;
using DCMS.Client.Models.Products;
using DCMS.Client.Models.WareHouses;
using DCMS.Client.Pages;
using DCMS.Client.Services;
using DCMS.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using System.Reactive.Disposables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Diagnostics;

namespace DCMS.Client.ViewModels
{
    public class BackStockBillPageViewModel : ViewModelBaseCutom<AllocationBillModel>
    {
        [Reactive] public AllocationItemModel Selecter { get; set; }
        public BackStockBillPageViewModel(INavigationService navigationService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
              IDialogService dialogService
            ) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "回库调拨单";

            InitBill();

            //验证
            var valid_IsReversed = this.ValidationRule(x => x.Bill.ReversedStatus, _isBool, "已红冲单据不能操作");
            var valid_IsAudited = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");
            var valid_ShipmentWareHouseId = this.ValidationRule(x => x.Bill.ShipmentWareHouseId, _isZero, "出货仓库未指定");
            var valid_IncomeWareHouseId = this.ValidationRule(x => x.Bill.IncomeWareHouseId, _isZero, "入货仓库未指定");
            var valid_ProductCount = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "请添加商品项目");

            //添加商品
            this.AddProductCommand = ReactiveCommand.Create<object>(async e =>
           {
               if (Bill.ShipmentWareHouseId == 0)
               {
                   _dialogService.ShortAlert(valid_ShipmentWareHouseId.Message[0]);
                   return;
               }

               if (Bill.IncomeWareHouseId == 0)
               {
                   _dialogService.ShortAlert(valid_IncomeWareHouseId.Message[0]);
                   return;
               }

               if (Bill.ShipmentWareHouseId == Bill.IncomeWareHouseId)
               {
                   _dialogService.ShortAlert("出入库不能相同！");
                   return;
               }

               //传入当前主库
               this.WareHouse.Id = Bill.ShipmentWareHouseId;
               this.WareHouse.Name = Bill.ShipmentWareHouseName;
               this.Filter.WareHouseId = Bill.ShipmentWareHouseId;
               this.Filter.WareHouseName = Bill.ShipmentWareHouseName;

               await this.NavigateAsync("SelectProductPage", ("Reference", this.PageName),
                       ("Bill", Bill),
                       ("Filter", Filter),
                       ("WareHouse", WareHouse),
                       ("SerchKey", Filter.SerchKey));
           });

            //编辑商品
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
                .Skip(1)
                .Where(x => x != null)
                .SubOnMainThread(async item =>
               {
                   if (item != null)
                   {
                       var product = ProductSeries.Select(p => p).Where(p => p.Id == item.ProductId).FirstOrDefault();
                       if (product != null)
                       {
                           product.UnitId = item.UnitId;
                           product.Quantity = item.Quantity;
                           product.Remark = item.Remark;
                           product.Subtotal = item.Subtotal;
                           product.UnitName = item.UnitName;

                           if (item.BigUnitId > 0)
                           {
                               product.bigOption.Name = item.UnitName;
                               product.BigPriceUnit.Quantity = item.Quantity;
                               product.BigPriceUnit.UnitId = item.BigUnitId ?? 0;
                               product.BigPriceUnit.Remark = item.Remark;
                           }

                           if (item.SmallUnitId > 0)
                           {
                               product.bigOption.Name = item.UnitName;
                               product.SmallPriceUnit.Quantity = item.Quantity;
                               product.SmallPriceUnit.UnitId = item.SmallUnitId;
                               product.SmallPriceUnit.Remark = item.Remark;
                           }

                           await this.NavigateAsync("EditAllocationProductPage", ("Product", product));
                       }
                   }

                   this.Selecter = null;
               }, ex => _dialogService.ShortAlert(ex.ToString()))
                .DisposeWith(DeactivateWith);

            //提交单据
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                //await this.Access(AccessGranularityEnum.AllocationFormSave);
                return await this.Access(AccessGranularityEnum.AllocationFormSave, async () =>
                {
                    if (!valid_ShipmentWareHouseId.IsValid) { this.Alert(valid_ShipmentWareHouseId.Message[0]); return Unit.Default; }
                    if (!valid_IncomeWareHouseId.IsValid) { this.Alert(valid_IncomeWareHouseId.Message[0]); return Unit.Default; }
                    if (!valid_ProductCount.IsValid) { this.Alert(valid_ProductCount.Message[0]); return Unit.Default; }

                    var postMData = new AllocationUpdateModel()
                    {
                        BillNumber = this.Bill.BillNumber,
                        ShipmentWareHouseId = Bill.ShipmentWareHouseId,
                        IncomeWareHouseId = Bill.IncomeWareHouseId,
                        CreatedOnUtc = Bill.CreatedOnUtc,
                        AllocationByMinUnit = false,
                        Remark = Bill.Remark,
                        //商品项目
                        Items = Bill.Items?.Where(i => i.Quantity > 0).ToList(),
                    };

                    return await SubmitAsync(postMData, Bill.Id, _wareHousesService.CreateOrUpdateAllocationbillAsync, (result) =>
                    {
                        Bill = new AllocationBillModel();
                    }, token: new System.Threading.CancellationToken());
                });
            },
            this.IsValid());

            //存储记录
            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                if (this.Bill.ShipmentWareHouseId != 0 && this.Bill.ShipmentWareHouseId != Settings.BackStockBill?.ShipmentWareHouseId)
                {
                    var ok = await _dialogService.ShowConfirmAsync("你是否要保存单据？", "提示", "确定", "取消");
                    if (ok)
                    {
                        if (!this.Bill.AuditedStatus)
                            Settings.BackStockBill = this.Bill;
                    }
                }
                await _navigationService.GoBackAsync();
            });


            //绑定页面菜单
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                //整单备注
                { MenuEnum.REMARK,(m,vm)=>{

                     AllRemak((result) =>
                     {
                             Bill.Remark = result;
                     }, Bill.Remark);

                } },
                //清空单据
                { MenuEnum.CLEAR,(m,vm)=>{

                    ClearBill<AllocationBillModel, AllocationItemModel>(Bill, DoClear);
                } },
                //AXBH
                { MenuEnum.AXBH,async (m,vm)=>{

                    if (Bill.IncomeWareHouseId == 0)
                    {
                        _dialogService.ShortAlert("入仓为必选");
                        return;
                    }
                    await this.NavigateAsync("SelectAllocationProductPage", ("WareHouseId", Bill.IncomeWareHouseId));
                } },
                 //ATDB
                { MenuEnum.ATDB,async (m,vm)=>{

                    if (Bill.ShipmentWareHouseId == 0)
                    {
                        _dialogService.ShortAlert("出仓为必选");
                        return;
                    }
                    await this.NavigateAsync("SelectAllocationProductPage", ("WareHouseId", Bill.ShipmentWareHouseId));
                } },
                //AKCDB
                { MenuEnum.AKCDB,async (m,vm)=>{
                    if (Bill.ShipmentWareHouseId == 0)
                    {
                               _dialogService.ShortAlert("出仓为必选");
                               return;
                    }
                    await this.NavigateAsync("SelectAllocationProductPage", ("WareHouseId", Bill.ShipmentWareHouseId));
                } },
                 //打印
                { MenuEnum.PRINT,async (m,vm)=>{

                     this.Bill.BillType = BillTypeEnum.AllocationBill;
                     await SelectPrint(Bill);
                } },
                //历史单据
                { MenuEnum.HISTORY,async (m,vm)=>{
                        await SelectHistory();
                } },
                //审核
                { MenuEnum.SHENGHE,async (m,vm)=>{

                    ResultData result = null;
                         using (UserDialogs.Instance.Loading("审核中..."))
                         {
                             result = await _wareHousesService.AuditingAsync(Bill.Id);
                         }
                         if (result!=null&&result.Success)
                         {
                            //红冲审核水印
                            this.EnableOperation = true;
                            this.Bill.AuditedStatus = true;
                            await ShowConfirm(true, "审核成功", true, goReceipt: false);
                         }
                         else
                         {
                             await ShowConfirm(false, $"审核失败！{result?.Message}", false, goReceipt: false);
                         }

                } }
            });
        }

        private void InitBill(bool clear = false)
        {
            this.BillType = BillTypeEnum.BackStockBill;
            var bill = Settings.BackStockBill;
            if (bill != null && !clear)
            {
                this.Bill = bill;
                this.Bill.Items = new ObservableCollection<AllocationItemModel>(bill?.Items);
                this.Selecter = bill?.Items?.FirstOrDefault();
            }
            else
            {
                this.Bill = new AllocationBillModel()
                {
                    BillTypeId = (int)this.BillType,
                    MakeUserId = Settings.UserId,
                    MakeUserName = Settings.UserRealName,
                    CreatedOnUtc = DateTime.Now,
                    BillNumber = CommonHelper.GetBillNumber(CommonHelper.GetEnumDescription(BillType).Split(',')[1], Settings.StoreId)
                };
            }
        }

        private void DoClear()
        {
            InitBill(true);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                if (parameters.ContainsKey("DelProduct"))//删除商品
                {
                    parameters.TryGetValue<ProductModel>("DelProduct", out ProductModel p);
                    if (p != null)
                    {
                        var bigProduct = Bill.Items?.Where(b => b.GUID == p.GUID && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items?.Where(b => b.GUID == p.GUID && b.SmallUnitId == p.UnitId).FirstOrDefault();
                        if (bigProduct != null)
                        {
                            Bill.Items?.Remove(bigProduct);
                        }

                        if (smalProduct != null)
                        {
                            Bill.Items?.Remove(smalProduct);
                        }
                    }
                }

                if (parameters.ContainsKey("UpdateProduct"))//编辑商品更新
                {
                    parameters.TryGetValue<ProductModel>("UpdateProduct", out ProductModel p);
                    if (p != null)
                    {
                        var bigProduct = Bill.Items?.Where(b => b.GUID == p.GUID && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items?.Where(b => b.GUID == p.GUID && b.SmallUnitId == p.UnitId).FirstOrDefault();
                        if (bigProduct != null)
                        {
                            bigProduct.UnitName = p.UnitName;
                            bigProduct.Quantity = p.Quantity;
                            bigProduct.Remark = p.Remark;
                            bigProduct.BigUnitId = p.UnitId;
                            bigProduct.Subtotal = p.Quantity * p.BigQuantity ?? 0;
                        }
                        if (smalProduct != null)
                        {
                            smalProduct.UnitName = p.UnitName;
                            smalProduct.Quantity = p.Quantity;
                            smalProduct.Remark = p.Remark;
                            smalProduct.BigUnitId = p.UnitId;
                            smalProduct.Subtotal = p.Quantity;
                        }

                        if (Bill.Items?.Any() ?? false)
                        {
                            foreach (var ip in Bill.Items)
                            {
                                if (smalProduct != null && (ip.UnitId == smalProduct.SmallUnitId || ip.SmallUnitId == smalProduct.SmallUnitId))
                                {
                                    ip.UnitName = smalProduct.UnitName;
                                    ip.Quantity = smalProduct.Quantity;
                                    ip.Remark = smalProduct.Remark;
                                    ip.SmallUnitId = smalProduct.SmallUnitId;
                                    ip.Subtotal = smalProduct.Subtotal;
                                }
                                else if (bigProduct != null && (ip.UnitId == bigProduct.SmallUnitId || ip.BigUnitId == bigProduct.BigUnitId))
                                {
                                    ip.UnitName = bigProduct.UnitName;
                                    ip.Quantity = bigProduct.Quantity;
                                    ip.Remark = bigProduct.Remark;
                                    ip.BigUnitId = bigProduct.UnitId;
                                    ip.Subtotal = bigProduct.Subtotal;
                                }
                            }
                        }
                    }
                }

                if (parameters.ContainsKey("ProductSeries"))//选择商品序列
                {
                    parameters.TryGetValue("ProductSeries", out ObservableCollection<ProductModel> productSeries);
                    this.ProductSeries = productSeries;
                    if (productSeries != null && productSeries.Count > 0)
                    {
                        foreach (var p in productSeries)
                        {
                            var bigItem = new AllocationItemModel()
                            {
                                Id = p.Id,
                                GUID = p.GUID,
                                UnitId = p.BigPriceUnit.UnitId,
                                ProductId = p.Id,
                                ProductName = p.ProductName,
                                UnitName = p.bigOption.Name,
                                TradePrice = p.BigPriceUnit.Price,
                                Quantity = p.BigPriceUnit.Quantity,
                                Remark = p.BigPriceUnit.Remark,
                                BigUnitId = p.BigPriceUnit.UnitId,
                                SmallUnitId = 0,
                                Subtotal = p.BigPriceUnit.Quantity * p.BigQuantity ?? 0
                            };

                            var smallItem = new AllocationItemModel()
                            {
                                Id = p.Id,
                                GUID = p.GUID,
                                UnitId = p.SmallPriceUnit.UnitId,
                                ProductId = p.Id,
                                ProductName = p.ProductName,
                                UnitName = p.smallOption.Name,
                                TradePrice = p.SmallPriceUnit.Price,
                                Quantity = p.SmallPriceUnit.Quantity,
                                Remark = p.SmallPriceUnit.Remark,
                                SmallUnitId = p.SmallPriceUnit.UnitId,
                                BigUnitId = 0,
                                Subtotal = p.SmallPriceUnit.Quantity
                            };

                            Bill.Items?.Add(bigItem);
                            Bill.Items?.Add(smallItem);
                        }
                        ProductSeries = new ObservableCollection<ProductModel>(productSeries);
                    }
                }
                if (parameters.ContainsKey("Bill"))
                {
                    //var bill = parameters.GetValue<AllocationBillModel>("Bill");
                    parameters.TryGetValue<AllocationBillModel>("Bill", out AllocationBillModel bill);
                    if (bill != null)
                    {
                        Bill = bill;
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        public override void OnAppearing()
        {
            base.OnAppearing();

            //控制显示菜单
            _popupMenu?.Show(3, 4, 22, 23, 24);
        }
    }
}
