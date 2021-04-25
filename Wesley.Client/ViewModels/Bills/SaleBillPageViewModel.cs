using Wesley.Client.CustomViews;
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
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{

    public class SaleBillPageViewModel : ViewModelBaseCutom<SaleBillModel>
    {
        private readonly ISaleBillService _saleBillService;
        private readonly IMicrophoneService _microphoneService;
        //private readonly ISaleBillRepository _saleBillRepository;

        [Reactive] public SaleBillUpdateModel PostMData { get; set; } = new SaleBillUpdateModel();
        public bool IsVisit { get; set; } = false;
        [Reactive] public SaleItemModel Selecter { get; set; }
        [Reactive] public DispatchItemModel DispatchItem { get; set; }
        public ReactiveCommand<object, Unit> RefusedCommand { get; }
        public ReactiveCommand<object, Unit> ConfirmCommand { get; }
        [Reactive] public bool ShowSignInCommand { get; set; } = false;
        [Reactive] public SaleReservationBillModel OrderBill { get; set; } = new SaleReservationBillModel();

        public SaleBillPageViewModel(INavigationService navigationService,
            ISaleBillService saleBillService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IMicrophoneService microphoneService,
            IDialogService dialogService) : base(navigationService, productService,
                terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "销售单";

            _saleBillService = saleBillService;
            _microphoneService = microphoneService;

            InitBill();

            //验证
            var valid_TerminalId = this.ValidationRule(x => x.Bill.TerminalId, _isZero, "客户未指定");
            var valid_BusinessUserId = this.ValidationRule(x => x.Bill.DeliveryUserId, _isZero, "配送员未指定");
            var valid_WareHouseId = this.ValidationRule(x => x.Bill.WareHouseId, _isZero, "仓库未指定");
            var valid_ProductCount = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "请添加商品项目");
            var valid_SelectesCount = this.ValidationRule(x => x.PaymentMethods.Selectes.Count, _isZero, "请选择支付方式");
            var valid_IsVieweBill = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");

            //初始化 
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                var result = await _saleBillService.GetInitDataAsync(this.ForceRefresh);
                if (result != null)
                {
                    var defaultAcc = await _accountingService.GetDefaultAccountingAsync((int)BillType, this.ForceRefresh, calToken: cts.Token);
                    var options = new List<AccountingModel>
                    {
                        new AccountingModel()
                        {
                            AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                            Selected = true,
                            CollectionAmount = 0,
                            Name = defaultAcc.Item1?.Name,
                            AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                        }
                    };

                    PaymentMethods.Selectes = new ObservableCollection<AccountingModel>(options);
                    Bill.SaleDefaultAmounts = result.SaleDefaultAmounts;

                }
            }));

            //添加商品
            this.AddProductCommand = ReactiveCommand.Create<object>(async e =>
           {
               if (!valid_IsVieweBill.IsValid)
               {
                   _dialogService.ShortAlert("已审核单据不能操作！");
                   return;
               }

               if (!valid_TerminalId.IsValid)
               {
                   _dialogService.ShortAlert(valid_TerminalId.Message[0]);
                   ((ICommand)CustomSelected)?.Execute(null);
                   return;
               }

               if (!valid_WareHouseId.IsValid)
               {
                   _dialogService.ShortAlert(valid_WareHouseId.Message[0]);
                   ((ICommand)StockSelected)?.Execute(null);
                   return;
               }

               if (!valid_BusinessUserId.IsValid)
               {
                   _dialogService.ShortAlert(valid_BusinessUserId.Message[0]);
                   ((ICommand)DeliverSelected)?.Execute(null);
                   return;
               }

               await this.NavigateAsync("SelectProductPage", ("Reference", this.PageName),
                   ("WareHouse", WareHouse),
                   ("TerminalId", Bill.TerminalId),
                   ("Terminaler", this.Terminal),
                   ("SerchKey", Filter.SerchKey));
           });

            //编辑商品
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
                           product.BigPriceUnit.UnitId = item.BigUnitId ?? 0;
                           product.BigPriceUnit.Amount = item.Amount;
                           product.BigPriceUnit.Remark = item.Remark;
                       }

                       if (item.SmallUnitId > 0)
                       {
                           product.bigOption.Name = item.UnitName;
                           product.SmallPriceUnit.Quantity = item.Quantity;
                           product.SmallPriceUnit.UnitId = item.SmallUnitId ?? 0;
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
                await this.Access(AccessGranularityEnum.SaleBillSave);
                var postMData = new SaleBillUpdateModel()
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
                    Items = Bill.Items.ToList(),
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
                    //预收款
                    AdvanceAmount = PaymentMethods.Selectes.Where(s => s.AccountCodeTypeId == (int)AccountingCodeEnum.AdvancesReceived).FirstOrDefault()?.CollectionAmount ?? 0,
                    //预收款余额
                    AdvanceAmountBalance = this.TBalance.AdvanceAmountBalance,

                    //签收逻辑
                    OrderId = Bill.SaleReservationBillId ?? 0,
                    DispatchItemId = this.DispatchItem?.Id ?? 0,
                    Latitude = GlobalSettings.Latitude,
                    Longitude = GlobalSettings.Longitude,
                };

                if (ReferencePage.Equals("UnSalePage") || ReferencePage.Equals("UnOrderPage"))
                {
                    //如果终端异常签收大于5次，则拍照
                    if (Settings.Abnormal.Counter > 5)
                    {
                        await TakePhotograph((u, m) =>
                        {
                            var photo = new RetainPhoto
                            {
                                DisplayPath = $"{GlobalSettings.FileCenterEndpoint}HRXHJS/document/image/" + u.Id + ""
                            };
                            postMData.Photos.Add(photo);
                        });
                    }
                    if (Settings.Abnormal.Counter > 5 && postMData.Photos.Count == 0)
                    {
                        await _dialogService.ShowAlertAsync("拒绝操作，请留存拍照！", "", "取消");
                        return Unit.Default;
                    }
                }

                //提交单据
                await SubmitAsync(postMData, Bill.Id, _saleBillService.CreateOrUpdateAsync, (result) =>
                {
                    BillId = result.Code;
                    Bill = new SaleBillModel();
                }, token: cts.Token);

                if (IsVisit)
                {
                    //转向拜访界面
                    await this.NavigateAsync("VisitStorePage", ("BillTypeId", BillTypeEnum.SaleBill),
                        ("BillId", BillId),
                        ("Amount", Bill.SumAmount));
                }

                return Unit.Default;
            },
            this.IsValid());

            //存储记录
            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                var c1 = this.Bill.TerminalId != 0 && this.Bill.TerminalId != (Settings.SaleBill?.TerminalId ?? 0);
                var c2 = this.Bill.DeliveryUserId != 0 && this.Bill.DeliveryUserId != (Settings.SaleBill?.DeliveryUserId ?? 0);
                var c3 = this.Bill.WareHouseId != 0 && this.Bill.WareHouseId != (Settings.SaleBill?.WareHouseId ?? 0);
                var c4 = this.Bill.Items?.Count != (Settings.SaleBill?.Items?.Count ?? 0);
                if (!this.Bill.AuditedStatus && (c1 || c2 || c3 || c4))
                {
                    if (!this.Bill.AuditedStatus)
                    {
                        var ok = await _dialogService.ShowConfirmAsync("你是否要保存单据？", "提示", "确定", "取消");
                        if (ok)
                        {
                            Settings.SaleBill = this.Bill;
                            //await _saleBillRepository.AddAsync(this.Bill);
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

            //显示拒签
            this.WhenAnyValue(x => x.DispatchItem)
              .Subscribe(x =>
              {
                  if (x != null)
                  {
                      this.EnableOperation = false;
                      this.EnableRefused = true;
                  }
              })
              .DisposeWith(this.DeactivateWith);

            //审核
            this.AuditingDataCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                //是否具有审核权限
                await this.Access(AccessGranularityEnum.SaleBillApproved);
                await SubmitAsync(Bill.Id, _saleBillService.AuditingAsync, async (result) =>
                {
                    var db = Shiny.ShinyHost.Resolve<LocalDatabase>();
                    await db.SetPending(SelecterMessage.Id, true);
                }, token: cts.Token);
            }, this.WhenAny(x => x.Bill.Id, (x) => x.GetValue() > 0));

            //拒签
            this.RefusedCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                try
                {
                    var ok = await _dialogService.ShowConfirmAsync("你确定要放弃签收吗？", "", "确定", "取消");
                    if (ok)
                    {
                        if (this.DispatchItem != null && ReferencePage.Equals("UnOrderPage"))
                        {
                            var postMData = new DeliverySignUpdateModel()
                            {
                                //关联调拨单
                                DispatchItemId = this.DispatchItem?.Id ?? 0,
                                DispatchBillId = this.DispatchItem?.DispatchBillId ?? 0,
                                StoreId = Settings.StoreId,
                                BillId = Bill.Id,
                                BillTypeId = this.DispatchItem.BillTypeId,
                                //
                                Latitude = GlobalSettings.Latitude,
                                Longitude = GlobalSettings.Longitude,
                                Signature = ""
                            };
                            await SubmitAsync(postMData, postMData.Id, _saleBillService.RefusedConfirmAsync, async (result) =>
                            {
                                await _navigationService.GoBackAsync();
                            });
                        }
                        else if (this.DispatchItem == null && ReferencePage.Equals("UnSalePage"))
                        {
                            var postMData = new DeliverySignUpdateModel()
                            {
                                //不关联调拨单
                                DispatchItemId = 0,
                                DispatchBillId = 0,
                                StoreId = Settings.StoreId,
                                BillId = Bill.Id,
                                BillTypeId = (int)BillTypeEnum.SaleBill,
                                //
                                Latitude = GlobalSettings.Latitude,
                                Longitude = GlobalSettings.Longitude,
                                Signature = ""
                            };
                            await SubmitAsync(postMData, postMData.Id, _saleBillService.RefusedConfirmAsync, async (result) =>
                            {
                                await _navigationService.GoBackAsync();
                            }, token: cts.Token);
                        }
                    }
                }
                catch (Exception)
                {
                    await _dialogService.ShowAlertAsync("拒签失败，服务器请求错误！", "", "取消");
                }
            });

            //签收
            this.ConfirmCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                try
                {
                    //如果签收距离>50 则计数
                    if ((Terminal.Distance ?? 0) > 50)
                    {
                        var tmp = Settings.Abnormal;
                        if (tmp != null)
                        {
                            tmp.Id = Terminal.Id;
                            tmp.Counter += 1;
                            Settings.Abnormal = tmp;
                        }
                        else
                        {
                            Settings.Abnormal = new AbnormalNum { Id = Terminal.Id, Counter = 1 };
                        }
                    }

                    var signature = await CrossDiaglogKit.Current.GetSignaturePadAsync("手写签名", "", Keyboard.Default, defaultValue: "", placeHolder: "请手写签名...");
                    if (!string.IsNullOrEmpty(signature))
                    {
                        //来自订单
                        if (this.DispatchItem != null && ReferencePage.Equals("UnOrderPage"))
                        {
                            ((ICommand)SubmitDataCommand)?.Execute(null);
                        }
                        //来自销售单
                        else if (this.DispatchItem == null && ReferencePage.Equals("UnSalePage"))
                        {
                            var postMData = new DeliverySignUpdateModel()
                            {
                                //不关联调拨单
                                DispatchItemId = 0,
                                DispatchBillId = 0,
                                StoreId = Settings.StoreId,
                                BillId = Bill.Id,
                                BillTypeId = (int)BillTypeEnum.SaleBill,
                                //
                                Latitude = GlobalSettings.Latitude,
                                Longitude = GlobalSettings.Longitude,
                                Signature = signature
                            };

                            //如果终端异常签收大于5次，则拍照
                            if (Settings.Abnormal.Counter > 5)
                            {
                                await TakePhotograph((u, m) =>
                                {
                                    var photo = new RetainPhoto
                                    {
                                        DisplayPath = $"{GlobalSettings.FileCenterEndpoint}HRXHJS/document/image/" + u.Id + ""
                                    };
                                    postMData.RetainPhotos.Add(photo);
                                });
                            }

                            if (Settings.Abnormal.Counter > 5 && postMData.RetainPhotos.Count == 0)
                            {
                                await _dialogService.ShowAlertAsync("拒绝操作，请留存拍照！", "", "取消");
                                return;
                            }

                            await SubmitAsync(postMData, postMData.Id, _saleBillService.DeliverySignConfirmAsync, async (result) =>
                            {
                                await _navigationService.GoBackAsync();
                            }, token: cts.Token);
                        }
                    }

                }
                catch (Exception)
                {
                    await _dialogService.ShowAlertAsync("签收失败，服务器请求错误！", "", "取消");
                }
            });

            //菜单选择
            this.SetMenus(async (x) =>
            {
                switch (x)
                {
                    case MenuEnum.PAY://支付方式
                        {
                            if (Bill.SumAmount == 0) { this.Alert("请添加商品项目！"); break; }
                            var payments = this.PaymentMethods;
                            SelectPaymentMethods(("PaymentMethods", payments),
                                        ("TBalance", this.TBalance),
                                        ("BillType", this.BillType),
                                        ("Reference", PageName));
                        }
                        break;
                    case MenuEnum.ARREARS://欠款
                        {
                            SetOweCash((result) =>
                            {
                                decimal.TryParse(result, out decimal oweCash);
                                PaymentMethods.OweCash = oweCash;
                                PaymentMethods.OweCashShowFiled = oweCash > 0;
                                PaymentMethods.OweCashShowFiled = oweCash > 0;
                                UpdateUI();
                            }, Bill.OweCash);
                        }
                        break;
                    case MenuEnum.DISCOUNT://优惠
                        {
                            SetDiscount((result) =>
                            {
                                decimal.TryParse(result, out decimal preferentialAmount);
                                PaymentMethods.PreferentialAmount = preferentialAmount;
                                PaymentMethods.PreferentialAmountShowFiled = preferentialAmount > 0;
                                PaymentMethods.PreferentialAmountShowFiled = preferentialAmount > 0;
                                UpdateUI();
                            }, Bill.PreferentialAmount);
                        }
                        break;
                    case MenuEnum.REMARK://整单备注
                        AllRemak((result) =>
                        {

                            Bill.Remark = result;

                        }
                        , Bill.Remark);
                        break;
                    case MenuEnum.CLEAR://清空单据
                        {
                            ClearBill<SaleBillModel, SaleItemModel>(Bill, DoClear);
                        }
                        break;
                    case MenuEnum.PRINT://打印
                        {
                            if (!valid_ProductCount.IsValid) { this.Alert(valid_IsVieweBill.Message[0]); return; }
                            await SelectPrint(this.Bill);
                        }
                        break;
                    case MenuEnum.HISTORY://历史单据
                        await SelectHistory();
                        break;
                    case MenuEnum.DPRICE://默认售价
                        await SelectDefaultAmountId(Bill.SaleDefaultAmounts);
                        break;
                }
            }, 0, 2, 3, 4, 5, 6, 7);

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
            this.StockSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.DeliverSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
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
                    parameters.TryGetValue("Terminaler", out TerminalModel terminaler);
                    Bill.TerminalId = terminaler != null ? terminaler.Id : 0;
                    Bill.TerminalName = terminaler != null ? terminaler.Name : "";
                    //获取余额
                    this.TBalance = await _terminalService.GetTerminalBalance(Bill.TerminalId);
                }

                //删除商品
                if (parameters.ContainsKey("DelProduct"))
                {
                    parameters.TryGetValue("DelProduct", out ProductModel p);
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

                        UpdateUI();
                    }
                }

                //编辑商品更新
                if (parameters.ContainsKey("UpdateProduct"))
                {
                    parameters.TryGetValue("UpdateProduct", out ProductModel p);
                    if (p != null)
                    {
                        var bigProduct = Bill.Items.Where(b => b.ProductId == p.Id && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items.Where(b => b.ProductId == p.Id && b.SmallUnitId == p.UnitId).FirstOrDefault();
                        if (bigProduct != null)
                        {
                            bigProduct.UnitName = p.UnitName;
                            bigProduct.Quantity = p.Quantity;
                            bigProduct.Price = p.Price ?? 0;
                            bigProduct.Amount = p.Amount ?? 0;
                            bigProduct.Remark = p.Remark;
                            bigProduct.BigUnitId = p.UnitId;
                            bigProduct.Subtotal = (p.Price ?? 0) * p.Quantity;
                            bigProduct.IsGifts = (p.Price ?? 0) == 0;
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
                            smalProduct.IsGifts = (p.Price ?? 0) == 0;
                        }
                        UpdateUI();
                    }
                }

                //支付方式更新
                if (parameters.ContainsKey("PaymentMethods"))
                {
                    parameters.TryGetValue("PaymentMethods", out PaymentMethodBaseModel paymentMethod);
                    if (paymentMethod != null)
                    {
                        this.PaymentMethods = paymentMethod;
                        UpdateUI();
                    }
                }

                //选择商品回传
                if (parameters.ContainsKey("ProductSeries"))
                {
                    parameters.TryGetValue("ProductSeries", out List<ProductModel> productSeries);
                    foreach (var p in productSeries)
                    {
                        //赠送商品
                        if (p.GiveProduct.BigUnitQuantity != 0)
                        {
                            var bigGive = new SaleItemModel()
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
                            var smallGive = new SaleItemModel()
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

                        var bigItem = new SaleItemModel()
                        {
                            Id = 0,
                            UnitId = p.BigPriceUnit.UnitId,
                            ProductId = p.Id,
                            ProductName = p.ProductName,
                            StoreId = Settings.StoreId,
                            UnitName = string.IsNullOrEmpty(p.bigOption.Name) ? p.BigPriceUnit.UnitName : p.bigOption.Name,
                            Quantity = p.BigPriceUnit.Quantity,
                            Price = p.BigPriceUnit.Price ?? 0,
                            Amount = (p.BigPriceUnit.Quantity) * (p.BigPriceUnit.Price ?? 0),
                            Remark = p.BigPriceUnit.Remark,
                            BigUnitId = p.BigPriceUnit.UnitId,
                            SmallUnitId = 0,
                            IsGifts = p.BigPriceUnit.Quantity > 0 && p.BigPriceUnit.Price == 0,
                            Subtotal = (p.BigPriceUnit.Price ?? 0) * p.BigPriceUnit.Quantity,
                            //
                            CampaignId = p.CampaignId,
                            CampaignName = p.CampaignName,
                            CampaignBuyProductId = p.TypeId == 1 ? p.Id : 0,
                            CampaignGiveProductId = p.TypeId == 2 ? p.Id : 0
                        };
                        var smallItem = new SaleItemModel()
                        {
                            Id = 0,
                            UnitId = p.SmallPriceUnit.UnitId,
                            ProductId = p.Id,
                            ProductName = p.ProductName,
                            StoreId = Settings.StoreId,
                            UnitName = string.IsNullOrEmpty(p.smallOption.Name) ? p.SmallPriceUnit.UnitName : p.smallOption.Name,
                            Quantity = p.SmallPriceUnit.Quantity,
                            Price = p.SmallPriceUnit.Price ?? 0,
                            Amount = (p.SmallPriceUnit.Quantity) * (p.SmallPriceUnit.Price ?? 0),
                            Remark = p.SmallPriceUnit.Remark,
                            SmallUnitId = p.SmallPriceUnit.UnitId,
                            BigUnitId = 0,
                            IsGifts = p.SmallPriceUnit.Quantity > 0 && p.SmallPriceUnit.Price == 0,
                            Subtotal = (p.SmallPriceUnit.Price ?? 0) * p.SmallPriceUnit.Quantity,
                            //
                            CampaignId = p.CampaignId,
                            CampaignName = p.CampaignName,
                            CampaignBuyProductId = p.TypeId == 1 ? p.Id : 0,
                            CampaignGiveProductId = p.TypeId == 2 ? p.Id : 0
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
                    UpdateUI();
                }

                SaleBillModel bill = null;
                DispatchItemModel dispatchItem = null;

                if (parameters.ContainsKey("Bill"))
                {
                    parameters.TryGetValue("Bill", out bill);
                }

                if (parameters.ContainsKey("BillId"))
                {
                    parameters.TryGetValue("BillId", out int billId);
                    this.BillId = billId;
                    bill = await _saleBillService.GetBillAsync(billId);
                }

                if (parameters.ContainsKey("DispatchItemModel"))
                {
                    parameters.TryGetValue("DispatchItemModel", out dispatchItem);
                }

                //调度项目
                if (dispatchItem != null && bill == null)
                {
                    this.SubmitText = "签收";
                    this.Title = "销售转单";
                    this.DispatchItem = dispatchItem;

                    //转换单据
                    var order = dispatchItem.SaleReservationBill;
                    this.OrderBill = order;

                    if (order == null)
                    {
                        this.Alert("参数错误");
                        await _navigationService.GoBackAsync();
                    }

                    //重新构造销售单
                    bill = new SaleBillModel
                    {
                        BillNumber = CommonHelper.GetBillNumber(CommonHelper.GetEnumDescription(BillType).Split(',')[1], Settings.StoreId),
                        SaleReservationBillNumber = order?.BillNumber,
                        SaleReservationBillId = order?.Id ?? 0,

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
                        ReceivableAmount = order?.ReceivableAmount ?? 0,
                        //订单优惠金额 转到销售优惠上
                        PreferentialAmount = order?.PreferentialAmount ?? 0,
                        OweCash = 0,
                        Remark = order?.Remark,
                        AuditedStatus = false,
                        ReversedStatus = order.ReversedStatus
                    };

                    //商品项目
                    var products = new List<ProductModel>();
                    foreach (var item in order.Items)
                    {
                        var sitem = new SaleItemModel()
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
                            SaleProductTypeId = item.SaleProductTypeId,
                            GiveTypeId = item.GiveTypeId,
                            CampaignId = item.CampaignId,
                            CampaignBuyProductId = item.CampaignBuyProductId,
                            CampaignGiveProductId = item.CampaignGiveProductId,
                            CostContractId = item.CostContractId,
                            CostContractItemId = item.CostContractItemId,
                            CostContractMonth = item.CostContractMonth,
                            Subtotal = item.Amount,
                        };

                        var product = new ProductModel
                        {
                            Id = item.ProductId,
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            Name = item.ProductName,
                            UnitId = item.UnitId,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Amount = item.Amount,
                            Remark = item.Remark,
                            Subtotal = item.Subtotal,
                            UnitName = item.UnitName
                        };

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

                        products.Add(product);
                        bill.Items.Add(sitem);
                    }

                    ProductSeries = new ObservableCollection<ProductModel>(products);

                    //销售单默认付款
                    var defaultAcc = await _accountingService.GetDefaultAccountingAsync((int)BillType);
                    var options = new List<AccountingModel>
                    {
                        new AccountingModel()
                        {
                            AccountingOptionId = defaultAcc?.Item1?.Id ?? 0,
                            Selected = true,
                            CollectionAmount = bill.ReceivableAmount,
                            Name = defaultAcc.Item1?.Name,
                            AccountCodeTypeId = defaultAcc?.Item1?.AccountCodeTypeId ?? 0
                        }
                    };

                    this.PaymentMethods.Selectes = new ObservableCollection<AccountingModel>(options);
                    //总计
                    this.PaymentMethods.SubAmount = bill.SumAmount;
                    //本次收款
                    this.PaymentMethods.CurrentCollectionAmount = bill.ReceivableAmount;
                    //优惠
                    this.PaymentMethods.PreferentialAmount = bill.PreferentialAmount;

                    //来自订单签收单
                    if (ReferencePage.Equals("UnOrderPage"))
                    {
                        this.ShowAddProduct = false;
                        this.ShowSignInCommand = true;
                    }

                    this.Bill = bill;
                    this.Bill.OweCash = 0;

                    UpdateUI();
                }

                //预览单据时（销售单时）
                if (dispatchItem == null && bill != null)
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
                        this.PaymentMethods = this.ToPaymentMethod(Bill, bill.SaleBillAccountings);

                    //来自销售签收单
                    if (ReferencePage.Equals("UnSalePage"))
                    {
                        this.ShowAddProduct = false;
                        this.ShowSignInCommand = true;
                    }

                    UpdateUI();
                    ViewBill(Bill, _saleBillService.ReverseAsync, _saleBillService.AuditingAsync);
                }

                //拜访时开单
                if (parameters.ContainsKey("TerminalId") && parameters.ContainsKey("TerminalName"))
                {
                    parameters.TryGetValue<int>("TerminalId", out int terminalId);
                    parameters.TryGetValue<string>("TerminalName", out string terminalName);
                    Bill.TerminalId = terminalId;
                    Bill.TerminalName = terminalName;
                    IsVisit = true;
                }

                //获取客户
                var tt = await _terminalService.GetTerminalAsync(Bill.TerminalId);
                if (tt != null)
                {
                    this.Terminal = tt;
                    this.Terminal.Distance = MapHelper.CalculateDistance(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0, tt.Location_Lat ?? 0, tt.Location_Lng ?? 0);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void UpdateUI()
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

        private void InitBill(bool clear = false)
        {
            this.BillType = BillTypeEnum.SaleBill;
            var bill = Settings.SaleBill;
            if (bill != null && !clear)
            {
                this.Bill = bill;
                this.Bill.Items = new ObservableCollection<SaleItemModel>(bill?.Items);
                if (!this.PaymentMethods.Selectes.Any())
                {
                    var defaultAccs = bill.SaleBillAccountings.Select(
                            s => new AccountingModel()
                            {
                                AccountCodeTypeId = s.AccountCodeTypeId,
                                Default = true,
                                Name = s.Name,
                                AccountingOptionId = s.AccountingOptionId,
                                AccountingOptionName = s.AccountingOptionName
                            }).ToList();
                    this.PaymentMethods.Selectes = new ObservableCollection<AccountingModel>(defaultAccs);
                }
                this.Selecter = bill?.Items?.FirstOrDefault();
            }
            else
            {
                this.Bill = new SaleBillModel()
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
            ((ICommand)Load)?.Execute(null);
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
