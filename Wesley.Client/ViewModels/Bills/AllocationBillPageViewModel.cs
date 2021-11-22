using Acr.UserDialogs;
using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Products;
using Wesley.Client.Models.WareHouses;
using Wesley.Client.Pages;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class AllocationBillPageViewModel : ViewModelBaseCutom<AllocationBillModel>
    {
        private readonly IMicrophoneService _microphoneService;
        [Reactive] public AllocationItemModel Selecter { get; set; }
        public bool IsNeedShowPrompt { get; set; } = true;
        public bool IsNeedPrint { get; set; } = false;

        public AllocationBillPageViewModel(INavigationService navigationService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IDialogService dialogService,
            IMicrophoneService microphoneService
            ) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "调拨单";
            _microphoneService = microphoneService;

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
               try
               {
                   if (this.Bill.ReversedStatus)
                   {
                       _dialogService.ShortAlert("已红冲单据不能操作"); return;
                   }

                   if (this.Bill.AuditedStatus) { _dialogService.ShortAlert("已审核单据不能操作！"); return; }

                   if (!valid_ShipmentWareHouseId.IsValid)
                   {
                       _dialogService.ShortAlert(valid_ShipmentWareHouseId.Message[0]);
                       return;
                   }

                   if (!valid_IncomeWareHouseId.IsValid)
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

                   //ShipmentWareHouseId
                   //IncomeWareHouseId

                   await this.NavigateAsync("SelectProductPage", ("Reference", this.PageName),
                             ("Bill", Bill),
                             ("Filter", Filter),
                             ("WareHouse", WareHouse),
                             ("SerchKey", Filter.SerchKey));
               }
               catch (Exception ex)
               {

                   Crashes.TrackError(ex);
               }
           });

            //编辑商品
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
                .Skip(1)
                .Where(x => x != null)
                .SubOnMainThread(async item =>
               {
                   try
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

                       if (this.Bill.IsSubmitBill)
                       {
                           _dialogService.ShortAlert("已提交的单据不能编辑");
                           return;
                       }

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
                               product.GUID = item.GUID;
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
                   }
                   catch (Exception ex)
                   {
                       Crashes.TrackError(ex);
                   }

                   this.Selecter = null;
               }, ex => _dialogService.ShortAlert(ex.ToString()))
                .DisposeWith(DeactivateWith);

            //提交单据
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                return await this.Access(AccessGranularityEnum.AllocationFormSave, async () =>
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

                    if (!valid_ShipmentWareHouseId.IsValid) { this.Alert(valid_ShipmentWareHouseId.Message[0]); return Unit.Default; }
                    if (!valid_IncomeWareHouseId.IsValid) { this.Alert(valid_IncomeWareHouseId.Message[0]); return Unit.Default; }
                    if (!valid_ProductCount.IsValid) { this.Alert(valid_ProductCount.Message[0]); return Unit.Default; }

                    var zero = Bill.Items?.Where(s => s.Quantity == 0).Count() > 0;
                    if (zero)
                    {
                        this.Alert("存在数量为空的调拨商品！");
                        return Unit.Default;
                    }

                    var postMData = new AllocationUpdateModel()
                    {
                        BillNumber = Bill.BillNumber,
                        ShipmentWareHouseId = Bill.ShipmentWareHouseId,
                        IncomeWareHouseId = Bill.IncomeWareHouseId,
                        CreatedOnUtc = Bill.CreatedOnUtc,
                        AllocationByMinUnit = false,
                        Remark = Bill.Remark,
                        //商品项目
                        Items = Bill.Items?.ToList(),
                    };

                    ShowPrintBtn = true;
                    if (IsNeedShowPrompt)
                    {
                        var ok = await _dialogService.ShowConfirmAsync("是否保存并提交吗？", "", "确定", "取消");
                        if (!ok)
                        {
                            return Unit.Default;
                        }
                    }
                    IsNeedShowPrompt = true;

                    var gr = Bill.AuditedStatus;
                    return await SubmitAsync(postMData, Bill.Id, _wareHousesService.CreateOrUpdateAllocationbillAsync, async (result) =>
                     {
                         if (IsNeedPrint)
                         {
                             Bill.BillType = BillTypeEnum.AllocationBill;
                             await SelectPrint((AllocationBillModel)Bill.Clone());
                         }
                         IsNeedPrint = false;
                         Bill = new AllocationBillModel();
                         InitBill();

                     }, gr, token: new System.Threading.CancellationToken());
                });
            },
            this.IsValid());

            //存储记录
            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                if (!this.Bill.AuditedStatus && this.Bill.ShipmentWareHouseId != 0 && this.Bill.ShipmentWareHouseId != Settings.AllocationBill?.ShipmentWareHouseId)
                {
                    var ok = await _dialogService.ShowConfirmAsync("你是否要保存单据？", "提示", "确定", "取消");
                    if (ok)
                    {
                        if (!this.Bill.AuditedStatus)
                            Settings.AllocationBill = this.Bill;
                    }
                }
                await _navigationService.GoBackAsync();
            });

            //审核
            this.AuditingDataCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                //是否具有审核权限
                await this.Access(AccessGranularityEnum.AllocationFormApproved);
                await SubmitAsync(Bill.Id, _wareHousesService.AuditingAsync, async (result) =>
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

                }, token: new System.Threading.CancellationToken());
            }, this.WhenAny(x => x.Bill.Id, (x) => x.GetValue() > 0));


            //启用麦克风
            this.WhenAnyValue(x => x.EnableMicrophone)
              .Subscribe(async x =>
              {
                  var micAccessGranted = await _microphoneService.GetPermissionsAsync();
                  if (!micAccessGranted)
                  {
                      this.Alert("请打开麦克风");
                  }
              })
              .DisposeWith(this.DeactivateWith);
            //匹配声音
            this.RecognitionCommand = ReactiveCommand.Create(() =>
            {
                RecognitionSpeech((key) =>
                {
                    Filter.SerchKey = key;
                    ((ICommand)this.AddProductCommand)?.Execute(null);
                });
            });
            //切换语音助手
            this.SpeechCommand = ReactiveCommand.Create(() =>
            {
                if (IsFooterVisible)
                {
                    IsVisible = true;
                    IsExpanded = true;
                    IsFooterVisible = false;
                }
                else
                {
                    IsVisible = false;
                    IsExpanded = false;
                    IsFooterVisible = true;
                }
            });


            //工具栏打印
            this.PrintCommand = ReactiveCommand.Create(async () =>
            {
                await Print();
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
                    await Print();
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


        private async Task Print()
        {
            if (Bill.Items.Count == 0)
            {
                Alert("请添加商品项目");
                return;
            }
            if (Bill.Id <= 0)
            {
                var ok = await _dialogService.ShowConfirmAsync("请您确认打印并提交？", "", "确定", "取消");
                if (ok)
                {
                    IsNeedPrint = true;
                    IsNeedShowPrompt = false;
                    ((ICommand)SubmitDataCommand)?.Execute(null);
                }
                else
                {
                    return;
                }
            }
            else
            {
                Bill.BillType = BillTypeEnum.AllocationBill;
                await SelectPrint(Bill);
            }
        }

        private void InitBill(bool clear = false)
        {
            this.BillType = BillTypeEnum.AllocationBill;
            var bill = Settings.AllocationBill;
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
                    BusinessUserId = Settings.UserId,
                    BusinessUserName = Settings.UserRealName,
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
                    if (Bill != null && p != null)
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

                    if (Bill != null && p != null)
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
                            smalProduct.SmallUnitId = p.UnitId;
                            smalProduct.Subtotal = p.Quantity;
                        }

                    }
                }

                //选择商品序列
                if (parameters.ContainsKey("ProductSeries"))
                {
                    parameters.TryGetValue("ProductSeries", out ObservableCollection<ProductModel> productSeries);
                    if (productSeries != null && productSeries.Count > 0)
                    {
                        this.ProductSeries = productSeries;
                        if (Bill != null && productSeries != null)
                        {
                            foreach (var p in productSeries)
                            {
                                var bigItem = new AllocationItemModel()
                                {
                                    Id = 0,//新加项目
                                    //GUID = p.GUID,
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
                                    Id = 0,//新加项目
                                    //GUID = p.GUID,
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

                                if (bigItem.Quantity > 0)
                                    BillAddItem(bigItem);


                                if (smallItem.Quantity > 0)
                                    BillAddItem(smallItem);
                            }
                        }
                    }
                }

                if (parameters.ContainsKey("Bill"))
                {
                    parameters.TryGetValue("Bill", out AllocationBillModel bill);
                    parameters.TryGetValue("IsSubmitBill", out bool isSubmitBill);
                    this.Bill.IsSubmitBill = isSubmitBill;
                    if (bill != null)
                    {

                        foreach (var p in bill?.Items)
                        {
                            ProductSeries.Clear();
                            p.BigUnitId = p.bigOption.Id;
                            p.SmallUnitId = p.smallOption.Id;
                            var product = new ProductModel
                            {
                                Id = p.ProductId,
                                ProductId = p.ProductId,
                                Name = p.ProductName,
                                UnitId = p.UnitId,
                                UnitName = p.UnitName,

                                BigUnitId = p.BigProductPrices.UnitId,
                                BigPriceUnit = new PriceUnit()
                                {
                                    UnitId = p.bigOption.Id,
                                    Amount = 0,
                                    //默认绑定批发价
                                    Price = p.BigProductPrices.TradePrice ?? 0,
                                    Quantity = 0,
                                    Remark = "",
                                    UnitName = p.bigOption.Name,
                                },

                                SmallUnitId = p.SmallProductPrices.UnitId,
                                SmallPriceUnit = new PriceUnit()
                                {
                                    UnitId = p.smallOption.Id,
                                    Amount = 0,
                                    //默认绑定批发价
                                    Price = p.SmallProductPrices.TradePrice ?? 0,
                                    Quantity = 0,
                                    Remark = "",
                                    UnitName = p.smallOption.Name
                                }
                            };
                            var currentStock = p.StockQuantities.Where(w => w.WareHouseId == Filter.WareHouseId).FirstOrDefault();
                            product.StockQty = currentStock != null ? currentStock.UsableQuantity : p.StockQty;
                            if (p.StockQty == 0)
                            {
                                p.StockQty = p.StockQuantities?.Sum(s => s.UsableQuantity);
                            }
                            ProductSeries.Add(product);
                        }

                        this.loaded = true;
                        Bill = bill;
                        Bill.ShipmentWareHouseId = bill.ShipmentWareHouseId;
                        Bill.IncomeWareHouseId = bill.IncomeWareHouseId;



                        if (bill.AuditedStatus) { }
                        else
                        {
                            //控制显示菜单
                            _popupMenu?.Show(34);
                        }

                        UpdateUI();
                    }
                }

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void BillAddItem(AllocationItemModel item)
        {
            if (null == Bill.Items?.FirstOrDefault(p => p.GUID == item.GUID))
            {
                item.SortIndex = Bill.Items?.Count ?? 0;
                Bill.Items?.Add(item);
            }
        }
        public void UpdateUI()
        {
            try
            {

                //红冲审核水印
                if (this.Bill.Id > 0 && this.Bill.AuditedStatus)
                    this.Bill.AuditedStatus = !this.Bill.ReversedStatus;
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
            if (this.Bill.Id > 0 && this.Bill.AuditedStatus)
                _popupMenu?.Show(3, 4, 5, 22, 23, 24);
            else
                _popupMenu?.Show(3, 4, 5, 22, 23, 24, 34);
        }
    }
}
