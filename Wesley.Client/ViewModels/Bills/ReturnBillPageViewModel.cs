using Acr.UserDialogs;
using Wesley.Client.CustomViews;
using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Products;
using Wesley.Client.Models.Sales;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Pages;
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
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{
    public class ReturnBillPageViewModel : ViewModelBaseCutom<ReturnBillModel>
    {
        private readonly IReturnBillService _returnBillService;
        private readonly IMicrophoneService _microphoneService;
        private readonly static object _lock = new object();

        public bool IsLocalBill { get; set; } = false;
        public bool IsNeedShowPrompt { get; set; } = true;
        public bool IsNeedPrint { get; set; } = false;
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
            IDialogService dialogService
            ) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "退货单";

            _returnBillService = returnBillService;

            _microphoneService = microphoneService;


            InitBill();

            //验证
            var valid_IsReversed = this.ValidationRule(x => x.Bill.ReversedStatus, _isBool, "已红冲单据不能操作");
            var valid_IsAudited = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");
            var valid_TerminalId = this.ValidationRule(x => x.Bill.TerminalId, _isZero, "客户未指定");
            var valid_BusinessUserId = this.ValidationRule(x => x.Bill.DeliveryUserId, _isZero, "配送员未指定");
            var valid_WareHouseId = this.ValidationRule(x => x.Bill.WareHouseId, _isZero, "仓库未指定");
            var valid_ProductCount = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "请添加商品项目");
            var valid_SelectesCount = this.ValidationRule(x => x.PaymentMethods.Selectes.Count, _isZero, "请选择支付方式");

            //初始化 
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                var whs = await _wareHousesService.GetWareHousesAsync(this.BillType, force: true);
                if (whs != null && whs.FirstOrDefault() != null)
                {
                    var wh = whs.FirstOrDefault();
                    if (Bill.WareHouseId == 0)
                    {
                        WareHouse = wh;
                        Bill.WareHouseId = wh.Id;
                    }

                    if (string.IsNullOrEmpty(Bill.WareHouseName))
                        Bill.WareHouseName = wh.Name;

                    if (Bill.Id == 0 && Bill.DeliveryUserId == 0)
                    {
                        Bill.DeliveryUserId = Settings.UserId;
                        Bill.DeliveryUserName = Settings.UserRealName;
                    }
                }

                InitPayment();
            }));

            //添加商品
            this.AddProductCommand = ReactiveCommand.CreateFromTask<object>(async e =>
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

               if (!valid_WareHouseId.IsValid || WareHouse == null)
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

               //已提交未审核的可以编辑
               if (this.Bill.IsSubmitBill && !this.Bill.AuditedStatus)
               {
                   _dialogService.ShortAlert("已提交的单据不能编辑");
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
                       //编辑商品无效
                       product.GUID = item.GUID;
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

                       await this.NavigateAsync("EditProductPage", ("Product", product), ("Reference", PageName), ("Item", item), ("WareHouse", WareHouse));
                   }
               }

               this.Selecter = null;
           }).DisposeWith(DeactivateWith);


            //提交单据
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                //await this.Access(AccessGranularityEnum.ReturnBillsSave);
                return await this.Access(AccessGranularityEnum.ReturnBillsSave, async () =>
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

                    if (IsNeedShowPrompt)
                    {
                        var ok = await _dialogService.ShowConfirmAsync("是否保存并提交吗？", "", "确定", "取消");
                        if (!ok)
                        {
                            return Unit.Default;
                        }
                    }
                    IsNeedShowPrompt = true;

                    if (Bill.BusinessUserId == 0)
                        Bill.BusinessUserId = Settings.UserId;

                    var postMData = new ReturnBillUpdateModel()
                    {
                        BillNumber = this.Bill.BillNumber,
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
                        Items = Bill.Items?.Where(i => i.Quantity > 0).ToList(),
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


                    return await SubmitAsync(postMData, Bill.Id, _returnBillService.CreateOrUpdateAsync, async (result) =>
                    {
                        if (IsNeedPrint)
                        {
                            Bill.BillType = BillTypeEnum.SaleBill;
                            await SelectPrint((ReturnBillModel)Bill.Clone());
                        }
                        IsNeedPrint = false;

                        BillId = result.Code;
                        ClearBillCache();
                        PaymentMethods.Selectes.Clear();
                        InitPayment();

                        if (IsVisit)
                        {
                            //转向拜访界面
                            //await this.NavigateAsync("VisitStorePage", ("BillTypeId",BillTypeEnum.ReturnBill),                               ("BillId", BillId),("Amount", Bill.SumAmount));
                            await _navigationService.GoBackAsync(("BillTypeId", BillTypeEnum.ReturnBill), ("BillId", BillId), ("Amount", Bill.SumAmount));
                        }

                    }, token: new System.Threading.CancellationToken());

                });
            },
            this.IsValid());

            #region //存储记录

            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                if (!this.Bill.AuditedStatus && !this.Bill.IsSubmitBill)
                {
                    this.OnArchiveing(true, async () => { await _navigationService.GoBackAsync(); });
                }
                else
                {
                    await _navigationService.GoBackAsync();
                }
            });
            #endregion

            //审核
            this.AuditingDataCommand = ReactiveCommand.CreateFromTask<object>(async _ =>
            {
                //是否具有审核权限
                await this.Access(AccessGranularityEnum.ReturnBillsApproved);
                await SubmitAsync(Bill.Id, _returnBillService.AuditingAsync, async (result) =>
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
              }).DisposeWith(DeactivateWith);
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
            //工具栏打印
            this.PrintCommand = ReactiveCommand.Create(async () =>
            {
                await Print();
            });

            //绑定页面菜单
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                //支付方式
                { MenuEnum.PAY,(m,vm)=> {
                  if (Bill.SumAmount == 0) { this.Alert("请添加商品项目！"); return; }
                         PaymentMethodBaseModel payments = this.PaymentMethods;
                         SelectPaymentMethods(("PaymentMethods", payments),
                                     ("TBalance", this.TBalance),
                                     ("BillType", this.BillType),
                                     ("Reference", PageName));
                } },
                //欠款
                { MenuEnum.ARREARS,(m,vm)=> {

                        SetOweCash((result) =>
                         {
                             decimal.TryParse(result, out decimal oweCash);
                             PaymentMethods.OweCash = oweCash;
                             PaymentMethods.OweCashShowFiled = oweCash > 0;
                             PaymentMethods.OweCashShowFiled = oweCash > 0;
                             UpdateUI();
                         }, Bill.OweCash);
                } },
                //优惠
                { MenuEnum.DISCOUNT,(m,vm)=>{

                SetDiscount((result) =>
                         {
                             decimal.TryParse(result, out decimal preferentialAmount);
                             PaymentMethods.PreferentialAmount = preferentialAmount;
                             PaymentMethods.PreferentialAmountShowFiled = preferentialAmount > 0;
                             PaymentMethods.PreferentialAmountShowFiled = preferentialAmount > 0;
                             UpdateUI();
                         }, Bill.PreferentialAmount);
                } },
                //整单备注
                { MenuEnum.REMARK,(m,vm)=>{

                 AllRemak((result) =>
                     {
                         Bill.Remark = result;
                     }, Bill.Remark);

                } },
                //清空单据
                { MenuEnum.CLEAR,(m,vm)=>{

                    ClearBill<ReturnBillModel, ReturnItemModel>(Bill, DoClear);

                } },
                //打印
                { MenuEnum.PRINT,async (m,vm)=>{
                    await Print();
                } },
                //历史单据
                { MenuEnum.HISTORY,async (m,vm)=>{
                        await SelectHistory();
                } },
                //默认售价
                { MenuEnum.DPRICE,async (m,vm)=>{

                 await SelectDefaultAmountId(Bill.ReturnDefaultAmounts);

                } },
                //审核
                { MenuEnum.SHENGHE,async (m,vm)=>{

                    ResultData result = null;
                         using (UserDialogs.Instance.Loading("审核中..."))
                         {
                             result = await _returnBillService.AuditingAsync(Bill.Id);
                         }
                         if (result!=null&&result.Success)
                         {
                            //红冲审核水印
                            this.EnableOperation = true;
                            this.Bill.AuditedStatus = true;
                            AppendMenus(Bill);
                             await ShowConfirm(true, "审核成功", true);
                         }
                         else
                         {
                             await ShowConfirm(false, $"审核失败！{result?.Message}", false, goReceipt: false);
                         }

                } },
                //红冲
                { MenuEnum.HONGCHOU,async (m,vm)=>{

                    var remark = await CrossDiaglogKit.Current.GetInputTextAsync("红冲备注", "",Keyboard.Text);
                    if (!string.IsNullOrEmpty(remark))
                    {
                       bool result = false;
                        using (UserDialogs.Instance.Loading("红冲中..."))
                        {
                            result = await _returnBillService.ReverseAsync(Bill.Id);
                        }

                        if (result)
                        {
                            //红冲审核水印
                            this.EnableOperation = true;
                            this.Bill.ReversedStatus = true;
                            AppendMenus(Bill);
                            await ShowConfirm(true, "红冲成功", true);
                        }
                        else
                        {
                            await ShowConfirm(false, "红冲失败！", false, goReceipt: false);
                        }
                    }
                } },
                //冲改
                { MenuEnum.CHOUGAI,async (m,vm)=>{
                    await _returnBillService.ReverseAsync(Bill.Id);
                } }
            });

            this.BindBusyCommand(Load);


            this.Load.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.AddProductCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.SubmitDataCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.SaveCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.AuditingDataCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.RecognitionCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.SpeechCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.PrintCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
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
                var ok = await _dialogService.ShowConfirmAsync("请您确认打印并提交销售单？", "", "确定", "取消");
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
                Bill.BillType = BillTypeEnum.ReturnBill;
                await SelectPrint(Bill);
            }
        }
        private void ClearBillCache()
        {
            try
            {
                InitBill();
                Settings.ReturnBill = Bill;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }




        public  void InitPayment()
        {
            try
            {
                PaymentMethods.PreferentialAmount = 0;
    
                bool firstOrDefault = false;
                _returnBillService.Rx_GetInitDataAsync(new System.Threading.CancellationToken())
                   .Subscribe((results) =>
                   {
                       try
                       {
                           lock (_lock)
                           {
                               //确保只取缓存数据
                               if (!firstOrDefault)
                               {
                                   firstOrDefault = true;
                                   if (results != null && results?.Code >= 0)
                                   {
                                       var result = results?.Data;
                                       if (result != null)
                                       {
                                           var defaultAccs = result
                                           .ReturnBillAccountings
                                           .Select(s => new AccountingModel()
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
                                   }
                               }
                           }
                       }
                       catch (Exception ex)
                       {
                           Crashes.TrackError(ex);
                       }
                   }).DisposeWith(DeactivateWith);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        /// <summary>
        /// 存档
        /// </summary>
        /// <param name="showTips"></param>
        /// <param name="call"></param>
        public async override void OnArchiveing(bool showTips = false, Action call = null)
        {
            //已经提交则不存档
            if (this.Bill.IsSubmitBill)
                return;

            Action save = () =>
            {
                this.Bill.PaymentMethods = PaymentMethods;
                Settings.ReturnBill = this.Bill;
                _dialogService.ShortAlert("已存档");
            };

            var c1 = this.Bill.TerminalId != 0 && this.Bill.TerminalId != (Settings.SaleReservationBill?.TerminalId ?? 0);
            var c2 = this.Bill.DeliveryUserId != 0 && this.Bill.DeliveryUserId != (Settings.SaleReservationBill?.DeliveryUserId ?? 0);
            var c3 = this.Bill.WareHouseId != 0 && this.Bill.WareHouseId != (Settings.SaleReservationBill?.WareHouseId ?? 0);
            var c4 = this.Bill.Items?.Count != 0;
            if (c1 || c2 || c3 || c4)
            {
                if (!this.Bill.AuditedStatus)
                {
                    if (showTips)
                    {
                        var ok = await _dialogService.ShowConfirmAsync("你是否要保存单据？", "提示", "确定", "取消");
                        if (ok)
                        {
                            save.Invoke();
                        }
                        else
                        {
                            ClearBillCache();
                            call?.Invoke();
                        }
                    }
                    else
                    {
                        save.Invoke();
                    }
                }
            }
            else 
            {
                call?.Invoke();
            }
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                string fromPage = "";
                if (parameters.ContainsKey("Reference"))
                {
                    parameters.TryGetValue("Reference", out fromPage);
                }
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
                        var bigProduct = Bill.Items?.Where(b => b.ProductId == p.Id && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items?.Where(b => b.ProductId == p.Id && b.SmallUnitId == p.UnitId).FirstOrDefault();
                        if (bigProduct != null)
                        {
                            Bill.Items?.Remove(bigProduct);
                        }

                        if (smalProduct != null)
                        {
                            Bill.Items?.Remove(smalProduct);
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
                        var bigProduct = Bill.Items?.Where(b => b.GUID == p.GUID && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items?.Where(b => b.GUID == p.GUID && b.SmallUnitId == p.UnitId).FirstOrDefault();
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


                        if (Bill.Items?.Any() ?? false)
                        {
                            foreach (var ip in Bill.Items)
                            {
                                if (smalProduct != null && bigProduct.GUID.Equals(ip.GUID) && (ip.UnitId == smalProduct.SmallUnitId || ip.SmallUnitId == smalProduct.SmallUnitId))
                                {
                                    ip.UnitName = smalProduct.UnitName;
                                    ip.Quantity = smalProduct.Quantity;
                                    ip.Remark = smalProduct.Remark;
                                    ip.SmallUnitId = smalProduct.SmallUnitId;
                                    ip.Subtotal = smalProduct.Subtotal;
                                }
                                else if (bigProduct != null && bigProduct.GUID.Equals(ip.GUID) && (ip.UnitId == bigProduct.SmallUnitId || ip.BigUnitId == bigProduct.BigUnitId))
                                {
                                    ip.UnitName = bigProduct.UnitName;
                                    ip.Quantity = bigProduct.Quantity;
                                    ip.Remark = bigProduct.Remark;
                                    ip.BigUnitId = bigProduct.UnitId;
                                    ip.Subtotal = bigProduct.Subtotal;
                                }
                            }
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

                    if (productSeries != null && productSeries.Count > 0)
                    {

                        List<ReturnItemModel> bigProduct = new List<ReturnItemModel>();
                        List<ReturnItemModel> smallProduct = new List<ReturnItemModel>();
                        List<ReturnItemModel> giveBigProduct = new List<ReturnItemModel>();
                        List<ReturnItemModel> giveSmallProduct = new List<ReturnItemModel>();


                        foreach (var p in productSeries)
                        {
                            //赠送商品
                            if (p.GiveProduct.BigUnitQuantity != 0)
                            {
                                var bigGive = new ReturnItemModel()
                                {
                                    Id = 0,
                                    //GUID = p.GUID,
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
                                    IsGifts = true,
                                    //促销活动
                                    CampaignId = p.CampaignId,
                                    CampaignName = p.CampaignName,
                                };
                                giveBigProduct.Add(bigGive);
                            }
                            if (p.GiveProduct.SmallUnitQuantity != 0)
                            {
                                var smallGive = new ReturnItemModel()
                                {
                                    Id = 0,
                                    //GUID = p.GUID,
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
                                    IsGifts = true,
                                    //促销活动
                                    CampaignId = p.CampaignId,
                                    CampaignName = p.CampaignName,
                                };
                                giveSmallProduct.Add(smallGive);
                            }

                            var bigItem = new ReturnItemModel()
                            {
                                Id = 0,
                                //GUID = p.GUID,
                                UnitId = p.BigPriceUnit.UnitId,
                                ProductId = p.Id,
                                ProductName = p.ProductName,
                                StoreId = Settings.StoreId,
                                UnitName = p.bigOption.Name,
                                Quantity = p.BigPriceUnit.Quantity,
                                Price = p.BigPriceUnit.Price,
                                Amount = (p.BigPriceUnit.Quantity) * (p.BigPriceUnit.Price),
                                Remark = p.BigPriceUnit.Remark,
                                BigUnitId = p.BigPriceUnit.UnitId,
                                SmallUnitId = 0,
                                IsGifts = p.BigPriceUnit.Quantity > 0 && p.BigPriceUnit.Price == 0,
                                Subtotal = (p.BigPriceUnit.Price) * p.BigPriceUnit.Quantity,

                                //促销活动
                                CampaignId = p.CampaignId,
                                CampaignName = p.CampaignName,
                                CampaignBuyProductId = p.TypeId == 1 ? p.Id : 0,
                                CampaignGiveProductId = p.TypeId == 2 ? p.Id : 0

                            };
                            var smallItem = new ReturnItemModel()
                            {
                                Id = 0,
                                //GUID = p.GUID,
                                UnitId = p.SmallPriceUnit.UnitId,
                                ProductId = p.Id,
                                ProductName = p.ProductName,
                                StoreId = Settings.StoreId,
                                UnitName = p.smallOption.Name,
                                Quantity = p.SmallPriceUnit.Quantity,
                                Price = p.SmallPriceUnit.Price,
                                Amount = (p.SmallPriceUnit.Quantity) * (p.SmallPriceUnit.Price),
                                Remark = p.SmallPriceUnit.Remark,
                                SmallUnitId = p.SmallPriceUnit.UnitId,
                                BigUnitId = 0,
                                IsGifts = p.SmallPriceUnit.Quantity > 0 && p.SmallPriceUnit.Price == 0,
                                Subtotal = (p.SmallPriceUnit.Price) * p.SmallPriceUnit.Quantity,

                                //促销活动
                                CampaignId = p.CampaignId,
                                CampaignName = p.CampaignName,
                                CampaignBuyProductId = p.TypeId == 1 ? p.Id : 0,
                                CampaignGiveProductId = p.TypeId == 2 ? p.Id : 0
                            };

                            if (bigItem.Quantity > 0)
                            {
                                if (bigItem.IsGifts && string.IsNullOrEmpty(bigItem.Remark))
                                    bigItem.Remark = "赠品(零元开单)";
                                if (bigItem.IsGifts)
                                {
                                    giveBigProduct.Add(bigItem);
                                }
                                else
                                {
                                    bigProduct.Add(bigItem);
                                }
                            }

                            if (smallItem.Quantity > 0)
                            {
                                if (smallItem.IsGifts && string.IsNullOrEmpty(smallItem.Remark))
                                    smallItem.Remark = "赠品(零元开单)";

                                if (smallItem.IsGifts)
                                {
                                    giveSmallProduct.Add(smallItem);
                                }
                                else
                                {
                                    smallProduct.Add(smallItem);
                                }
                            }
                        }
                        ProductSeries = new ObservableCollection<ProductModel>(productSeries);


                        List<ReturnItemModel> temp = new List<ReturnItemModel>();
                        foreach (var item in bigProduct)
                        {
                            if (item.IsGifts)
                            {
                                giveBigProduct.Add(item);
                                continue;
                            }
                            BillAddItem(item);
                            if (null != item.CampaignId && (int)item.CampaignId > 0)
                            {
                                temp = bigProduct.Where(p => p.CampaignId == item.CampaignId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                    }
                                }

                                temp = smallProduct.Where(p => p.CampaignId == item.CampaignId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                        smallProduct.Remove(tempItem);
                                    }
                                }

                                temp = giveBigProduct.Where(p => p.CampaignId == item.CampaignId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                        giveBigProduct.Remove(tempItem);
                                    }
                                }

                                temp = giveSmallProduct.Where(p => p.CampaignId == item.CampaignId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                        giveSmallProduct.Remove(tempItem);
                                    }
                                }
                            }
                            else
                            {
                                temp = smallProduct.Where(p => p.ProductId == item.ProductId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                        smallProduct.Remove(tempItem);
                                    }
                                }


                                temp = giveBigProduct.Where(p => p.ProductId == item.ProductId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                        giveBigProduct.Remove(tempItem);
                                    }
                                }

                                temp = giveSmallProduct.Where(p => p.ProductId == item.ProductId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                        giveSmallProduct.Remove(tempItem);
                                    }
                                }
                            }
                        }
                        foreach (var item in smallProduct)
                        {
                            if (item.IsGifts)
                            {
                                giveSmallProduct.Add(item);
                                continue;
                            }
                            BillAddItem(item);
                            if (null != item.CampaignId && (int)item.CampaignId > 0)
                            {
                                temp = smallProduct.Where(p => p.CampaignId == item.CampaignId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                    }
                                }


                                temp = giveBigProduct.Where(p => p.CampaignId == item.CampaignId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                        giveBigProduct.Remove(tempItem);
                                    }
                                }

                                temp = giveSmallProduct.Where(p => p.CampaignId == item.CampaignId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                        giveSmallProduct.Remove(tempItem);
                                    }
                                }
                            }
                            else
                            {
                                temp = giveBigProduct.Where(p => p.ProductId == item.ProductId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                        giveBigProduct.Remove(tempItem);
                                    }
                                }

                                temp = giveSmallProduct.Where(p => p.ProductId == item.ProductId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                        giveSmallProduct.Remove(tempItem);
                                    }
                                }
                            }
                        }


                        foreach (var item in giveBigProduct)
                        {
                            BillAddItem(item);
                            if (null != item.CampaignId && (int)item.CampaignId > 0)
                            {
                                temp = giveBigProduct.Where(p => p.CampaignId == item.CampaignId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                    }
                                }
                                temp = giveSmallProduct.Where(p => p.CampaignId == item.CampaignId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                        giveSmallProduct.Remove(tempItem);
                                    }
                                }
                            }
                            else
                            {
                                temp = giveBigProduct.Where(p => p.ProductId == item.ProductId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                    }
                                }

                                temp = giveSmallProduct.Where(p => p.ProductId == item.ProductId).ToList();
                                if (null != temp && temp.Any())
                                {
                                    foreach (var tempItem in temp)
                                    {
                                        BillAddItem(tempItem);
                                        giveSmallProduct.Remove(tempItem);
                                    }
                                }
                            }
                        }


                        foreach (var item in giveSmallProduct)
                        {
                            BillAddItem(item);
                        }
                    }
                    UpdateUI();
                }

                //拜访时开单
                if (parameters.ContainsKey("TerminalId"))
                {
                    parameters.TryGetValue("TerminalId", out int terminalId);
                    parameters.TryGetValue("TerminalName", out string name);
                    Bill.TerminalId = terminalId;
                    Bill.TerminalName = name;
                    IsVisit = true;
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


                //加载缓存
                PaymentMethodBaseModel paymentMethodCache = null;
                if (dispatchItem == null && bill == null && ("HomePage".Equals(fromPage) || "AllfunPage".Equals(fromPage) || "VisitStorePage".Equals(fromPage)))
                {
                    IsLocalBill = true;
                    var cacheBill = Settings.ReturnBill;
                    if (null != cacheBill)
                    {
                        bill = cacheBill;
                        paymentMethodCache = cacheBill.PaymentMethods;
                    }
                    //拜访页面开单，如果与缓存终端不一致取消缓存数据
                    if ("VisitStorePage".Equals(fromPage) && bill?.TerminalId != Bill.TerminalId)
                    {
                        bill = null;
                        paymentMethodCache = null;
                    }
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

                    if (order.Items != null && order.Items.Any())
                    {
                        //商品项目
                        foreach (var item in order?.Items)
                        {
                            bill.Items?.Add(new ReturnItemModel()
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
                    }

                    //转换支付方式
                    if (order.ReturnReservationBillAccountings != null && order.ReturnReservationBillAccountings.Any())
                    {
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

                    UpdateUI();
                }

                if (paymentMethodCache != null)
                {
                    this.PaymentMethods = paymentMethodCache;
                    UpdateUI();
                }

                //已提交
                if (parameters.ContainsKey("IsSubmitBill"))
                {
                    parameters.TryGetValue("IsSubmitBill", out bool isSubmitBill);
                    this.Bill.IsSubmitBill = isSubmitBill;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                this.OnArchiveing();
            }
        }

        private void BillAddItem(ReturnItemModel item)
        {
            if (null == Bill.Items?.FirstOrDefault(p => p.GUID == item.GUID))
            {
                item.SortIndex = Bill.Items?.Count ?? 0;
                Bill.Items?.Add(item);
            }
        }
        private void InitBill(bool clear = false)
        {
            this.BillType = BillTypeEnum.ReturnBill;
            this.Bill = new ReturnBillModel()
            {
                BusinessUserId = Settings.UserId,
                CreatedOnUtc = DateTime.Now,
                BillNumber = CommonHelper.GetBillNumber(CommonHelper.GetEnumDescription(BillType).Split(',')[1], Settings.StoreId)
            };

            //默认
            if (this.Bill.Id == 0)
            {
                this.Bill.BusinessUserId = Settings.UserId;
                this.Bill.BusinessUserName = Settings.UserRealName;

                var setting = Settings.ReturnBill;
                if (setting != null)
                {
                    this.Bill.WareHouseId = setting.WareHouseId;
                    this.Bill.WareHouseName = setting.WareHouseName;
                    this.WareHouse.Id = setting.WareHouseId;
                    this.WareHouse.Name = setting.WareHouseName;
                }
            }
        }
        private void DoClear()
        {
            InitBill(true);
            this.TBalance = null;
        }
        public void UpdateUI()
        {
            try
            {
                //合计
                this.Bill.SumAmount = decimal.Round(Bill.Items?.Select(p => p.Subtotal).Sum() ?? 0, 2);

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
            if (Bill.Id > 0)
            {
                AppendMenus(Bill);
            }
            else
            {
                //控制显示菜单
                _popupMenu?.Show(0, 1, 2, 3, 4, 5, 6, 7);
            }

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
