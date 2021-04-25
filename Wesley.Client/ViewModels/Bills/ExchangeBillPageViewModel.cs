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
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;


namespace Wesley.Client.ViewModels
{

    public class ExchangeBillPageViewModel : ViewModelBaseCutom<ExchangeBillModel>
    {
        private readonly IExchangeBillService _exchangeBillService;
        private readonly IMicrophoneService _microphoneService;
        private readonly ISaleBillService _saleBillService;
        //private readonly IMapper _mapper;

        [Reactive] public ExchangeBillUpdateModel PostMData { get; set; } = new ExchangeBillUpdateModel();
        [Reactive] public ExchangeItemModel Selecter { get; set; }
        [Reactive] public DispatchItemModel DispatchItem { get; set; }

        public bool IsVisit { get; set; } = false;
        [Reactive] public bool IsDeliveryExpanded { get; set; }

        [Reactive] public ObservableCollection<WeekDay> WeekDays { get; set; } = new ObservableCollection<WeekDay>();
        public ReactiveCommand<string, Unit> DeliverSelectedCommand { get; set; }

        public ReactiveCommand<object, Unit> RefusedCommand { get; }
        public ReactiveCommand<object, Unit> ConfirmCommand { get; }
        [Reactive] public bool ShowSignInCommand { get; set; } = false;


        public IReactiveCommand ConfirmDelive => ReactiveCommand.Create(() =>
        {
            IsDeliveryExpanded = false; IsExpanded = false;
        });

        [Reactive] public string AMTimeRange { get; set; }
        [Reactive] public string AMTimeRangeBG { get; set; } = "#ffffff";
        [Reactive] public string PMTimeRange { get; set; }
        [Reactive] public string PMTimeRangeBG { get; set; } = "#ffffff";
        [Reactive] public WeekDay DeliverDate { get; set; } = new WeekDay();


        public ExchangeBillPageViewModel(INavigationService navigationService,
            IExchangeBillService exchangeBillService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IMicrophoneService microphoneService,
            ISaleBillService saleBillService,
            //IMapper mapper,
            IDialogService dialogService) : base(navigationService,
                productService, terminalService,
                userService, wareHousesService,
                accountingService, dialogService)
        {
            Title = "换货单";

            _saleBillService = saleBillService;
            _exchangeBillService = exchangeBillService;
            _microphoneService = microphoneService;
            //_mapper = mapper;

            InitBill();

            #region //配送日期指定
            var weekDays = new List<WeekDay>();
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddDays(7);
            while (endTime.Subtract(startTime).Days > 0)
            {
                var wd = new WeekDay
                {
                    Wname = $"{startTime:MM-dd} - {GlobalSettings.GetWeek(startTime.DayOfWeek.ToString())}",
                    Date = startTime,
                    AMTimeRange = $"09:00-12:00",
                    PMTimeRange = $"15:00-21:00",
                    SelectedCommand = ReactiveCommand.Create<WeekDay>(r =>
                    {
                        this.WeekDays.ForEach(s => { s.Selected = false; });
                        r.Selected = !r.Selected;
                        this.DeliverDate = r;
                        this.AMTimeRange = r.AMTimeRange;
                        this.PMTimeRange = r.PMTimeRange;
                    })
                };
                weekDays.Add(wd);
                startTime = startTime.AddDays(1);
            }
            var defaultWd = weekDays.FirstOrDefault();
            if (defaultWd != null)
            {
                defaultWd.Selected = true;
                this.AMTimeRange = defaultWd.AMTimeRange;
                this.PMTimeRange = defaultWd.PMTimeRange;
            }
            this.WeekDays = new ObservableCollection<WeekDay>(weekDays);

            #endregion

            //验证
            var valid_TerminalId = this.ValidationRule(x => x.Bill.TerminalId, _isZero, "客户未指定");
            var valid_BusinessUserId = this.ValidationRule(x => x.Bill.DeliveryUserId, _isZero, "配送员未指定");
            var valid_WareHouseId = this.ValidationRule(x => x.Bill.WareHouseId, _isZero, "仓库未指定");
            var valid_ProductCount = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "请添加商品项目");

            //添加商品
            this.AddProductCommand = ReactiveCommand.Create<object>(async e =>
           {
               if (this.Bill.AuditedStatus && !this.ReferencePage.Equals("UnDeliveryPage"))
               {
                   _dialogService.ShortAlert("已审核单据不能操作");
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

               //转向商品选择
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

            //提交表单
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var vaild = await this.Access(AccessGranularityEnum.ExchangeBillsSave);
                if (!vaild) return Unit.Default;

                if (this.Bill.AuditedStatus || DispatchItem != null)
                {
                    _dialogService.ShortAlert("已审核单据不能操作");
                    return Unit.Default;
                }

                //结算方式
                Bill.PayTypeId = 0;
                Bill.PayTypeName = "";

                var postMData = new ExchangeBillUpdateModel()
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
                    PreferentialAmount = 0,
                    //优惠后金额
                    PreferentialEndAmount = 0,
                    //欠款金额
                    OweCash = 0,
                    //商品项目
                    Items = Bill.Items,
                    //收款账户
                    //预收款
                    AdvanceAmount = 0,
                    //预收款余额
                    AdvanceAmountBalance = 0,
                    //配送时间
                    DeliverDate = this.DeliverDate?.Date ?? DateTime.Now,
                    AMTimeRange = this.DeliverDate?.AMTimeRange,
                    PMTimeRange = this.DeliverDate?.PMTimeRange
                };

                await SubmitAsync(postMData, Bill.Id, _exchangeBillService.CreateOrUpdateAsync, (result) =>
                {
                    BillId = result.Code;
                    //清空单据
                    Bill = new ExchangeBillModel();
                }, !IsVisit, token: cts.Token);

                if (IsVisit) //拜访时开单
                {
                    //转向拜访界面
                    await this.NavigateAsync("VisitStorePage", ("BillTypeId", BillTypeEnum.ExchangeBill),
                        ("BillId", BillId),
                        ("Amount", Bill.SumAmount));
                }

                return Unit.Default;
            },
            this.IsValid());

            //存储记录
            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                if (!this.Bill.AuditedStatus && this.Bill.TerminalId != 0 && this.Bill.TerminalId != Settings.ExchangeBill?.TerminalId)
                {
                    var ok = await _dialogService.ShowConfirmAsync("你是否要保存单据？", "提示", "确定", "取消");
                    if (ok)
                    {
                        if (!this.Bill.AuditedStatus)
                            Settings.ExchangeBill = this.Bill;
                    }
                }

                await _navigationService.GoBackAsync();
            });

            //审核
            this.AuditingDataCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                //是否具有审核权限
                await this.Access(AccessGranularityEnum.ExchangeBillsApproved);
                await SubmitAsync(Bill.Id, _exchangeBillService.AuditingAsync, async (result) =>
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
                    case MenuEnum.REMARK://整单备注
                        AllRemak((result) => { Bill.Remark = result; }, Bill.Remark);
                        break;
                    case MenuEnum.CLEAR://清空单据
                        ClearBill<ExchangeBillModel, ExchangeItemModel>(Bill, DoClear);
                        break;
                    case MenuEnum.PRINT://打印
                        {
                            if (!valid_ProductCount.IsValid) { this.Alert(valid_ProductCount.Message[0]); return; }
                            await SelectPrint(this.Bill);
                        }
                        break;
                    case MenuEnum.HISTORY://历史单据
                        await SelectHistory();
                        break;
                    case MenuEnum.DPRICE://默认售价
                        await SelectDefaultAmountId(Bill.ExchangeBillDefaultAmounts);
                        break;
                    case MenuEnum.DELIVERYTIME://配送时间
                        {
                            IsVisible = false;
                            IsFooterVisible = false;

                            IsExpanded = !IsExpanded;
                            IsDeliveryExpanded = !IsDeliveryExpanded;
                        }
                        break;
                }
            }, 3, 4, 5, 6, 37);

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
                IsDeliveryExpanded = false;
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

            //配送日期指定
            this.DeliverSelectedCommand = ReactiveCommand.Create<string>((date) =>
            {
                if (date == "AM")
                {
                    this.AMTimeRangeBG = "#eeeeee";
                    this.PMTimeRangeBG = "#ffffff";
                }
                else
                {
                    this.AMTimeRangeBG = "#ffffff";
                    this.PMTimeRangeBG = "#eeeeee";
                }
            });

            //显示签收
            this.WhenAnyValue(x => x.DispatchItem)
              .Subscribe(x =>
              {
                  if (x != null)
                  {
                      this.EnableOperation = false;
                      this.ShowSignInCommand = true;
                      this.ShowAddProduct = false;
                      this.EnabledSubmitBtn = false;
                      Title = "换货签收";
                  }
              })
              .DisposeWith(this.DeactivateWith);

            //拒签
            this.RefusedCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                try
                {
                    var ok = await _dialogService.ShowConfirmAsync("你确定要放弃签收吗？", "", "确定", "取消");
                    if (ok)
                    {
                        //换货单拒签时生成退货单
                        if (this.DispatchItem != null)
                        {
                            var postMData = new DeliverySignUpdateModel()
                            {
                                DispatchItemId = this.DispatchItem?.Id ?? 0,
                                DispatchBillId = this.DispatchItem?.DispatchBillId ?? 0,
                                StoreId = Settings.StoreId,
                                BillId = Bill.Id,
                                BillTypeId = (int)BillTypeEnum.ExchangeBill,
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
                    }
                }
                catch (Exception)
                {
                    await _dialogService.ShowAlertAsync("拒签失败，服务器请求错误！", "", "取消");
                }
            });
            //换货签收
            this.ConfirmCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                try
                {
                    if (this.DispatchItem != null)
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
                            var postMData = new DeliverySignUpdateModel()
                            {
                                DispatchItemId = this.DispatchItem?.Id ?? 0,
                                DispatchBillId = this.DispatchItem?.DispatchBillId ?? 0,
                                StoreId = Settings.StoreId,
                                BillId = Bill.Id,
                                BillTypeId = (int)BillTypeEnum.ExchangeBill,
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

                            await SubmitAsync(postMData, postMData.Id, _exchangeBillService.DeliverySignConfirmAsync, async (result) =>
                            {
                                await _navigationService.GoBackAsync();
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    await _dialogService.ShowAlertAsync("换货签收失败，服务器请求错误！", "", "取消");
                }
            });


            this.StockSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.DeliverSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.AddProductCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

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

                //删除/更新商品回传
                if (parameters.ContainsKey("DelProduct"))//删除商品
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
                        UpdateUI();
                    }
                }

                //选择商品回传
                if (parameters.ContainsKey("ProductSeries"))
                {
                    parameters.TryGetValue("ProductSeries", out List<ProductModel> productSeries);
                    foreach (var p in productSeries)
                    {
                        //临期换货
                        if (p.GiveProduct.BigUnitQuantity != 0)
                        {
                            var bigGive = new ExchangeItemModel()
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
                                Remark = string.IsNullOrEmpty(p.BigPriceUnit.Remark) ? "临期换货" : p.BigPriceUnit.Remark,
                                BigUnitId = p.BigPriceUnit.UnitId,
                                SmallUnitId = 0,
                                Subtotal = 0,
                                IsGifts = true
                            };
                            Bill.Items.Add(bigGive);
                        }
                        if (p.GiveProduct.SmallUnitQuantity != 0)
                        {
                            var smallGive = new ExchangeItemModel()
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
                                Remark = string.IsNullOrEmpty(p.SmallPriceUnit.Remark) ? "临期换货" : p.SmallPriceUnit.Remark,
                                SmallUnitId = p.SmallPriceUnit.UnitId,
                                BigUnitId = 0,
                                Subtotal = 0,
                                IsGifts = true
                            };
                            Bill.Items.Add(smallGive);
                        }

                        var bigItem = new ExchangeItemModel()
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
                        var smallItem = new ExchangeItemModel()
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
                                bigItem.Remark = "临期换货";
                            else if (bigItem.CampaignBuyProductId > 0 || bigItem.CampaignGiveProductId > 0)
                            {
                                bigItem.Remark = bigItem.CampaignName;
                            }

                            Bill.Items.Add(bigItem);
                        }
                        if (smallItem.Quantity > 0)
                        {
                            if (smallItem.IsGifts && string.IsNullOrEmpty(smallItem.Remark))
                                smallItem.Remark = "临期换货";
                            else if (bigItem.CampaignBuyProductId > 0 || bigItem.CampaignGiveProductId > 0)
                            {
                                bigItem.Remark = bigItem.CampaignName;
                            }
                            Bill.Items.Add(smallItem);
                        }
                    }

                    ProductSeries = new ObservableCollection<ProductModel>(productSeries);
                    UpdateUI();
                }

                //预览单据
                if (parameters.ContainsKey("Bill"))
                {
                    parameters.TryGetValue("Bill", out ExchangeBillModel bill);
                    parameters.TryGetValue("DispatchItemModel", out DispatchItemModel dispatchItem);

                    //调度项目
                    if (dispatchItem != null)
                    {
                        this.SubmitText = "换货";
                        this.Title = "换货确认";
                        this.DispatchItem = dispatchItem;
                        this.EnableOperation = true;

                        //转换单据
                        var exchange = dispatchItem.ExchangeBillModel;
                        if (exchange == null)
                        {
                            this.Alert("参数错误");
                            await _navigationService.GoBackAsync();
                        }

                        if (dispatchItem.BillId > 0)
                        {
                            bill = await _exchangeBillService.GetBillAsync(dispatchItem.BillId);
                            bill.BusinessUserName = dispatchItem.BusinessUserName;
                            bill.TerminalName = dispatchItem.TerminalName;
                            bill.DeliveryUserName = dispatchItem.DeliveryUserName;
                            bill.Remark = "临期换货";
                        }
                    }

                    if (bill != null)
                    {
                        foreach (var p in bill?.Items)
                        {
                            p.Subtotal = p.Amount;

                            var product = new ProductModel
                            {
                                Id = p.ProductId,
                                ProductId = p.ProductId,
                                Name = p.ProductName,
                                UnitId = p.UnitId,
                                UnitName = p.UnitName,
                                Price = p.Price,

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
                        this.PaymentMethods = this.ToPaymentMethod(Bill, bill.ExchangeBillAccountings);


                        //来自订单签收
                        if (ReferencePage.Equals("UnOrderPage"))
                        {
                            this.ShowAddProduct = false;
                            this.ShowSignInCommand = true;
                        }

                        UpdateUI();

                        ViewBill(Bill, _exchangeBillService.ReverseAsync, _exchangeBillService.AuditingAsync);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 更新价格
        /// </summary>
        public void UpdateUI()
        {
            try
            {
                //合计
                this.Bill.SumAmount = decimal.Round(Bill.Items.Select(p => p.Subtotal).Sum(), 2);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void InitBill(bool clear = false)
        {
            this.BillType = BillTypeEnum.ExchangeBill;
            var bill = Settings.ExchangeBill;
            if (bill != null && !clear)
            {
                this.Bill = bill;
                this.Bill.Items = new ObservableCollection<ExchangeItemModel>(bill?.Items);
                this.Selecter = bill?.Items?.FirstOrDefault();
            }
            else
            {
                this.Bill = new ExchangeBillModel()
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
        public override void OnAppearing()
        {
            base.OnAppearing();
        }
        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
