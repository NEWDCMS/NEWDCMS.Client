using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Products;
using Wesley.Client.Models.Sales;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;

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
    public class ReturnBillPageViewModel : ViewModelBaseCutom<ReturnBillModel>
    {
        private readonly IReturnBillService _returnBillService;

        private readonly IMicrophoneService _microphoneService;

        [Reactive] public ReturnBillUpdateModel PostMData { get; set; } = new ReturnBillUpdateModel();
        [Reactive] public ReturnItemModel Selecter { get; set; }
        [Reactive] public DispatchItemModel DispatchItem { get; set; }
        public ReactiveCommand<object, Unit> RefusedCommand { get; }
        public bool IsVisit { get; set; } = false;


        public ReturnBillPageViewModel(INavigationService navigationService,
            IReturnBillService returnBillService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IMicrophoneService microphoneService,


            IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "退货单";

            _returnBillService = returnBillService;

            _microphoneService = microphoneService;


            InitBill();

            //验证
            var valid_TerminalId = this.ValidationRule(x => x.Bill.TerminalId, _isZero, "客户未指定");
            var valid_BusinessUserId = this.ValidationRule(x => x.Bill.DeliveryUserId, _isZero, "配送员未指定");
            var valid_WareHouseId = this.ValidationRule(x => x.Bill.WareHouseId, _isZero, "仓库未指定");
            var valid_ProductCount = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "请添加商品项目");
            var valid_SelectesCount = this.ValidationRule(x => x.PaymentMethods.Selectes.Count, _isZero, "请选择支付方式");

            //初始化 
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                var result = await _returnBillService.GetInitDataAsync(calToken: cts.Token);
                if (result != null)
                {
                    var defaultAccs = result.ReturnBillAccountings.Select(s => new AccountingModel()
                    {
                        AccountCodeTypeId = s.AccountCodeTypeId,
                        Default = true,
                        AccountingOptionId = s.AccountingOptionId,
                        AccountingOptionName = s.AccountingOptionName,
                        Name = s.Name
                    }).ToList();

                    Bill.ReturnDefaultAmounts = result.ReturnDefaultAmounts;
                    PaymentMethods.Selectes = new ObservableCollection<AccountingModel>(defaultAccs);
                }
            }));

            //添加商品
            this.AddProductCommand = ReactiveCommand.Create<object>(async e =>
           {
               if (this.Bill.AuditedStatus)
               {
                   _dialogService.ShortAlert("已审核单据不能操作");
                   return;
               }

               if (!valid_TerminalId.IsValid)
               {
                   _dialogService.ShortAlert("请选择客户！");
                   ((ICommand)CustomSelected)?.Execute(null);
                   return;
               }

               if (!valid_WareHouseId.IsValid)
               {
                   _dialogService.ShortAlert("请选择仓库！");
                   ((ICommand)StockSelected)?.Execute(null);
                   return;
               }

               await this.NavigateAsync("SelectProductPage",
                   ("Reference", this.PageName),
                   ("WareHouse", WareHouse),
                   ("TerminalId", Bill.TerminalId),
                   ("SerchKey", Filter.SerchKey));
           });

            //商品编辑
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
            .Skip(1)
            .Where(x => x != null)
            .SubOnMainThread(async item =>
           {
               if (this.Bill.AuditedStatus)
               {
                   _dialogService.ShortAlert("已审核单据不能操作");
                   return;
               }

               if (item != null)
               {
                   var product = ProductSeries.Select(p => p).Where(p => p.Id == item.ProductId).FirstOrDefault();
                   if (product != null)
                   {
                       //标记为调度商品
                       product.IsDispatchProduct = this.DispatchItem != null;

                       product.UnitId = item.UnitId;
                       product.Quantity = item.Quantity;
                       product.Price = item.Price;
                       product.Amount = item.Amount;
                       product.Remark = item.Remark;
                       product.Subtotal = item.Subtotal;
                       product.UnitName = item.UnitName;

                       if (item.BigUnitId > 0)
                       {
                           product.bigOption.Name = item.UnitName;
                           product.BigPriceUnit.Quantity = item.Quantity;
                           product.BigPriceUnit.UnitId = item.BigUnitId;
                           product.BigPriceUnit.Amount = item.Amount;
                           product.BigPriceUnit.Remark = item.Remark;
                       }

                       if (item.SmallUnitId > 0)
                       {
                           product.bigOption.Name = item.UnitName;
                           product.SmallPriceUnit.Quantity = item.Quantity;
                           product.SmallPriceUnit.UnitId = item.SmallUnitId;
                           product.SmallPriceUnit.Amount = item.Amount;
                           product.SmallPriceUnit.Remark = item.Remark;
                       }

                       await this.NavigateAsync("EditProductPage", ("Product", product));
                   }
               }

               this.Selecter = null;
           });


            //提交单据
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                await this.Access(AccessGranularityEnum.ReturnBillsSave);

                var postMData = new ReturnBillUpdateModel()
                {
                    //客户
                    TerminalId = Bill.TerminalId,
                    //业务员
                    BusinessUserId = Bill.BusinessUserId,
                    //送货员
                    DeliveryUserId = Bill.DeliveryUserId,
                    //仓库
                    WareHouseId = Bill.WareHouseId,
                    //交易日期
                    TransactionDate = DateTime.Now,
                    //默认售价方式
                    DefaultAmountId = Settings.DefaultPricePlan,
                    //备注
                    Remark = Bill.Remark,
                    //优惠金额
                    PreferentialAmount = Bill.PreferentialAmount,
                    //优惠后金额
                    PreferentialEndAmount = Bill.SumAmount - Bill.PreferentialAmount,
                    //欠款金额
                    OweCash = Bill.OweCash,
                    //商品项目
                    Items = Bill.Items.Where(i => i.Quantity > 0).ToList(),
                    //收款账户
                    Accounting = PaymentMethods.Selectes.Select(a =>
                    {
                        return new AccountMaping()
                        {
                            AccountingOptionId = a.AccountingOptionId,
                            CollectionAmount = a.CollectionAmount,
                            Name = a.Name,
                            BillId = 0,
                        };
                    }).ToList(),

                    //签收逻辑
                    OrderId = Bill.ReturnReservationBillId,
                    DispatchItemId = this.DispatchItem?.Id ?? 0,
                    Latitude = GlobalSettings.Latitude,
                    Longitude = GlobalSettings.Longitude,
                };

                await SubmitAsync(postMData, Bill.Id, _returnBillService.CreateOrUpdateAsync, (result) =>
                {
                    BillId = result.Code;
                    Bill = new ReturnBillModel();
                }, token: cts.Token);

                if (IsVisit)
                {
                    //转向拜访界面
                    await this.NavigateAsync("VisitStorePage", ("BillTypeId", BillTypeEnum.ReturnBill),
                        ("BillId", BillId),
                        ("Amount", Bill.SumAmount));
                }

                return Unit.Default;
            },
            this.IsValid());

            //存储记录
            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                var c1 = this.Bill.TerminalId != 0 && this.Bill.TerminalId != (Settings.ReturnBill?.TerminalId ?? 0);
                var c2 = this.Bill.DeliveryUserId != 0 && this.Bill.DeliveryUserId != (Settings.ReturnBill?.DeliveryUserId ?? 0);
                var c3 = this.Bill.WareHouseId != 0 && this.Bill.WareHouseId != (Settings.ReturnBill?.WareHouseId ?? 0);
                var c4 = this.Bill.Items?.Count != (Settings.ReturnBill?.Items?.Count ?? 0);
                if (!this.Bill.AuditedStatus && (c1 || c2 || c3 || c4))
                {
                    if (!this.Bill.AuditedStatus)
                    {
                        var ok = await _dialogService.ShowConfirmAsync("你是否要保存单据？", "提示", "确定", "取消");
                        if (ok)
                        {
                            Settings.ReturnBill = this.Bill;
                        }
                        else
                        {
                            await _navigationService.GoBackAsync();
                        }
                    }
                }
                else
                {
                    await _navigationService.GoBackAsync();
                }
            });


            //审核
            this.AuditingDataCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                //是否具有审核权限
                await this.Access(AccessGranularityEnum.ReturnBillsApproved);
                await SubmitAsync(Bill.Id, _returnBillService.AuditingAsync, async (result) =>
                {
                    var db = Shiny.ShinyHost.Resolve<LocalDatabase>();
                    await db.SetPending(SelecterMessage.Id, true);
                }, token: cts.Token);
            }, this.WhenAny(x => x.Bill.Id, (x) => x.GetValue() > 0));


            //菜单选择
            this.SetMenus(async (x) =>
            {
                switch (x)
                {
                    case Enums.MenuEnum.PAY://支付方式
                        {
                            if (Bill.SumAmount == 0) { this.Alert("请添加商品项目！"); break; }
                            SelectPaymentMethods(("PaymentMethods", PaymentMethods),
                                ("TBalance", this.TBalance),
                                ("BillType", BillTypeEnum.ReturnBill),
                                ("Reference", PageName));
                        }
                        break;
                    case Enums.MenuEnum.DISCOUNT://优惠
                        {
                            SetDiscount((result) =>
                            {
                                decimal.TryParse(result, out decimal preferentialAmount);
                                PaymentMethods.PreferentialAmount = preferentialAmount;
                                PaymentMethods.PreferentialAmountShowFiled = preferentialAmount > 0;
                                PaymentMethods.PreferentialAmountShowFiled = preferentialAmount > 0;
                                Update();
                            }, Bill.PreferentialAmount);
                        }
                        break;
                    case Enums.MenuEnum.REMARK://整单备注
                        AllRemak((result) => { Bill.Remark = result; }, Bill.Remark);
                        break;
                    case Enums.MenuEnum.CLEAR://清空单据
                        {
                            ClearBill<ReturnBillModel, ReturnItemModel>(Bill, DoClear);
                        }
                        break;
                    case Enums.MenuEnum.PRINT://打印
                        {
                            if (!valid_ProductCount.IsValid) { this.Alert(valid_ProductCount.Message[0]); return; }
                            await SelectPrint(this.Bill);
                        }
                        break;
                    case Enums.MenuEnum.HISTORY://历史单据
                        await SelectHistory();
                        break;
                    case Enums.MenuEnum.DPRICE://默认售价
                        await SelectDefaultAmountId(Bill.ReturnDefaultAmounts);
                        break;
                }
            }, 0, 1, 2, 3, 4, 5, 6, 7);

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
                if (!valid_TerminalId.IsValid)
                {
                    _dialogService.ShortAlert(valid_TerminalId.Message[0]);
                    return;
                }

                if (!valid_WareHouseId.IsValid)
                {
                    _dialogService.ShortAlert(valid_WareHouseId.Message[0]);
                    return;
                }

                if (!valid_BusinessUserId.IsValid)
                {
                    _dialogService.ShortAlert(valid_BusinessUserId.Message[0]);
                    return;
                }

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

            this.RecognitionCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.DeliverSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.StockSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.AddProductCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }



        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                //选择客户
                if (parameters.ContainsKey("Terminaler"))
                {
                    parameters.TryGetValue<TerminalModel>("Terminaler", out TerminalModel terminaler);
                    Bill.TerminalId = terminaler != null ? terminaler.Id : 0;
                    Bill.TerminalName = terminaler != null ? terminaler.Name : "";

                    //获取余额
                    this.TBalance = await _terminalService.GetTerminalBalance(Bill.TerminalId);

                }

                //删除商品
                if (parameters.ContainsKey("DelProduct"))
                {
                    parameters.TryGetValue<ProductModel>("DelProduct", out ProductModel p);
                    if (p != null)
                    {
                        var bigProduct = Bill.Items.Where(b => b.ProductId == p.Id && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items.Where(b => b.ProductId == p.Id && b.SmallUnitId == p.UnitId).FirstOrDefault();
                        if (bigProduct != null)
                        {
                            Bill.Items.Remove(bigProduct);
                        }

                        if (smalProduct != null)
                        {
                            Bill.Items.Remove(smalProduct);
                        }

                        Update();
                    }
                }

                //编辑商品更新
                if (parameters.ContainsKey("UpdateProduct"))
                {
                    parameters.TryGetValue<ProductModel>("UpdateProduct", out ProductModel p);
                    if (p != null)
                    {
                        var bigProduct = Bill.Items.Where(b => b.ProductId == p.Id && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items.Where(b => b.Id == p.ProductId && b.SmallUnitId == p.UnitId).FirstOrDefault();
                        if (bigProduct != null)
                        {
                            bigProduct.UnitName = p.UnitName;
                            bigProduct.Quantity = p.Quantity;
                            bigProduct.Price = p.Price ?? 0;
                            bigProduct.Amount = p.Amount ?? 0;
                            bigProduct.Remark = p.Remark;
                            bigProduct.BigUnitId = p.UnitId;
                            bigProduct.Subtotal = (p.Price ?? 0) * p.Quantity;
                        }
                        if (smalProduct != null)
                        {
                            smalProduct.UnitName = p.UnitName;
                            smalProduct.Quantity = p.Quantity;
                            smalProduct.Price = p.Price ?? 0;
                            smalProduct.Amount = p.Amount ?? 0;
                            smalProduct.Remark = p.Remark;
                            smalProduct.BigUnitId = p.UnitId;
                            smalProduct.Subtotal = (p.Price ?? 0) * p.Quantity;
                        }
                        Update();
                    }
                }

                //支付方式更新
                if (parameters.ContainsKey("PaymentMethods"))
                {
                    parameters.TryGetValue<PaymentMethodBaseModel>("PaymentMethods", out PaymentMethodBaseModel paymentMethod);
                    if (paymentMethod != null)
                    {
                        this.PaymentMethods = paymentMethod;
                        Update();
                    }
                }

                //选择商品回传
                if (parameters.ContainsKey("ProductSeries"))
                {
                    parameters.TryGetValue<List<ProductModel>>("ProductSeries", out List<ProductModel> productSeries);
                    foreach (var p in productSeries)
                    {
                        //赠送商品
                        if (p.GiveProduct.BigUnitQuantity != 0)
                        {
                            var bigGive = new ReturnItemModel()
                            {
                                Id = 0,
                                UnitId = p.BigPriceUnit.UnitId,
                                ProductId = p.Id,
                                ProductName = p.ProductName,
                                StoreId = Settings.StoreId,
                                UnitName = string.IsNullOrEmpty(p.bigOption.Name) ? p.BigPriceUnit.UnitName : p.bigOption.Name,
                                Quantity = p.GiveProduct.BigUnitQuantity,
                                Price = 0,
                                Amount = 0,
                                Remark = string.IsNullOrEmpty(p.BigPriceUnit.Remark) ? "赠品" : p.BigPriceUnit.Remark,
                                BigUnitId = p.BigPriceUnit.UnitId,
                                SmallUnitId = 0,
                                Subtotal = 0,
                                IsGifts = true
                            };
                            Bill.Items.Add(bigGive);
                        }
                        if (p.GiveProduct.SmallUnitQuantity != 0)
                        {
                            var smallGive = new ReturnItemModel()
                            {
                                Id = 0,
                                UnitId = p.SmallPriceUnit.UnitId,
                                ProductId = p.Id,
                                ProductName = p.ProductName,
                                StoreId = Settings.StoreId,
                                UnitName = string.IsNullOrEmpty(p.smallOption.Name) ? p.BigPriceUnit.UnitName : p.smallOption.Name,
                                Quantity = p.GiveProduct.SmallUnitQuantity,
                                Price = 0,
                                Amount = 0,
                                Remark = string.IsNullOrEmpty(p.SmallPriceUnit.Remark) ? "赠品" : p.SmallPriceUnit.Remark,
                                SmallUnitId = p.SmallPriceUnit.UnitId,
                                BigUnitId = 0,
                                Subtotal = 0,
                                IsGifts = true
                            };
                            Bill.Items.Add(smallGive);
                        }

                        var bigItem = new ReturnItemModel()
                        {
                            Id = 0,
                            UnitId = p.BigPriceUnit.UnitId,
                            ProductId = p.Id,
                            ProductName = p.ProductName,
                            StoreId = Settings.StoreId,
                            UnitName = p.bigOption.Name,
                            Quantity = p.BigPriceUnit.Quantity,
                            Price = p.BigPriceUnit.Price ?? 0,
                            Amount = (p.BigPriceUnit.Quantity) * (p.BigPriceUnit.Price ?? 0),
                            Remark = p.BigPriceUnit.Remark,
                            BigUnitId = p.BigPriceUnit.UnitId,
                            SmallUnitId = 0,
                            IsGifts = p.BigPriceUnit.Quantity > 0 && p.BigPriceUnit.Price == 0,
                            Subtotal = (p.BigPriceUnit.Price ?? 0) * p.BigPriceUnit.Quantity
                        };
                        var smallItem = new ReturnItemModel()
                        {
                            Id = 0,
                            UnitId = p.SmallPriceUnit.UnitId,
                            ProductId = p.Id,
                            ProductName = p.ProductName,
                            StoreId = Settings.StoreId,
                            UnitName = p.smallOption.Name,
                            Quantity = p.SmallPriceUnit.Quantity,
                            Price = p.SmallPriceUnit.Price ?? 0,
                            Amount = (p.SmallPriceUnit.Quantity) * (p.SmallPriceUnit.Price ?? 0),
                            Remark = p.SmallPriceUnit.Remark,
                            SmallUnitId = p.SmallPriceUnit.UnitId,
                            BigUnitId = 0,
                            IsGifts = p.SmallPriceUnit.Quantity > 0 && p.SmallPriceUnit.Price == 0,
                            Subtotal = (p.SmallPriceUnit.Price ?? 0) * p.SmallPriceUnit.Quantity
                        };

                        if (bigItem.Quantity > 0)
                        {
                            if (bigItem.IsGifts && string.IsNullOrEmpty(bigItem.Remark))
                                bigItem.Remark = "赠品(零元开单)";

                            Bill.Items.Add(bigItem);
                        }

                        if (smallItem.Quantity > 0)
                        {
                            if (smallItem.IsGifts && string.IsNullOrEmpty(smallItem.Remark))
                                smallItem.Remark = "赠品(零元开单)";

                            Bill.Items.Add(smallItem);
                        }
                    }
                    ProductSeries = new ObservableCollection<ProductModel>(productSeries);
                    Update();
                }

                //单据预览
                ReturnBillModel bill = null;
                DispatchItemModel dispatchItem = null;
                if (parameters.ContainsKey("Bill"))
                {
                    parameters.TryGetValue("Bill", out bill);
                }

                if (parameters.ContainsKey("BillId"))
                {
                    parameters.TryGetValue("BillId", out int billId);
                    this.BillId = billId;
                    bill = await _returnBillService.GetBillAsync(billId);
                }

                if (parameters.ContainsKey("DispatchItemModel"))
                {
                    parameters.TryGetValue("DispatchItemModel", out dispatchItem);
                }


                //调度项目
                if (dispatchItem != null)
                {
                    this.SubmitText = "签收";
                    this.Title = "退货转单";
                    this.DispatchItem = dispatchItem;

                    //转换单据
                    var order = dispatchItem.ReturnReservationBill;
                    if (order == null)
                    {
                        this.Alert("参数错误");
                        await _navigationService.GoBackAsync();
                    }

                    bill = new ReturnBillModel
                    {
                        BillNumber = order?.BillNumber,

                        ReturnReservationBillId = order?.Id ?? 0,
                        TerminalId = order?.TerminalId ?? 0,
                        BusinessUserId = order?.BusinessUserId ?? 0,
                        DeliveryUserId = order?.DeliveryUserId ?? 0,
                        WareHouseId = order?.WareHouseId ?? 0,
                        DistrictId = order?.DistrictId ?? 0,

                        TerminalName = order?.TerminalName,
                        BusinessUserName = order?.BusinessUserName,
                        DeliveryUserName = order?.DeliveryUserName,
                        WareHouseName = order?.WareHouseName,
                        DistrictName = order?.DistrictName,

                        PayTypeId = order?.PayTypeId ?? 0,
                        DefaultAmountId = order?.DefaultAmountId,
                        SumAmount = order?.SumAmount ?? 0,

                        //把订单的欠款转到应收上
                        ReceivableAmount = order?.OweCash ?? 0,
                        PreferentialAmount = order?.PreferentialAmount ?? 0,
                        OweCash = 0,
                        Remark = order?.Remark
                    };

                    //商品项目
                    foreach (var item in order.Items)
                    {
                        bill.Items.Add(new ReturnItemModel()
                        {
                            Id = 0,
                            UnitId = item.UnitId,
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            StoreId = Settings.StoreId,
                            UnitName = item.UnitName,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Amount = item.Amount,
                            Remark = item.Remark,
                            SmallUnitId = item.SmallUnitId,
                            BigUnitId = item.BigUnitId,
                            IsGifts = item.IsGifts,
                            Subtotal = item.Subtotal,
                        });
                    }

                    //转换支付方式
                    foreach (var acc in order.ReturnReservationBillAccountings)
                    {
                        var account = new AccountingModel()
                        {
                            AccountingOptionId = acc.AccountingOptionId,
                            AccountingOptionName = acc.AccountingOptionName == null ? acc.AccountingOptionName : acc.Name,
                            Name = acc.Name
                        };
                        this.PaymentMethods.Selectes.Add(account);
                    }

                    this.PaymentMethods.SubAmount = bill.SumAmount;
                    this.PaymentMethods.OweCash = 0;
                    this.PaymentMethods.PreferentialAmount = bill.PreferentialAmount;
                }
                if (bill != null)
                {
                    this.loaded = true;

                    foreach (var p in bill?.Items)
                    {
                        p.Subtotal = p.Amount;

                        p.IsDispatchProduct = this.DispatchItem != null;

                        var product = new ProductModel
                        {
                            Id = p.ProductId,
                            ProductId = p.ProductId,
                            Name = p.ProductName,
                            UnitId = p.UnitId,
                            UnitName = p.UnitName,
                            Price = p.Price,

                            //标记为调度商品
                            IsDispatchProduct = this.DispatchItem != null,

                            BigUnitId = p.BigProductPrices.UnitId,
                            BigPriceUnit = new PriceUnit()
                            {
                                UnitId = p.bigOption.Id,
                                Amount = 0,
                                //默认绑定批发价
                                Price = p.BigProductPrices.TradePrice ?? 0,
                                Quantity = 0,
                                Remark = "",
                                UnitName = p.bigOption.Name
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

                    Bill = bill;
                    Bill.WareHouseId = bill.WareHouseId;
                    Bill.WareHouseName = bill.WareHouseName;

                    if (dispatchItem == null)
                        this.PaymentMethods = this.ToPaymentMethod(Bill, bill.ReturnBillAccountings);

                    Update();

                    ViewBill(Bill, _returnBillService.ReverseAsync, _returnBillService.AuditingAsync, cts.Token);
                }

                //拜访时开单
                if (parameters.GetValue<int?>("TerminalId") != null)
                {
                    Bill.TerminalId = parameters.GetValue<int?>("TerminalId") ?? 0;
                    Bill.TerminalName = parameters.GetValue<string>("TerminalName");

                    IsVisit = true;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        private void InitBill(bool clear = false)
        {
            this.BillType = BillTypeEnum.ReturnBill;
            var bill = Settings.ReturnBill;
            if (bill != null && !clear)
            {
                this.Bill = bill;
                this.Bill.Items = new ObservableCollection<ReturnItemModel>(bill?.Items);
                var defaultAccs = bill.ReturnBillAccountings.Select(
                        s => new AccountingModel()
                        {
                            AccountCodeTypeId = s.AccountCodeTypeId,
                            Default = true,
                            Name = s.Name,
                            AccountingOptionId = s.AccountingOptionId,
                            AccountingOptionName = s.AccountingOptionName
                        }).ToList();
                this.PaymentMethods.Selectes = new ObservableCollection<AccountingModel>(defaultAccs);
                this.Selecter = bill?.Items?.FirstOrDefault();
            }
            else
            {
                this.Bill = new ReturnBillModel()
                {
                    BusinessUserId = Settings.UserId,
                    CreatedOnUtc = DateTime.Now,
                    BillNumber = CommonHelper.GetBillNumber(CommonHelper.GetEnumDescription(BillType).Split(',')[1], Settings.StoreId)
                };
                if (this.Bill.Id == 0)
                {
                    this.Bill.BusinessUserId = Settings.UserId;
                    this.Bill.BusinessUserName = Settings.UserRealName;
                }
            }
        }
        private void DoClear()
        {
            InitBill(true);
            this.TBalance = null;
        }
        public void Update()
        {
            try
            {
                //合计
                this.Bill.SumAmount = decimal.Round(Bill.Items.Select(p => p.Subtotal).Sum(), 2);

                //惠
                this.Bill.PreferentialAmount = PaymentMethods.PreferentialAmount;

                //支付
                var totalAccountingAmount = PaymentMethods.Selectes.Sum(s => s.CollectionAmount);
                this.Bill.Accountings = PaymentMethods.Selectes.Select(a => { return $"{a.Name}: {string.Format("{0:F2}", a.CollectionAmount)}"; }).ToArray();

                //(欠款)待支付
                this.Bill.OweCash = this.Bill.SumAmount - this.Bill.PreferentialAmount - totalAccountingAmount;

                //用于传递
                this.PaymentMethods.SubAmount = this.Bill.SumAmount;
                this.PaymentMethods.OweCash = this.Bill.OweCash;
                this.PaymentMethods.PreferentialAmount = this.Bill.PreferentialAmount;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            if (!loaded)
            {
                loaded = true;
                ((ICommand)Load)?.Execute(null);
            }
        }
        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
