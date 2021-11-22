using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Finances;
using Wesley.Client.Models.Products;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Pages;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
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

namespace Wesley.Client.ViewModels
{
    public class CostContractBillPageViewModel : ViewModelBaseCutom<CostContractBillModel>
    {
        private readonly ICostContractService _costContractService;

        public ReactiveCommand<object, Unit> AccountingSelected { get; }
        [Reactive] public CostContractItemModel Selecter { get; set; }

        public CostContractBillPageViewModel(INavigationService navigationService,
                  IProductService productService,
                  IUserService userService,
                  ITerminalService terminalService,
                  IWareHousesService wareHousesService,
                  IAccountingService accountingService,
                  ICostContractService costContractService,
                  IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "费用合同";

            _costContractService = costContractService;
            this.BillType = BillTypeEnum.CostContractBill;

            InitBill();

            //验证
            var valid_IsReversed = this.ValidationRule(x => x.Bill.ReversedStatus, _isBool, "已红冲单据不能操作");
            var valid_IsAudited = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");
            var valid_TerminalId = this.ValidationRule(x => x.Bill.CustomerId, _isZero, "客户未指定");
            var valid_AccountingOptionId = this.ValidationRule(x => x.Bill.AccountingOptionId, _isZero, "费用类型未指定");
            var valid_ItemCount = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "合同项目为空，请先添加");


            //编辑
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
             .Skip(1)
             .Where(x => x != null)
             .SubOnMainThread(async x =>
            {
                if (this.Bill.ReversedStatus)
                {
                    _dialogService.ShortAlert("已红冲单据不能操作");
                    return;
                }

                if (this.Bill.AuditedStatus)
                {
                    _dialogService.ShortAlert("已审核单据不能操作");
                    return;
                }

                if (x != null)
                {
                    var product = ProductSeries.Select(p => p).Where(p => p.ProductId == x.ProductId).FirstOrDefault();
                    if (product != null)
                    {
                        product.UnitId = x.UnitId ?? 0;
                        product.Quantity = x.TotalQuantity;
                        product.Remark = x.Remark;
                        product.UnitName = x.UnitName;

                        if (x.BigUnitId > 0)
                        {
                            product.Id = x.Id;
                            product.bigOption.Name = x.UnitName;
                            product.BigPriceUnit.Quantity = x.TotalQuantity;
                            product.BigPriceUnit.Remark = x.Remark;
                            product.BigPriceUnit.UnitId = x.BigUnitId ?? 0;
                        }

                        if (x.SmallUnitId > 0)
                        {
                            product.Id = x.Id;
                            product.bigOption.Name = x.UnitName;
                            product.SmallPriceUnit.Quantity = x.TotalQuantity;
                            product.SmallPriceUnit.Remark = x.Remark;
                            product.SmallPriceUnit.UnitId = x.SmallUnitId ?? 0;
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
                return await this.Access(AccessGranularityEnum.CostContractSave, async () =>
                {
                    if (this.Bill.ReversedStatus)
                    {
                        _dialogService.ShortAlert("已红冲单据不能操作");
                        return Unit.Default;
                    }

                    if (this.Bill.AuditedStatus)
                    {
                        _dialogService.ShortAlert("已审核单据不能操作");
                        return Unit.Default;
                    }

                    if (Bill.CustomerId == 0)
                        Bill.CustomerId = Settings.UserId;

                    var postData = new CostContractUpdateModel()
                    {
                        BillNumber = this.Bill.BillNumber,
                        EmployeeId = Settings.UserId,
                        CustomerId = Bill.CustomerId,
                        AccountingOptionId = Bill.AccountingOptionId,
                        Year = DateTime.Now.Year,
                        Month = DateTime.Now.Month,
                        ContractType = 1, //按单位量总计兑付
                        SaleRemark = Bill.SaleRemark,
                        Remark = Bill.Remark,
                        Items = Bill.Items
                    };

                    return await SubmitAsync(postData, Bill.Id, _costContractService.CreateOrUpdateAsync, (result) =>
                    {
                        Bill = new CostContractBillModel();
                    }, token: new System.Threading.CancellationToken());
                });
            },
            this.IsValid());

            //存储记录
            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                if (!this.Bill.AuditedStatus && this.Bill.CustomerId != 0 && this.Bill.CustomerId != Settings.CostContractBill?.CustomerId)
                {
                    var ok = await _dialogService.ShowConfirmAsync("你是否要保存单据？", "提示", "确定", "取消");
                    if (ok)
                    {
                        if (!this.Bill.AuditedStatus)
                            Settings.CostContractBill = this.Bill;
                    }
                }
                await _navigationService.GoBackAsync();
            });

            //费用选择
            this.AccountingSelected = ReactiveCommand.Create<object>(async e =>
            {
                if (!valid_TerminalId.IsValid) { this.Alert(valid_TerminalId.Message[0]); return; }
                await SelectCostAccounting((data) =>
                 {
                     Bill.AccountingOptionId = data.AccountingOptionId;
                     Bill.AccountingOptionName = data.Name;
                 }, BillTypeEnum.CostContractBill);
            });

            //添加商品
            this.AddProductCommand = ReactiveCommand.Create<object>(async e =>
           {
               if (!valid_TerminalId.IsValid) { this.Alert(valid_TerminalId.Message[0]); return; }
               if (!valid_AccountingOptionId.IsValid) { this.Alert(valid_AccountingOptionId.Message[0]); return; }
               if (!valid_IsAudited.IsValid) { this.Alert(valid_IsAudited.Message[0]); return; }
               await this.NavigateAsync("SelectProductPage", ("Reference", this.PageName), ("SerchKey", Filter.SerchKey));
           });

            //审核
            this.AuditingDataCommand = ReactiveCommand.Create<object>(async _ =>
            {
                //是否具有审核权限
                await this.Access(AccessGranularityEnum.CostContractApproved);
                await SubmitAsync(Bill.Id, _costContractService.AuditingAsync, async (result) =>
               {
                   //红冲审核水印
                   this.Bill.AuditedStatus = true;

                   var _conn = App.Resolve<ILiteDbService<MessageInfo>>();
                   var ms = await _conn.Table.FindByIdAsync(SelecterMessage.Id);
                   if (ms != null)
                   {
                       ms.IsRead = true;
                       await _conn.UpsertAsync(ms);
                   }
               });
            }, this.WhenAny(x => x.Bill.Id, (x) => x.GetValue() > 0));

            //绑定页面菜单
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                 //整单备注
                { MenuEnum.REMARK, (m,vm) => {
                    AllRemak((result) => { Bill.Remark = result; }, Bill.Remark);
                } }, 
                //清空单据
                { MenuEnum.CLEAR, (m,vm) => {
                    ClearBill<CostContractBillModel, CostContractItemModel>(Bill, DoClear);
                } },
                 //销售备注
                { MenuEnum.SALEREMARK, (m,vm) => {
                    AllRemak((result) => { Bill.SaleRemark = result; }, Bill.SaleRemark);
                } }
            });
        }

        private void InitBill(bool clear = false)
        {
            this.BillType = BillTypeEnum.CostContractBill;
            var bill = Settings.CostContractBill;
            if (bill != null && !clear)
            {
                this.Bill = bill;
                this.Bill.Items = new ObservableCollection<CostContractItemModel>(bill?.Items);
                this.Selecter = bill?.Items?.FirstOrDefault();
            }
            else
            {
                this.Bill = new CostContractBillModel()
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
                //删除商品
                if (parameters.ContainsKey("DelProduct"))
                {
                    parameters.TryGetValue("DelProduct", out ProductModel p);
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

                //编辑商品更新
                if (parameters.ContainsKey("UpdateProduct"))
                {
                    parameters.TryGetValue("UpdateProduct", out ProductModel p);
                    if (p != null)
                    {
                        var bigProduct = Bill.Items?.Where(b => b.GUID == p.GUID && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items?.Where(b => b.GUID == p.GUID && b.SmallUnitId == p.UnitId).FirstOrDefault();
                        if (bigProduct != null)
                        {
                            bigProduct.UnitName = p.UnitName;
                            bigProduct.BigUnitQuantity = p.Quantity;
                            bigProduct.Total = p.Quantity;
                            bigProduct.TotalQuantity = p.Quantity;
                            bigProduct.Remark = p.Remark;
                            bigProduct.BigUnitId = p.UnitId;
                        }
                        if (smalProduct != null)
                        {
                            smalProduct.UnitName = p.UnitName;
                            smalProduct.SmallUnitQuantity = p.Quantity;
                            smalProduct.Total = p.Quantity;
                            smalProduct.TotalQuantity = p.Quantity;
                            smalProduct.Remark = p.Remark;
                            smalProduct.BigUnitId = p.UnitId;
                        }

                        if (Bill.Items?.Any() ?? false)
                        {
                            foreach (var ip in Bill.Items)
                            {
                                if (smalProduct != null && (ip.UnitId == smalProduct.SmallUnitId || ip.SmallUnitId == smalProduct.SmallUnitId))
                                {
                                    ip.UnitName = smalProduct.UnitName;
                                    //ip.Quantity = smalProduct.Quantity;
                                    ip.Remark = smalProduct.Remark;
                                    ip.SmallUnitId = smalProduct.SmallUnitId;
                                    //ip.Subtotal = smalProduct.Subtotal;
                                }
                                else if (bigProduct != null && (ip.UnitId == bigProduct.SmallUnitId || ip.BigUnitId == bigProduct.BigUnitId))
                                {
                                    ip.UnitName = bigProduct.UnitName;
                                    //ip.Quantity = bigProduct.Quantity;
                                    ip.Remark = bigProduct.Remark;
                                    ip.BigUnitId = bigProduct.UnitId;
                                    //ip.Subtotal = bigProduct.Subtotal;
                                }
                            }
                        }

                        Bill.AllNum = Bill.Items?.Count ?? 0;
                        Bill.BigNum = Bill.Items?.Select(s => s.BigUnitQuantity).Sum() ?? 0;
                        Bill.SmallNum = Bill.Items?.Select(s => s.SmallUnitQuantity).Sum() ?? 0;



                    }
                }

                //选择客户
                if (parameters.ContainsKey("Terminaler"))
                {
                    parameters.TryGetValue("Terminaler", out TerminalModel terminaler);
                    Bill.CustomerId = terminaler != null ? terminaler.Id : 0;
                    Bill.CustomerName = terminaler != null ? terminaler.Name : "";
                }

                //选择商品序列
                if (parameters.ContainsKey("ProductSeries"))
                {
                    parameters.TryGetValue("ProductSeries", out List<ProductModel> productSeries);
                    this.ProductSeries = new ObservableCollection<ProductModel>(productSeries);
                    if (productSeries != null && productSeries.Count > 0)
                    {
                        foreach (var p in productSeries)
                        {
                            var bigItem = new CostContractItemModel()
                            {
                                GUID = p.GUID,
                                UnitId = p.BigPriceUnit.UnitId,
                                ProductId = p.ProductId,
                                ProductName = p.ProductName,
                                UnitName = p.bigOption.Name,
                                BigUnitQuantity = p.BigPriceUnit.Quantity,
                                TotalQuantity = p.BigPriceUnit.Quantity,
                                Total = p.BigPriceUnit.Quantity,
                                Remark = p.BigPriceUnit.Remark,
                                BigUnitId = p.BigPriceUnit.UnitId,
                            };

                            var smallItem = new CostContractItemModel()
                            {
                                GUID = p.GUID,
                                UnitId = p.SmallPriceUnit.UnitId,
                                ProductId = p.ProductId,
                                ProductName = p.ProductName,
                                UnitName = p.smallOption.Name,
                                SmallUnitQuantity = p.SmallPriceUnit.Quantity,
                                TotalQuantity = p.SmallPriceUnit.Quantity,
                                Total = p.SmallPriceUnit.Quantity,
                                Remark = p.SmallPriceUnit.Remark,
                                SmallUnitId = p.SmallPriceUnit.UnitId,
                            };

                            if (bigItem.TotalQuantity > 0 && Bill.Items?.Where(s => s.ProductId == bigItem.ProductId && s.UnitId == bigItem.UnitId && s.TotalQuantity == bigItem.TotalQuantity).Count() == 0)
                                Bill.Items?.Add(bigItem);

                            if (smallItem.TotalQuantity > 0 && Bill.Items?.Where(s => s.ProductId == smallItem.ProductId && s.UnitId == smallItem.UnitId && s.TotalQuantity == smallItem.TotalQuantity).Count() == 0)
                                Bill.Items?.Add(smallItem);
                        }
                    }
                    Bill.AllNum = Bill.Items?.Count ?? 0;
                    Bill.BigNum = Bill.Items?.Select(s => s.BigUnitQuantity).Sum() ?? 0;
                    Bill.SmallNum = Bill.Items?.Select(s => s.SmallUnitQuantity).Sum() ?? 0;
                }

                //预览单据
                if (parameters.ContainsKey("Bill"))
                {
                    parameters.TryGetValue("Bill", out CostContractBillModel bill);
                    parameters.TryGetValue("IsSubmitBill", out bool isSubmitBill);
                    this.Bill.IsSubmitBill = isSubmitBill;
                    if (bill != null)
                    {
                        Bill = bill;
                        if (bill.AuditedStatus) { }
                        else
                        {
                            //控制显示菜单
                            _popupMenu?.Show(34);
                        }
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
            _popupMenu?.Show(3, 4, 20);
        }
    }
}
