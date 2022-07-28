using Acr.UserDialogs;
using DCMS.ChartJS.Models;
using DCMS.Client.Abstractions;
using DCMS.Client.CustomViews;
using DCMS.Client.Enums;
using DCMS.Client.Models;
using DCMS.Client.Models.Campaigns;
using DCMS.Client.Models.Census;
using DCMS.Client.Models.Configuration;
using DCMS.Client.Models.Finances;
using DCMS.Client.Models.Media;
using DCMS.Client.Models.Products;
using DCMS.Client.Models.Sales;
using DCMS.Client.Models.Settings;
using DCMS.Client.Models.Terminals;
using DCMS.Client.Models.Users;
using DCMS.Client.Models.Visit;
using DCMS.Client.Models.WareHouses;
using DCMS.Client.Pages;
using DCMS.Client.Services;
using DCMS.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism;
using Prism.AppModel;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using Rg.Plugins.Popup.Services;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using PLU = Plugin.Media.Abstractions;

namespace DCMS.Client.ViewModels
{
    /// <summary>
    /// VM 标准基类  
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class ViewModelBase : ReactiveObject,
        IInitialize,
        IInitializeAsync,
        IPageLifecycleAware,
        IApplicationLifecycleAware,
        INavigationAware,
        INavigatableViewModel,
        IDestructible,
        IConfirmNavigationAsync,
        IActiveAware,
        IActivatableViewModel,
        IValidatableViewModel
    {

        protected ILogger<ViewModelBase> _logger;
        protected INavigationService _navigationService;
        protected IDialogService _dialogService;
        protected CancellationTokenSource cts;

        protected bool isInCall = false;
        protected object syncLock = new object();
        protected bool loaded = false;
        protected bool IsView = false;
        protected DateTime _lastExecution = DateTime.MinValue;
        public event EventHandler IsActiveChanged;

        private System.Threading.Timer _timer;

        /// <summary>
        /// 初始化管理reactive验证的验证上下文
        /// </summary>
        public ValidationContext ValidationContext { get; } = new ValidationContext();
        protected readonly Func<string, bool> _isDefined = value => !string.IsNullOrEmpty(value);
        protected readonly Func<int, bool> _isZero = value => value > 0;
        protected readonly Func<bool, bool> _isBool = value => !value;
        protected readonly Func<decimal?, bool> _isDZero = value => value > 0;

        /// <summary>
        /// 判断网络状态
        /// </summary>
        /// <returns></returns>
        public static bool InternetConnectivity()
        {
            var connectivity = Connectivity.NetworkAccess;
            if (connectivity == NetworkAccess.Internet)
                return true;

            return false;
        }


        #region Prevents

        protected readonly TimeSpan minTapInterval = new TimeSpan(0, 0, 2);
        protected DateTime lastTapTimestamp;

        /// <summary>
        /// 防止对inFunction的多次快速调用（异步版本）
        /// </summary>
        /// <param name="inFunction"></param>
        protected async void RapidTapPreventorAsync(Func<Task<Page>> inFunction)
        {
            lock (syncLock)
            {
                if (isInCall)
                    return;
                isInCall = true;
            }

            try
            {
                await inFunction();
            }
            finally
            {
                lock (syncLock)
                {
                    isInCall = false;
                }
            }
        }
        protected async Task<Unit> RapidTapPreventorAsync(Func<Task<Unit>> inFunction)
        {
            lock (syncLock)
            {
                if (isInCall)
                    return Unit.Default;
                isInCall = true;
            }
            try
            {
                return await inFunction();
            }
            finally
            {
                lock (syncLock)
                {
                    isInCall = false;
                }
            }
        }
        protected async void RapidTapPreventorAsync(Func<Task> inFunction)
        {
            lock (syncLock)
            {
                if (isInCall)
                    return;
                isInCall = true;
            }

            try
            {
                await inFunction();
            }
            finally
            {
                lock (syncLock)
                {
                    isInCall = false;
                }
            }
        }
        protected void RapidTapPreventor(Action inFunction)
        {
            lock (syncLock)
            {
                if (isInCall)
                    return;

                isInCall = true;
            }
            try
            {
                inFunction();
            }
            finally
            {
                lock (syncLock)
                {
                    isInCall = false;
                }
            }
        }

        #endregion

        #region Field


        /// <summary>
        /// 防止内存泄漏，在某些情况下没有及时取消订阅导致内存泄漏。CompositeDisposable可以将Disposable统一管理
        /// CompositeDisposable 是一个一次性的东西，如果调用了dispose 方法，
        /// 那么之后加进来的 disposable 会自动dispose，所以不要试图在dispose 之后希望它还能重用
        /// </summary>
        private CompositeDisposable deactivateWith;
        protected CompositeDisposable DeactivateWith => this.deactivateWith ??= new CompositeDisposable();

        protected IDisposable subProductCatagoryBus;
        protected IDisposable subGPSBus;
        protected IDisposable subMessage;
        protected IDisposable subUpdateUI;

        public ViewModelActivator Activator { get; }

        [Reactive] public bool IsTouched { get; set; }
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnActiveTabChangedAsync(); }
        }
        public string MenuBusKey { get; set; }

        public int PageSize { get; set; } = 20;
        public int TotalPageSize { get; set; } = 0;
        public int PageCounter { get; set; } = 0;

        [Reactive] public string CurrentAppVersion { get; set; }
        [Reactive] public string StoreName { get; set; }
        [Reactive] public string UserName { get; set; }
        [Reactive] public string DefaultRole { get; set; } = Settings.DefaultRole;
        [Reactive] public int PayTypeId { get; set; }
        [Reactive] public string PayTypeName { get; set; } = "现结";
        [Reactive] public string Title { get; set; }
        [Reactive] public bool IsRefreshing { get; set; }
        [Reactive] public bool IsNull { get; set; }
        [Reactive] public DateTime LastUpdateTime { get; set; } = Settings.LastUpdateTime;

        [Reactive] public bool IsBusy { get; set; } = false;
        [Reactive] public bool Is1Busy { get; set; } = false;
        [Reactive] public bool Is2Busy { get; set; } = false;
        [Reactive] public bool Is3Busy { get; set; } = false;
        [Reactive] public bool Is4Busy { get; set; } = false;
        [Reactive] public bool Is5Busy { get; set; } = false;
        [Reactive] public bool Is6Busy { get; set; } = false;

        [Reactive] public bool ForceRefresh { get; set; } = false;
        [Reactive] public bool ShowSubmitBtn { get; set; } = true;
        [Reactive] public bool ShowPrintBtn { get; set; } = true;
        [Reactive] public bool EnabledSubmitBtn { get; set; } = true;
        [Reactive] public string SubmitText { get; set; } = "保存";
        [Reactive] public int ItemTreshold { get; set; }
        [Reactive] public string ReferencePage { get; set; }
        [Reactive] public MessageInfo SelecterMessage { get; set; }
        [Reactive] public int BillId { get; set; }
        [Reactive] public bool Edit { get; set; }


        //语音模块
        [Reactive] public double ExpandedPercentage { get; set; }
        [Reactive] public bool IsVisible { get; set; } = false;
        [Reactive] public bool IsExpanded { get; set; }
        [Reactive] public bool IsFooterVisible { get; set; } = true;

        [Reactive] public string RecognitionText { get; set; } = "请说一句话，如：雪花啤酒！";
        [Reactive] public bool EnableMicrophone { get; set; } = true;
        public IReactiveCommand RecognitionCommand { get; set; }
        [Reactive] public int Counter { get; set; }
        [Reactive] public string IconCounter { get; set; } = "microphone";
        [Reactive] public bool ShowAddProduct { get; set; } = true;
        [Reactive] public bool EnableOperation { get; set; } = true;
        [Reactive] public bool EnableRefused { get; set; } = false;
        [Reactive] public bool IsChatNull { get; set; }
        [Reactive] public bool IsChat2Null { get; set; }

        #endregion

        #region Common Model
        [Reactive] public bool ShowTBalance { get; set; } = false;
        [Reactive] public bool ShowMBalance { get; set; } = false;
        [Reactive] public TerminalBalance TBalance { get; set; } = new TerminalBalance();
        [Reactive] public ManufacturerBalance MBalance { get; set; } = new ManufacturerBalance();

        [Reactive] public CategoryModel Category { get; set; } = new CategoryModel();
        [Reactive] public TerminalModel Terminal { get; set; } = new TerminalModel();
        [Reactive] public WareHouseModel WareHouse { get; set; } = new WareHouseModel();
        [Reactive] public UserModel BusinessUser { get; set; } = new UserModel();
        [Reactive] public BrandModel Brand { get; set; } = new BrandModel();
        [Reactive] public RemarkConfig RemarkConfig { get; set; } = new RemarkConfig();
        [Reactive] public ChannelModel Channel { get; set; } = new ChannelModel();
        [Reactive] public DistrictModel District { get; set; } = new DistrictModel();
        [Reactive] public LineTierModel LineTier { get; set; } = new LineTierModel();
        [Reactive] public RankModel Rank { get; internal set; } = new RankModel();
        [Reactive] public ProductModel Product { get; set; } = new ProductModel();
        [Reactive] public PaymentMethodBaseModel PaymentMethods { get; set; } = new PaymentMethodBaseModel();
        [Reactive] public SpecificationAttributeOptionModel Specification { get; internal set; } = new SpecificationAttributeOptionModel();
        [Reactive] public FinanceAccountingOptionSetting AccountingOptionSetting { get; set; } = new FinanceAccountingOptionSetting();
        [Reactive] public BillTypeEnum BillType { get; set; }
        [Reactive] public AccountingModel Accounting { get; set; } = new AccountingModel();
        [Reactive] public FilterModel Filter { get; set; } = new FilterModel();
        [Reactive] public VisitStore OutVisitStore { get; set; }
        [Reactive] public AccountingOption AccountingOption { get; set; } = new AccountingOption();

        #endregion

        #region ObservableCollection

        [Reactive] public ObservableCollection<TrackingModel> GpsEvents { get; set; } = new ObservableCollection<TrackingModel>();
        [Reactive] public ObservableCollection<WareHouseModel> WareHouses { get; set; } = new ObservableCollection<WareHouseModel>();
        [Reactive] public ObservableCollection<BusinessVisitList> BusinessUsers { get; set; } = new ObservableCollection<BusinessVisitList>();
        [Reactive] public ObservableCollection<ManufacturerModel> Manufacturers { get; set; } = new ObservableCollection<ManufacturerModel>();
        [Reactive] public AsyncObservableCollection<TerminalModel> Terminals { get; set; } = new AsyncObservableCollection<TerminalModel>();
        [Reactive] public ObservableCollection<ProductModel> TempProductSeries { get; set; } = new ObservableCollection<ProductModel>();
        [Reactive] public ObservableCollection<ProductModel> ProductSeries { get; set; } = new ObservableCollection<ProductModel>();
        [Reactive] public ObservableCollection<SubMenu> BindMenus { get; set; } = new ObservableCollection<SubMenu>();
        [Reactive] public ObservableCollection<CategoryModel> BindCategories { get; set; } = new ObservableCollection<CategoryModel>();
        [Reactive] public ObservableCollection<DistrictModel> Districts { get; set; } = new ObservableCollection<DistrictModel>();


        public void SetMenus(Action<MenuEnum> action = null, params int[] menus)
        {
            BindMenus = new ObservableCollection<SubMenu>(GlobalSettings.ToolBarMenus?.Where(s => menus?.Contains(s.Id) ?? false));
            var key = MenuBusKey.ToUpper();
            MessageBus.Current
                          .Listen<MenuEnum>(key)
                          .Subscribe(x =>
                          {
                              action?.Invoke(x);
                          });
        }

        public void SubscribeMenus(Action<MenuEnum> action = null, string key = "")
        {
            MessageBus
                .Current
                .Listen<MenuEnum>(key.ToUpper())
                .Subscribe(x =>
                {
                    action?.Invoke(x);
                });
        }

        /// <summary>
        /// 筛选日期
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="call"></param>
        public void BindFilterDateMenus(bool reLoad = false)
        {
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                //TODAY
                { MenuEnum.TODAY, (m,vm) => {
                    Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                        Filter.EndTime = DateTime.Now;
                    if(reLoad)
                         ((ICommand)Load)?.Execute(null);
                } },
                //YESTDAY
                { MenuEnum.YESTDAY, (m,vm) => {
                     Filter.StartTime = DateTime.Now.AddDays(-1);
                        Filter.EndTime = DateTime.Now;
                     if(reLoad)
                         ((ICommand)Load)?.Execute(null);
                } },
                //MONTH
                { MenuEnum.MONTH, (m,vm) => {
                      Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01 00:00:00"));
                        Filter.EndTime = DateTime.Now;
                     if(reLoad)
                         ((ICommand)Load)?.Execute(null);
                } },
                //THISWEEBK
                { MenuEnum.THISWEEBK, (m,vm) => {
                      Filter.StartTime = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                        Filter.EndTime = DateTime.Now;
                     if(reLoad)
                         ((ICommand)Load)?.Execute(null);
                } },
                //OTHER
                { MenuEnum.OTHER, (m,vm) => {
                     SelectDateRang(reLoad);
                } }
            });
        }

        [Reactive] public IList<AccountingModel> Accounts { get; set; } = new ObservableCollection<AccountingModel>();

        #endregion

        #region virtual

        public PopupMenu _popupMenu { get; set; }
        public virtual void OnNavigatedFrom(INavigationParameters parameters) { } //=> this.Deactivate();
        public virtual void Initialize(INavigationParameters parameters) { }
        public virtual Task InitializeAsync(INavigationParameters parameters) => Task.CompletedTask;
        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (parameters.ContainsKey("Edit"))
                {
                    parameters.TryGetValue("Edit", out bool edit);
                    this.Edit = edit;
                }

                //获取单据
                if (parameters.ContainsKey("BillId"))
                {
                    parameters.TryGetValue("BillId", out int billId);
                    this.BillId = billId;
                }

                //BillType
                if (parameters.ContainsKey("BillType"))
                {
                    parameters.TryGetValue("BillType", out BillTypeEnum billType);
                    this.BillType = billType;
                }

                //Title
                if (parameters.ContainsKey("Title"))
                {
                    parameters.TryGetValue("Title", out string title);
                    if (!string.IsNullOrEmpty(title))
                        this.Title = title;
                }

                //Message
                if (parameters.ContainsKey("Message"))
                {
                    parameters.TryGetValue("Message", out MessageInfo message);
                    if (message != null)
                        this.SelecterMessage = message;
                }

                //过滤器
                if (parameters.ContainsKey("Filter"))
                {
                    parameters.TryGetValue("Filter", out FilterModel filter);
                    if (filter != null)
                        this.Filter = filter;
                }

                //终端客户
                if (parameters.ContainsKey("Terminaler"))
                {
                    parameters.TryGetValue("Terminaler", out TerminalModel terminaler);
                    if (terminaler != null)
                    {
                        this.Terminal = terminaler;
                        this.Filter.TerminalId = terminaler.Id;
                        this.Filter.TerminalName = terminaler.Name;
                    }
                }

                //TerminalId
                if (parameters.ContainsKey("TerminalId"))
                {
                    parameters.TryGetValue("TerminalId", out int terminalId);
                    this.Terminal.Id = terminalId;
                    this.Filter.TerminalId = terminalId;
                }

                //片区
                if (parameters.ContainsKey("District"))
                {
                    parameters.TryGetValue("District", out DistrictModel area);
                    if (area != null)
                    {
                        this.District = area;
                        this.Filter.DistrictName = District.Name;
                        this.Filter.DistrictId = District.Id;
                    }
                }
                //ClearDistrict
                if (parameters.ContainsKey("ClearDistrict"))
                {
                    this.Filter.DistrictName = "";
                    this.Filter.DistrictId = 0;
                }

                //仓库
                if (parameters.ContainsKey("WareHouse"))
                {
                    parameters.TryGetValue("WareHouse", out WareHouseModel wareHouse);
                    if (wareHouse != null)
                    {
                        this.WareHouse = wareHouse;
                        this.Filter.WareHouseId = wareHouse.Id;
                        this.Filter.WareHouseName = wareHouse.Name;
                    }
                }

                if (parameters.ContainsKey("WareHouseId"))
                {
                    parameters.TryGetValue<int>("WareHouseId", out int wareHouseId);
                    this.WareHouse.Id = wareHouseId;
                    this.Filter.WareHouseId = wareHouseId;
                }

                //Products
                if (parameters.ContainsKey("Products"))
                {
                    parameters.TryGetValue("Products", out List<ProductModel> products);
                    if (products != null && products.Count() > 0)
                    {
                        var product = products.FirstOrDefault();
                        if (product != null)
                        {
                            this.Product = product;
                            this.Filter.ProductName = product != null ? product.Name : "";
                            this.Filter.ProductId = product != null ? product.Id : 0;
                            this.ProductSeries = new ObservableCollection<ProductModel>(products);
                        }
                    }
                }

                //Accounts 
                if (parameters.ContainsKey("Accounts"))
                {
                    parameters.TryGetValue("Accounts", out List<AccountingModel> accounts);
                    if (accounts != null)
                    {
                        accounts.ForEach(a =>
                        {
                            a.BalanceName = (int)AccountingCodeEnum.AdvancePayment == a.AccountCodeTypeId ? $"余:{a.Balance:#.00}" : "";
                        });
                        this.Accounts = new ObservableCollection<AccountingModel>(accounts.ToList());
                    }
                }

                //科目单项
                if (parameters.ContainsKey("Accounting"))
                {
                    parameters.TryGetValue("Accounting", out AccountingModel accounting);
                    if (accounting != null)
                        this.Accounting = accounting;
                }


                //搜索关键字
                if (parameters.ContainsKey("SerchKey"))
                {
                    parameters.TryGetValue("SerchKey", out string serchKey);
                    if (!string.IsNullOrEmpty(serchKey))
                        this.Filter.SerchKey = serchKey;
                }

                //引用页
                if (parameters.ContainsKey("Reference"))
                {
                    parameters.TryGetValue("Reference", out string reference);
                    if (!string.IsNullOrEmpty(reference))
                        this.ReferencePage = reference;
                }
            }
            catch (Exception ex)
            {
                Alert($"获取参数错误:{ex.Message} {ex.StackTrace}");
            }
           
        }
        public virtual void OnAppearing()
        {
            try
            {
                cts = new CancellationTokenSource();
                once = false;
                CheckAccessExpired();
            }
            catch (Exception) { }
        }
        public virtual void OnDisappearing()
        {
            try
            {
                _timer?.Dispose();
                subProductCatagoryBus?.Dispose();
                subGPSBus?.Dispose();
                subMessage?.Dispose();
                subUpdateUI?.Dispose();
                loaded = false;

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            }
            catch (Exception) { }
            finally
            {
                if (cts != null)
                    cts.Cancel();
            }
        }
        public virtual void Destroy()
        {
            try
            {
                if (!deactivateWith?.IsDisposed ?? true)
                {
                    this.deactivateWith?.Dispose();
                    this.deactivateWith = null;
                    System.Diagnostics.Debug.Print($"{this.PageName}-------------------------------> dw is disposed");
                }
            }
            catch (Exception) { }
            finally
            {
                if (cts != null)
                    cts.Dispose();
            }
        }
        public virtual Task<bool> CanNavigateAsync(INavigationParameters parameters) => Task.FromResult(true);
        public virtual void OnActiveTabChangedAsync() { IsActiveChanged?.Invoke(this, EventArgs.Empty); }
        public virtual void OnResume() { }
        public virtual void OnSleep() { }
        public virtual void OnArchiveing(bool showTips = false, Action call = null) { }

        /// <summary>
        /// 返回回退软操作时，执行SaveCommand 保存命令
        /// </summary>
        public async virtual void OnSoftBackButtonPressed(bool noSave = false)
        {
            if (!noSave)
            {
                var cmd = (ICommand)SaveCommand;
                if (cmd != null)
                    ((ICommand)SaveCommand)?.Execute(null);
                else
                    await _navigationService.GoBackAsync();
            }
            else
                await _navigationService.GoBackAsync();
        }

        #endregion

        #region Command
        public ReactiveCommand<string, Unit> NavigateCommand => ReactiveCommand.CreateFromTask<string>(async (r) => await this.NavigateAsync(r));
        public IReactiveCommand ItemTresholdReachedCommand { get; set; }
        public IReactiveCommand OpenMapNavigation { get; set; }
        public IReactiveCommand CallPhone { get; set; }
        public IReactiveCommand Load { get; set; }
        public IReactiveCommand EditCommand { get; set; }
        public IReactiveCommand SubmitDataCommand { get; set; }
        public IReactiveCommand SaveCommand { get; set; }
        public IReactiveCommand AddCommand { get; set; }
        public IReactiveCommand ItemSelectedCommand { get; set; }
        public IReactiveCommand SwichCommand { get; set; }
        public ReactiveCommand<string, Unit> SerchCommand { get; set; }
        public IReactiveCommand RefreshCommand { get; set; }
        public IReactiveCommand ShowWaitCommand { get; set; }
        public IReactiveCommand SpeechCommand { get; set; }
        public IReactiveCommand GoBackAsync { get; set; }
        public ReactiveCommand<object, Unit> HistoryCommand { get; set; }
        public IReactiveCommand PrintCommand { get; set; }
        public IReactiveCommand AuditingDataCommand { get; set; }
        public ReactiveCommand<object, Unit> DeliverSelected { get; set; }
        public ReactiveCommand<object, Unit> DistrictSelected { get; set; }
        public ReactiveCommand<object, Unit> BrandSelected { get; set; }
        public ReactiveCommand<object, Unit> CustomSelected { get; set; }
        public ReactiveCommand<object, Unit> ManufacturerSelected { get; set; }
        public ReactiveCommand<object, Unit> RemarkSelected { get; set; }



        protected void BindBusyCommand(ICommand command) => this.BindBusyCommand((IReactiveCommand)command);
        protected void BindBusy1Command(ICommand command) => this.BindBusy1Command((IReactiveCommand)command);
        protected void BindBusy2Command(ICommand command) => this.BindBusy2Command((IReactiveCommand)command);
        protected void BindBusy3Command(ICommand command) => this.BindBusy3Command((IReactiveCommand)command);
        protected void BindBusy4Command(ICommand command) => this.BindBusy4Command((IReactiveCommand)command);
        protected void BindBusy5Command(ICommand command) => this.BindBusy5Command((IReactiveCommand)command);
        protected void BindBusy6Command(ICommand command) => this.BindBusy6Command((ReactiveCommand<object, Unit>)command);


        protected void BindBusyCommand(IReactiveCommand command) => command.IsExecuting.Subscribe(x => IsBusy = x, _ => IsBusy = false, () => IsBusy = false).DisposeWith(DeactivateWith);
        protected void BindBusy1Command(IReactiveCommand command) => command.IsExecuting.Subscribe(x => Is1Busy = x, _ => Is1Busy = false, () => Is1Busy = false).DisposeWith(DeactivateWith);
        protected void BindBusy2Command(IReactiveCommand command) => command.IsExecuting.Subscribe(x => Is2Busy = x, _ => Is2Busy = false, () => Is2Busy = false).DisposeWith(DeactivateWith);
        protected void BindBusy3Command(IReactiveCommand command) => command.IsExecuting.Subscribe(x => Is3Busy = x, _ => Is3Busy = false, () => Is3Busy = false).DisposeWith(DeactivateWith);
        protected void BindBusy4Command(IReactiveCommand command) => command.IsExecuting.Subscribe(x => Is4Busy = x, _ => Is4Busy = false, () => Is4Busy = false).DisposeWith(DeactivateWith);
        protected void BindBusy5Command(IReactiveCommand command) => command.IsExecuting.Subscribe(x => Is5Busy = x, _ => Is5Busy = false, () => Is5Busy = false).DisposeWith(DeactivateWith);
        protected void BindBusy6Command(ReactiveCommand<object, Unit> command) => command.IsExecuting.Subscribe(x => Is6Busy = x, _ => Is6Busy = false, () => Is6Busy = false).DisposeWith(DeactivateWith);

        private DelegateCommand<string> _scanBarcodeCommand;
        public DelegateCommand<string> ScanBarcodeCommand
        {
            get
            {
                if (_scanBarcodeCommand == null)
                {
                    _scanBarcodeCommand = new DelegateCommand<string>(async (r) =>
                    {
                        await this.NavigateAsync("ScanBarcodePage", ("action", "add"));
                    });
                }
                return _scanBarcodeCommand;
            }
        }

        #endregion

        #region Action

        //private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private bool once = false;
        private async void CheckAccessExpired()
        {
            if (!once)
            {
                if (this.PageName != "LoginPage" && this.PageName != "SecurityPage")
                {
                    if (!Settings.IsAuthenticated)
                    {
                        var ok = await UserDialogs.Instance.ConfirmAsync("账户会话已经过期？", "", "重新登录", "");
                        if (ok)
                        {
                            once = true;
                            await _navigationService.NavigateAsync("LoginPage");
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 两次点击按钮之间的点击间隔不能少于1000毫秒
        /// </summary>
        /// <returns></returns>
        public static bool IsFastClick()
        {
            bool flag = false;
            long curClickTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            if ((curClickTime - App.LastClickTime) >= App.MIN_CLICK_DELAY_TIME)
            {
                flag = true;
            }
            App.LastClickTime = curClickTime;
            return flag;
        }

        public static void ThrottleLoad(Action action, bool condition = false, bool enable = true)
        {
            if (condition)
            {
                action?.Invoke();
            }
            else
            {
                long curLoadTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                if ((curLoadTime - App.LastLoadTime) >= App.MIN_LOAD_DELAY_TIME)
                {
                    if (enable)
                    {
                        action?.Invoke();
                    }
                }
                App.LastLoadTime = curLoadTime;
            }
        }


        public async void SelectDateRang(bool load = false)
        {
            var result = await CrossDiaglogKit.Current.GetDateTimePickerRankAsync("选择日期");
            if (result != null)
            {
                Filter.StartTime = UtcHelper.ConvertDateTimeInt(result.Item1.ToUniversalTime());
                Filter.EndTime = UtcHelper.ConvertDateTimeInt(result.Item2.ToUniversalTime());
                ((ICommand)Load)?.Execute(null);
            }
        }

        public async void ClearHistory(Func<Task<bool>> func)
        {
            var result = await func?.Invoke();
            if (result)
            {
                await ShowAlert(true);
            }
        }


        /// <summary>
        /// 异步转向导航
        /// </summary>
        /// <param name="page">页面名称</param>
        /// <param name="param">动态匿名参数</param>
        /// <returns></returns>
        public async Task NavigateAsync(string page, params (string, object)[] parameters)
        {
            if (!InternetConnectivity())
            {
                _dialogService.LongAlert("网络已经断开 :(");
                return;
            }

            var now = DateTime.Now;
            try
            {
                if (now - this.lastTapTimestamp < this.minTapInterval)
                {
                    return;
                }

                await _navigationService.TryNavigateAsync(page, parameters.ToNavParams());
            }
            finally
            {
                this.lastTapTimestamp = now;
            }
        }
        public IReactiveCommand Navigate(string page, params (string, object)[] parameters)
        {
            return ReactiveCommand.Create(async () => { await this.NavigateAsync(page, parameters); });
        }

        public void Alert(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (_dialogService != null)
                {
                    _dialogService.LongAlert(message);
                }
            });
        }
        public async Task<Unit> ShowConfirm(Action call, bool success, string message = null, string reffPage = "")
        {
            if (message == null)
            {
                message = success ? "恭喜，操作成功!" : "抱歉，操作失败!";
            }
            var ok = await CrossDiaglogKit.Current.ShowSuccessAsync(message, success, true);
            if (ok)
            {
                call?.Invoke();
            }
            return Unit.Default;
        }
        public async Task<Unit> ShowConfirm(bool success, string message = null, bool goBack = true, bool goReceipt = true, string reffPage = "")
        {
            if (message == null)
            {
                message = success ? "恭喜，操作成功!" : "抱歉，操作失败!";
            }
            var ok = await CrossDiaglogKit.Current.ShowSuccessAsync(message, success, true);
            if (ok)
            {
                //提示是否去签收
                if (goReceipt)
                {
                    await PopupNavigation.Instance.PushAsync(new GoReceiptPage());
                    return Unit.Default;
                }

                if (goBack)
                    await _navigationService.GoBackAsync(("reffPage", reffPage));

            }
            return Unit.Default;
        }
        public async Task<Unit> ShowAlert(bool success, string message = null)
        {
            if (message == null)
            {
                message = success ? "恭喜，操作成功!" : "抱歉，操作失败!";
            }
            await CrossDiaglogKit.Current.ShowSuccessAsync(message, success, true);
            return Unit.Default;
        }
        private bool Authorization(params AccessGranularityEnum[] access)
        {
            try
            {
                if (access==null||access.Length<=0)
                {
                    return true;
                }

                foreach (var item in access)
                {
                    var authorizeCodes = JsonConvert.DeserializeObject<List<PermissionRecordQuery>>(Settings.AvailablePermissionRecords);
                    if (authorizeCodes != null && authorizeCodes.Count > 0 && authorizeCodes.Where(a => a.Code == (int)item).ToList().Count > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }
        public async Task<bool> Access(AccessGranularityEnum age)
        {
            var accessState = Authorization(age);
            if (!accessState)
            {
                await ShowAlert(false, "对不起，你无权访问");
                return false;
            }
            return true;
        }
        public async Task<bool> Access(Module m, AccessStateEnum ase)
        {
            if (m == null) return false;
            var accessState = Authorization(m.PermissionCodes?.ToArray());
            if (!accessState)
            {
                await ShowAlert(false, "对不起，你无权访问");
                return false;
            }
            return true;
        }
        public async Task<bool> Access(Module m, AccessStateEnum ase, Action call)
        {
            if (m == null) return false;
            var accessState = Authorization(m.PermissionCodes?.ToArray());
            if (!accessState)
            {
                await ShowAlert(false, "对不起，你无权访问");
                return false;
            }
            else
            {
                call?.Invoke();
                return true;
            }
        }
        public async Task<Unit> Access(AccessGranularityEnum ase, Action call)
        {
            var accessState = Authorization(ase);
            if (!accessState)
            {
                await ShowAlert(false, "对不起，你无权访问");
            }
            else
            {
                call?.Invoke();
            }
            return Unit.Default;
        }
        public async Task<Unit> Access(AccessGranularityEnum ase, Func<Task<Unit>> call)
        {
            var accessState = Authorization(ase);
            if (!accessState)
            {
                return await ShowAlert(false, "对不起，你无权访问");
            }
            else
            {
                return await call.Invoke();
            }
        }

        /// <summary>
        /// 获取VM对应PageName
        /// </summary>
        public string PageName
        {
            get { return this.GetType().Name.Replace("ViewModel", "").Trim(); }
        }

        /// <summary>
        /// 获取Page对应VM Name
        /// </summary>
        public string PageViewName
        {
            get { return this.GetType().Name.Replace("ViewModel", "").Replace("Page", "View").Trim(); }
        }

        /// <summary>
        /// 用于提交单据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="postData"></param>
        /// <param name="func"></param>
        /// <param name="clear"></param>
        /// <returns></returns>
        public async Task<Unit> SubmitAsync<T>(T postData, int billId, Func<T, int, CancellationToken, Task<APIResult<T>>> func, Action<APIResult<T>> clear, bool goBack = true, bool goReceipt = false, CancellationToken token = default) where T : Base
        {
            if (!InternetConnectivity())
            {
                _dialogService.LongAlert("网络已经断开 :(");
                return Unit.Default;
            }

            return await RapidTapPreventorAsync(async () =>
            {
                try
                {
                    APIResult<T> result = null;
                    using (Acr.UserDialogs.UserDialogs.Instance.Loading("提交中..."))
                    {
                        result = await func?.Invoke(postData, billId, token);
                    }
                    if (result != null)
                    {
                        if (result.Success)
                        {
                            clear?.Invoke(result);
                            return await ShowConfirm(result.Success, "提交成功！", goBack, goReceipt);
                        }
                        else
                        {
                            await ShowAlert(false, result.Message);
                        }
                    }
                    else
                    {
                        await ShowAlert(false, "提交失败！");
                    }
                }
                catch (Exception ex)
                {
                    await ShowAlert(false, ex.Message);
                }

                return Unit.Default;
            });
        }

        /// <summary>
        /// 用于提交单据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="postData"></param>
        /// <param name="func"></param>
        /// <param name="clear"></param>
        /// <returns></returns>
        public async Task<Unit> SubmitAsync<T>(T postData, Func<T, CancellationToken, Task<APIResult<T>>> func, Action<APIResult<T>> clear, bool goBack = true, bool goReceipt = false, CancellationToken token = default, string reffPage = "") where T : Base
        {
            if (!InternetConnectivity())
            {
                _dialogService.LongAlert("网络已经断开 :(");
                return Unit.Default;
            }

            return await RapidTapPreventorAsync(async () =>
            {
                try
                {
                    APIResult<T> result = null;
                    using (UserDialogs.Instance.Loading("提交中..."))
                    {
                        try
                        {
                            result = await func?.Invoke(postData, token);
                        }
                        catch (Exception)
                        { 
                        
                        }
                    }

                    if (result != null)
                    {
                        if (result.Success)
                        {
                            try
                            {
                                clear?.Invoke(result);
                            }
                            catch (Exception)
                            {

                            }
                            return await ShowConfirm(result.Success, "提交成功！", goBack, goReceipt, reffPage);
                        }
                        else
                        {
                            try
                            {
                                clear?.Invoke(result);
                            }
                            catch (Exception)
                            {

                            }
                            await ShowAlert(false, result.Message);
                        }
                    }
                    else
                    {
                        await ShowAlert(false, "提交失败！");
                    }
                }
                catch (Exception ex)
                {
                    await ShowAlert(false, ex.Message);
                }
                return Unit.Default;
            });
        }

        /// <summary>
        /// 用于单据审核/回冲
        /// </summary>
        /// <param name="billId"></param>
        /// <param name="func"></param>
        /// <param name="clear"></param>
        /// <returns></returns>
        public async Task<Unit> SubmitAsync(int billId, Func<int, CancellationToken, Task<bool>> func, Action<bool> call, CancellationToken token = default)
        {
            if (!InternetConnectivity())
            {
                _dialogService.LongAlert("网络已经断开 :(");
                return Unit.Default;
            }

            return await RapidTapPreventorAsync(async () =>
            {
                try
                {
                    var ok = await _dialogService.ShowConfirmAsync("确认审批吗？", okText: "确定", cancelText: "取消");
                    if (ok)
                    {
                        bool result = false;
                        using (UserDialogs.Instance.Loading("提交中..."))
                        {
                            result = await func?.Invoke(billId, token);
                        }
                        if (result)
                        {
                            call?.Invoke(result);
                            return await ShowConfirm(true, "审核成功");
                        }
                        else
                        {
                            await ShowAlert(false, "审核失败！");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await ShowAlert(false, ex.Message);
                }
                return Unit.Default;
            });
        }

        /// <summary>
        /// 用于单据审核/回冲 ResultData
        /// </summary>
        /// <param name="billId"></param>
        /// <param name="func"></param>
        /// <param name="clear"></param>
        /// <returns></returns>
        public async Task<Unit> SubmitAsync(int billId, Func<int, CancellationToken, Task<ResultData>> func, Action<bool> call, CancellationToken token = default)
        {
            if (!InternetConnectivity())
            {
                _dialogService.LongAlert("网络已经断开 :(");
                return Unit.Default;
            }

            return await RapidTapPreventorAsync(async () =>
            {
                try
                {
                    var ok = await _dialogService.ShowConfirmAsync("确认审批吗？", okText: "确定", cancelText: "取消");
                    if (ok)
                    {
                        ResultData result = null;

                        using (UserDialogs.Instance.Loading("提交中..."))
                        {
                            result = await func?.Invoke(billId, token);
                        }

                        if (result != null)
                        {
                            call?.Invoke(result.Success);
                            return await ShowConfirm(true, "审核成功");
                        }
                        else
                        {
                            await ShowAlert(false, result.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await ShowAlert(false, ex.Message);
                }
                return Unit.Default;
            });
        }


        /// <summary>
        /// 单据账户映射转换为支付方式模型
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="accountMap"></param>
        /// <returns></returns>
        public virtual PaymentMethodBaseModel ToPaymentMethod(AbstractBill bill, IList<AccountMaping> accountMap)
        {
            var accounts = accountMap.Select(s =>
            {
                return new AccountingModel()
                {
                    Name = s.AccountingOptionName,
                    AccountingTypeId = s.AccountingTypeId,
                    AccountCodeTypeId = s.AccountCodeTypeId,
                    AccountingOptionId = s.AccountingOptionId,
                    AccountingOptionName = s.AccountingOptionName,
                    CollectionAmount = s.CollectionAmount,
                };
            }).ToList();

            var pay = new PaymentMethodBaseModel()
            {
                Selectes = new ObservableCollection<AccountingModel>(accounts),
                SubAmount = bill.SumAmount,
                PreferentialAmount = bill.PreferentialAmount,
                OweCash = bill.OweCash
            };
            return pay;
        }

        /// <summary>
        /// 匹配语音
        /// </summary>
        public async void RecognitionSpeech(Action<string> action)
        {
            if (!InternetConnectivity())
            {
                _dialogService.LongAlert("网络已经断开 :(");
                return;
            }

            Counter = 6;
            IconCounter = Counter.ToString();
            _timer = new System.Threading.Timer((state) =>
            {
                if (Counter > 0)
                {
                    Counter--;
                    IconCounter = Counter.ToString();
                }
                else
                {
                    IconCounter = "microphone";
                    _timer.Dispose();
                }
            }, null, 0, 1000);

            try
            {

                //终结点: https://westus.api.cognitive.microsoft.com/sts/v1.0
                //密钥 1: 3ed4e3b74df348d2aa76e94dbb9faa4c
                //密钥 2: e8a80d28659448c485ad4f586d19e494

                //var config = SpeechConfig.FromSubscription("密匙", "区域");
                var speechConfig = SpeechConfig.FromSubscription("3ed4e3b74df348d2aa76e94dbb9faa4c", "westus");
                speechConfig.SpeechRecognitionLanguage = "zh-CN";


                // 创建一个异步任务数组
                //var stopRecognition = new TaskCompletionSource<int>();
                //相对路径转绝对路径
                //string fullpath = Path.GetFullPath(@"./whatstheweatherlike.wav");

                using (var recognizer = new SpeechRecognizer(speechConfig))
                {
                    this.RecognitionText = $"请说一句话...";

                    //开始录入，并返回结果
                    var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);
                    this.Counter = 0;
                    switch (result.Reason)
                    {
                        //语音识别成功
                        case ResultReason.RecognizedSpeech:
                            {
                                this.RecognitionText = $"识别到的语音为 {result.Text}";
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    action?.Invoke(result.Text.Replace("。", "").Replace(".", ""));
                                });
                            }
                            break;
                        //未识别到语音
                        case ResultReason.NoMatch:
                            this.RecognitionText = "没有识别到语音.";
                            break;
                        //取消识别
                        case ResultReason.Canceled:
                            {
                                var cancellation = CancellationDetails.FromResult(result);
                                var sb = new StringBuilder();
                                sb.AppendLine($"取消：原因={cancellation.Reason}\r");

                                //识别出错
                                if (cancellation.Reason == CancellationReason.Error)
                                {
                                    sb.AppendLine($"已取消: 错误码={cancellation.ErrorCode}\r");
                                    sb.AppendLine($"已取消: 详细描述={cancellation.ErrorDetails}\r");
                                    sb.AppendLine("已取消: 你更新订阅信息了吗?");
                                }
                                this.RecognitionText = sb.ToString();
                                Counter = 0;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Counter = 0;
                await _dialogService.DisplayAlertAsync("Exception", message: ex.Message, cancelButton: "取消");
            }
        }

        /// <summary>
        /// 接收消息转单
        /// </summary>
        /// <param name="mqObj"></param>
        public async Task RedirectAsync(MessageInfo item, params (string, object)[] parameters)
        {
            if (!InternetConnectivity())
            {
                _dialogService.LongAlert("网络已经断开 :(");
                return;
            }

            if (item != null)
            {
                try
                {
                    if (item != null)
                    {
                        var navigation = "";
                        if (item.MType == MTypeEnum.Receipt)
                        {
                            await this.NavigateAsync("ReceiptBillPage", ("Bill", null), ("Message", item));
                        }
                        else if (item.MType == MTypeEnum.Hold)
                        {
                            await this.NavigateAsync("ReconciliationORreceivablesPage", ("Bill", null), ("Message", item));
                        }
                        else
                        {
                            navigation = GlobalSettings.GetNavigation(item.BillType);

                            switch (item.BillType)
                            {
                                case BillTypeEnum.SaleReservationBill://销售订单
                                    {
                                        if (!string.IsNullOrEmpty(navigation) && item.BillId != 0)
                                        {
                                            var service = App.Resolve<ISaleReservationBillService>();
                                            var bill = await service?.GetBillAsync(item.BillId, this.ForceRefresh);
                                            if (bill != null && bill.Id > 0)
                                            {
                                                await this.NavigateAsync(navigation, ("Bill", bill));
                                            }
                                            else
                                            {
                                                _dialogService.ShortAlert("无效的单据信息 :(");
                                                return;
                                            }
                                        }
                                    }
                                    break;
                                case BillTypeEnum.SaleBill://销售单
                                    {

                                        if (!string.IsNullOrEmpty(navigation) && item.BillId != 0)
                                        {
                                            var service = App.Resolve<ISaleBillService>();
                                            var bill = await service?.GetBillAsync(item.BillId, this.ForceRefresh);
                                            if (bill != null && bill.Id > 0)
                                            {
                                                await this.NavigateAsync(navigation, ("Bill", bill), ("Message", item));
                                            }
                                            else
                                            {
                                                _dialogService.ShortAlert("无效的单据信息 :(");
                                                return;
                                            }
                                        }
                                    }
                                    break;
                                case BillTypeEnum.ReturnReservationBill://退货订单
                                    {

                                        if (!string.IsNullOrEmpty(navigation) && item.BillId != 0)
                                        {
                                            var service = App.Resolve<IReturnReservationBillService>();
                                            var bill = await service?.GetBillAsync(item.BillId, this.ForceRefresh);
                                            if (bill != null && bill.Id > 0)
                                            {
                                                await this.NavigateAsync(navigation, ("Bill", bill), ("Message", item));
                                            }
                                            else
                                            {
                                                _dialogService.ShortAlert("无效的单据信息 :(");
                                                return;
                                            }
                                        }
                                    }
                                    break;
                                case BillTypeEnum.ReturnBill://退货单
                                    {

                                        if (!string.IsNullOrEmpty(navigation) && item.BillId != 0)
                                        {
                                            var service = App.Resolve<IReturnBillService>();
                                            var bill = await service?.GetBillAsync(item.BillId, this.ForceRefresh);
                                            if (bill != null && bill.Id > 0)
                                            {
                                                await this.NavigateAsync(navigation, ("Bill", bill), ("Message", item));
                                            }
                                            else
                                            {
                                                _dialogService.ShortAlert("无效的单据信息 :(");
                                                return;
                                            }
                                        }
                                    }
                                    break;
                                case BillTypeEnum.PurchaseBill://采购单
                                    {

                                        if (!string.IsNullOrEmpty(navigation) && item.BillId != 0)
                                        {
                                            var service = App.Resolve<IPurchaseBillService>();
                                            var bill = await service?.GetBillAsync(item.BillId, this.ForceRefresh);
                                            if (bill != null && bill.Id > 0)
                                            {
                                                await this.NavigateAsync(navigation, ("Bill", bill), ("Message", item));
                                            }
                                            else
                                            {
                                                _dialogService.ShortAlert("无效的单据信息 :(");
                                                return;
                                            }
                                        }
                                    }
                                    break;
                                case BillTypeEnum.AllocationBill://调拨单
                                    {

                                        if (!string.IsNullOrEmpty(navigation) && item.BillId != 0)
                                        {
                                            var service = App.Resolve<IAllocationService>();
                                            var bill = await service?.GetBillAsync(item.BillId, this.ForceRefresh);
                                            if (bill != null && bill.Id > 0)
                                            {
                                                await this.NavigateAsync(navigation, ("Bill", bill), ("Message", item));
                                            }
                                            else
                                            {
                                                _dialogService.ShortAlert("无效的单据信息 :(");
                                                return;
                                            }
                                        }
                                    }
                                    break;
                                case BillTypeEnum.InventoryAllTaskBill://盘点单
                                    {

                                        if (!string.IsNullOrEmpty(navigation) && item.BillId != 0)
                                        {
                                            var service = App.Resolve<IInventoryService>();
                                            var bill = await service?.GetInventoryPartTaskBillAsync(item.BillId, this.ForceRefresh);
                                            if (bill != null && bill.Id > 0)
                                            {
                                                await this.NavigateAsync(navigation, ("Bill", bill), ("Message", item));
                                            }
                                            else
                                            {
                                                _dialogService.ShortAlert("无效的单据信息 :(");
                                                return;
                                            }
                                        }
                                    }
                                    break;
                                case BillTypeEnum.InventoryPartTaskBill://盘点单
                                    {
                                        if (!string.IsNullOrEmpty(navigation) && item.BillId != 0)
                                        {
                                            var service = App.Resolve<IInventoryService>();
                                            var bill = await service?.GetInventoryPartTaskBillAsync(item.BillId, this.ForceRefresh);
                                            if (bill != null && bill.Id > 0)
                                            {
                                                await this.NavigateAsync(navigation, ("Bill", bill), ("Message", item));
                                            }
                                            else
                                            {
                                                _dialogService.ShortAlert("无效的单据信息 :(");
                                                return;
                                            }
                                        }
                                    }
                                    break;
                                case BillTypeEnum.CashReceiptBill://收款单
                                    {
                                        if (!string.IsNullOrEmpty(navigation) && item.BillId != 0)
                                        {
                                            var service = App.Resolve<IReceiptCashService>();
                                            var bill = await service?.GetBillAsync(item.BillId, this.ForceRefresh);
                                            if (bill != null && bill.Id > 0)
                                            {
                                                await this.NavigateAsync(navigation, ("Bill", bill), ("Message", item));
                                            }
                                            else
                                            {
                                                _dialogService.ShortAlert("无效的单据信息 :(");
                                                return;
                                            }
                                        }
                                    }
                                    break;
                                case BillTypeEnum.CostExpenditureBill://费用支出
                                    {
                                        if (!string.IsNullOrEmpty(navigation) && item.BillId != 0)
                                        {
                                            var service = App.Resolve<ICostExpenditureService>();
                                            var bill = await service?.GetBillAsync(item.BillId, this.ForceRefresh);
                                            if (bill != null && bill.Id > 0)
                                            {
                                                await this.NavigateAsync(navigation, ("Bill", bill), ("Message", item));
                                            }
                                            else
                                            {
                                                _dialogService.ShortAlert("无效的单据信息 :(");
                                                return;
                                            }
                                        }
                                    }
                                    break;
                                case BillTypeEnum.CostContractBill://费用合同
                                    {
                                        if (!string.IsNullOrEmpty(navigation) && item.BillId != 0)
                                        {
                                            var service = App.Resolve<ICostContractService>();
                                            var bill = await service?.GetBillAsync(item.BillId, this.ForceRefresh);
                                            if (bill != null && bill.Id > 0)
                                            {
                                                await this.NavigateAsync(navigation, ("Bill", bill), ("Message", item));
                                            }
                                            else
                                            {
                                                _dialogService.ShortAlert("无效的单据信息 :(");
                                                return;
                                            }
                                        }
                                    }
                                    break;
                                case BillTypeEnum.AdvanceReceiptBill: //预收款单
                                    {
                                        if (!string.IsNullOrEmpty(navigation) && item.BillId != 0)
                                        {
                                            var service = App.Resolve<IAdvanceReceiptService>();
                                            var bill = await service?.GetBillAsync(item.BillId, this.ForceRefresh);
                                            if (bill != null && bill.Id > 0)
                                            {
                                                await this.NavigateAsync(navigation, ("Bill", bill), ("Message", item));
                                            }
                                            else
                                            {
                                                _dialogService.ShortAlert("无效的单据信息 :(");
                                                return;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }
        }

        #endregion

        #region ViewModelLoader

        public ViewModelLoader<IReadOnlyCollection<TrackingModel>> GpsEventsLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<TrackingModel>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<ProductModel>> ProductSeriesLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<ProductModel>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<CampaignBuyGiveProductModelGroup>> CampaignBuyGiveProductLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<CampaignBuyGiveProductModelGroup>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<TerminalModel>> TerminalsLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<TerminalModel>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<WareHouseModel>> WareHousesLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<WareHouseModel>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<List<MyReportingModel>> ChartsLoader { get; set; } = new ViewModelLoader<List<MyReportingModel>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<CashReceiptItemModel>> BillItemsLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<CashReceiptItemModel>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<List<TreeViewNode>> AccountLoader { get; set; } = new ViewModelLoader<List<TreeViewNode>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<CostContractBindingModel>> CostContractLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<CostContractBindingModel>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);

        public ViewModelLoader<IReadOnlyCollection<DeliverySignModel>> BillsLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<DeliverySignModel>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<BusinessVisitList>> BusinessVisitLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<BusinessVisitList>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<ManufacturerModel>> ManufacturersLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<ManufacturerModel>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<DispatchItemModel>> DispatchItemsLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<DispatchItemModel>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<AmountReceivableGroupModel>> AmountReceivableGroupsLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<AmountReceivableGroupModel>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<VisitStoreGroup>> VisitStoresLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<VisitStoreGroup>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<StockCategoryGroup>> StockSeriesLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<StockCategoryGroup>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<string> NewsLoader { get; set; } = new ViewModelLoader<string>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<MessageItemsGroup>> MessageLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<MessageItemsGroup>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<ModuleGroup>> ModulesLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<ModuleGroup>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        public ViewModelLoader<IReadOnlyCollection<NewsInfoModel>> NewsInfosLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<NewsInfoModel>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);
        #endregion

        public ViewModelBase(INavigationService navigationService, IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Activator = new ViewModelActivator();

            this.MenuBusKey = string.Format(Constants.MENU_KEY, this.GetType().FullName.ToUpper());
            this.ReferencePage = "";

            this.GoBackAsync = ReactiveCommand.CreateFromTask<object>(async e =>
            {
                await _navigationService.TryNavigateBackAsync();
            });

            //强制刷新
            this.RefreshCommand = ReactiveCommand.Create<object>(e =>
            {
                Settings.LastUpdateTime = DateTime.Now;
                this.ItemTreshold = -1;
                //强制刷新
                this.ForceRefresh = true;
                ((ICommand)Load)?.Execute(null);
                this.ForceRefresh = false;
            });


            //拨打电话
            this.CallPhone = ReactiveCommand.Create<string>(number =>
            {
                try
                {
                    PhoneDialer.Open(number);
                }
                catch (ArgumentNullException)
                {
                    Alert("数字为空或空白");
                }
                catch (FeatureNotSupportedException)
                {
                    Alert("此设备不支持电话拨号程序");
                }
                catch (Exception)
                {
                    Alert("发生了其他错误");
                }
            });

            this.NavigateCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.GoBackAsync.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.RefreshCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.CallPhone.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);

        }
    }

    /// <summary>
    /// 用于表示自定义VM基类
    /// </summary>
    public abstract class ViewModelBaseCutom : ViewModelBase
    {
        protected IProductService _productService;
        protected ITerminalService _terminalService;
        protected IUserService _userService;
        protected IWareHousesService _wareHousesService;
        protected IAccountingService _accountingService;

        public ViewModelBaseCutom(INavigationService navigationService,
            IProductService productService,
            ITerminalService terminalService,
            IUserService userService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            _productService = productService;
            _terminalService = terminalService;
            _userService = userService;
            _wareHousesService = wareHousesService;
            _accountingService = accountingService;

            //员工选择
            this.DeliverSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectUser((data) =>
                {
                    Filter.BusinessUserId = data.Id;
                    Filter.BusinessUserName = data.Column;
                    this.OnArchiveing();

                }, Enums.UserRoleType.Delivers, true);
            });
            //片区选择
            this.DistrictSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectDistrict((data) =>
                {
                    Filter.DistrictId = data.Id;
                    Filter.DistrictName = data.Name;
                    this.OnArchiveing();
                });
            });
            //品牌选择
            this.BrandSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectBrand((data) =>
                {
                    Filter.BrandId = data.Id;
                    Filter.BrandName = data.Name;
                    this.OnArchiveing();
                });
            });
            //备注选择
            this.RemarkSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectRemark((data) =>
                {
                    RemarkConfig.Id = data.Id;
                    RemarkConfig.Name = data.Name;
                    this.OnArchiveing();
                });
            });
            //客户选择
            this.CustomSelected = ReactiveCommand.CreateFromTask<object>(async e => await this.NavigateAsync("SelectCustomerPage"));
            //供应商选择
            this.ManufacturerSelected = ReactiveCommand.CreateFromTask<object>(async e => await this.NavigateAsync("SelectManufacturerPage"));

            this.DeliverSelected.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.DistrictSelected.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.CustomSelected.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.RemarkSelected.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.BrandSelected.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.ManufacturerSelected.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
        }


        #region Select


        public async Task<IList<BusinessUserModel>> GetUser(UserRoleType userRoleType, bool include = false)
        {
            IList<BusinessUserModel> users;
            if (include)
                users = await _userService.GetSubUsersAsync(this.ForceRefresh);
            else
            {
                var userFlag = "Users";
                switch (userRoleType)
                {
                    case UserRoleType.Administrators:
                        userFlag = "Administrators";
                        break;
                    case UserRoleType.Delivers:
                        userFlag = "Delivers";
                        break;
                    case UserRoleType.Financials:
                        userFlag = "Financials";
                        break;
                    case UserRoleType.Users:
                        userFlag = "Users";
                        break;
                    case UserRoleType.Employees:
                        userFlag = "Employees";
                        break;
                }
                users = await _userService.GetBusinessUsersAsync(null, userFlag);
            }
            return users;
        }

        public async Task SelectUser(Action<PopData> call, UserRoleType userRoleType, bool include = false)
        {
            //var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择员工", "", async () =>
            //{
            //    var users = await GetUser(userRoleType, include);
            //    var popDatas = new List<PopData>();
            //    if (users != null && users.Any())
            //    {
            //        popDatas = users?.Where(s => s != null)
            //        .Select(s => { return new PopData { Id = s.Id, Column = s.UserRealName }; })?.ToList();
            //    }
            //    return popDatas;
            //});
            //call?.Invoke(result);

            var _dialogView = new PopRadioButtonPage("选择员工", "", async () =>
            {
                var users = await GetUser(userRoleType, include);
                var popDatas = new List<PopData>();
                if (users != null && users.Any())
                {
                    popDatas = users?.Where(s => s != null)
                    .Select(s => { return new PopData { Id = s.Id, Column = s.UserRealName }; })?.ToList();
                }
                return popDatas;
            });

            _dialogView.Completed += (sender, result) =>
            {
                try { 
                if (result != null)
                {
                    call?.Invoke(result);
                }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            };

            await PopupNavigation.Instance.PushAsync(_dialogView);
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        /// <param name="parameter"></param>
        public async void SelectPaymentMethods(params (string, object)[] parameters)
        {
            await NavigateAsync("PaymentMethodPage", parameters);
        }

        /// <summary>
        /// 设置欠款
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="amount"></param>
        public async void SetOweCash(Action<string> callback, decimal amount = 0)
        {
            var result = await CrossDiaglogKit.Current.GetInputTextAsync("输入欠款", "", keyboard: Keyboard.Numeric, defaultValue: string.Format("{0:F2}", amount));
            if (!string.IsNullOrEmpty(result))
            {
                callback(result);
            }
        }


        /// <summary>
        /// 设置折扣
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="amount"></param>
        public async void SetDiscount(Action<string> callback, decimal amount = 0)
        {
            var result = await CrossDiaglogKit.Current.GetInputTextAsync("输入优惠", "", keyboard: Keyboard.Numeric, defaultValue: string.Format("{0:F2}", amount));
            if (!string.IsNullOrEmpty(result))
            {
                callback(result);
            }
        }

        /// <summary>
        /// 整单备注
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="remark"></param>
        public async void AllRemak(Action<string> callback, string remark = "")
        {
            var result = await CrossDiaglogKit.Current.GetInputTextAsync("整单备注", "", keyboard: Keyboard.Text, defaultValue: remark);
            if (!string.IsNullOrEmpty(result))
            {
                callback(result);
            }
        }

        /// <summary>
        /// 重置单据
        /// </summary>
        /// <param name="callback"></param>
        public async void ClearForm(Action callback)
        {
            var ok = await _dialogService.ShowConfirmAsync("你确定要清除项目吗？", okText: "清空", cancelText: "取消");
            if (ok)
            {
                callback();
            }
        }

        /// <summary>
        /// 清理单据
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TT"></typeparam>
        /// <param name="bill"></param>
        public async void ClearBill<TData, TT>(TData bill, Action doClear = null) where TData : AbstractBill, IBCollection<TT> where TT : EntityBase
        {
            var ok = await _dialogService.ShowConfirmAsync("你确定要清除项目吗？", okText: "清空", cancelText: "取消");
            if (ok)
            {
                if (doClear == null)
                {
                    if (null != bill)
                    {
                        bill.BillNumber = CommonHelper.GetBillNumber(CommonHelper.GetEnumDescription(bill.BillType).Split(',')[1], Settings.StoreId);
                        bill.Items?.Clear();
                    }
                }
                else
                {
                    doClear.Invoke();
                }
            }
        }

        /// <summary>
        /// 选择打印设备
        /// </summary>
        public async Task SelectPrint(AbstractBill bill)
        {
            //var num = await CrossDiaglogKit.Current.GetInputTextAsync("重复打印数", "", keyboard: Keyboard.Numeric, defaultValue: string.Format("{0}", 2),placeHolder: "自定义打印单页次数");
            //if (!string.IsNullOrEmpty(num))
            //{
            //    await NavigateAsync("PrintSettingPage", ("Bill", bill), ("PrintNum", num));
            //}

            int printNum = 1;
            switch (bill.BillType)
            {
                case BillTypeEnum.SaleBill:
                    printNum = Settings.SaleBill?.PrintNum ?? 0;
                    break;
                case BillTypeEnum.SaleReservationBill:
                    printNum = Settings.SaleReservationBill?.PrintNum ?? 0;
                    break;
                case BillTypeEnum.ReturnBill:
                    printNum = Settings.ReturnBill?.PrintNum ?? 0;
                    break;
                case BillTypeEnum.ReturnReservationBill:
                    printNum = Settings.ReturnReservationBill?.PrintNum ?? 0;
                    break;
                case BillTypeEnum.ExchangeBill:
                    printNum = Settings.ExchangeBill?.PrintNum ?? 0;
                    break;
                case BillTypeEnum.AllocationBill:
                    printNum = Settings.AllocationBill?.PrintNum ?? 0;
                    break;
                case BillTypeEnum.CashReceiptBill:
                    printNum = Settings.CashReceiptBill?.PrintNum ?? 0;
                    break;
                case BillTypeEnum.PurchaseBill:
                    printNum = Settings.PurchaseBill?.PrintNum ?? 0;
                    break;
                case BillTypeEnum.InventoryPartTaskBill:
                    printNum = Settings.InventoryPartTaskBill?.PrintNum ?? 0;
                    break;
                case BillTypeEnum.CostExpenditureBill:
                    printNum = Settings.CostExpenditureBill?.PrintNum ?? 0;
                    break;
                case BillTypeEnum.CostContractBill:
                    printNum = Settings.CostContractBill?.PrintNum ?? 0;
                    break;
                case BillTypeEnum.AdvanceReceiptBill:
                    printNum = Settings.AdvanceReceiptBill?.PrintNum ?? 0;
                    break;
            }

            if (!string.IsNullOrEmpty(App.BtAddress))
            {
                var _blueToothService = DependencyService.Get<IBlueToothService>();
                _blueToothService?.Print(bill, Settings.PrintStyleSelected, printNum);
            }
            else
            {
                this.loaded = false;
                await NavigateAsync("PrintSettingPage", ("Bill", bill), ("PrintNum", printNum));
            }
        }

        /// <summary>
        /// 历史单据
        /// </summary>
        public async Task SelectHistory()
        {
            await NavigateAsync("BillSummaryPage", null);
        }

        /// <summary>
        /// 结算方式(付款方式)
        /// </summary>
        public async Task SelectCheckOutMethod()
        {
            try
            {
                /*
                var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("结算方式", "", () =>
                {
                    var popDatas = new List<PopData>()
                    {
                        new PopData { Id = (int)PaymentMethodType.XianJie, Column = "现结" },
                        new PopData { Id = (int)PaymentMethodType.GuaZhang, Column = "挂账" }
                    };
                    return Task.FromResult(popDatas);
                });

                if (result != null)
                {
                    this.PayTypeId = result.Id;
                    this.PayTypeName = result.Column;
                }
                */

                var _dialogView = new PopRadioButtonPage("结算方式", "", () =>
               {
                   var popDatas = new List<PopData>()
                   {
                        new PopData { Id = (int)PaymentMethodType.XianJie, Column = "现结" },
                        new PopData { Id = (int)PaymentMethodType.GuaZhang, Column = "挂账" }
                   };
                   return Task.FromResult(popDatas);
               });

                _dialogView.Completed += (sender, result) =>
                {
                    try {
                    if (result != null)
                    {
                        this.PayTypeId = result.Id;
                        this.PayTypeName = result.Column;
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Alert($"Error:{ex.Message}");
            }
        }

        /// <summary>
        /// 默认售价
        /// </summary>
        /// <param name="options"></param>
        public async Task SelectDefaultAmountId(List<SelectListItem> options)
        {
            try
            {
                /*
                 "SaleReservationBillDefaultAmounts": [
                      {
                        "Value": "0_0",
                        "Text": "进价",
                        "Disabled": false,
                        "Selected": false
                      },
                      {
                        "Value": "0_1",
                        "Text": "批发价格",
                        "Disabled": false,
                        "Selected": false
                      },
                      {
                        "Value": "0_2",
                        "Text": "零售价格",
                        "Disabled": false,
                        "Selected": false
                      },
                      {
                        "Value": "0_3",
                        "Text": "最低售价",
                        "Disabled": false,
                        "Selected": false
                      },
                      {
                        "Value": "0_4",
                        "Text": "上次售价",
                        "Disabled": false,
                        "Selected": false
                      },
                      {
                        "Value": "0_5",
                        "Text": "成本价",
                        "Disabled": false,
                        "Selected": false
                      },
                      {
                        "Value": "29_88",
                        "Text": "批发价方案",
                        "Disabled": false,
                        "Selected": false
                      },
                      {
                        "Value": "30_88",
                        "Text": "最低价方案",
                        "Disabled": false,
                        "Selected": false
                      }
                    ],
                 */

                /*
                var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("默认售价", "", (() =>
                {
                    var popDatas = new List<PopData>();
                    int index = 0;
                    if (options != null && options.Any())
                    {
                        options.ForEach(o =>
                        {
                            popDatas.Add(new PopData { Id = index, Column = o.Text, Column1 = o.Value });
                            index++;
                        });
                    }
                    return Task.FromResult(popDatas);
                }));

                if (result != null)
                {
                    Settings.DefaultPricePlan = result.Column1;
                }*/

                var _dialogView = new PopRadioButtonPage("选择员工", "", () =>
               {
                   var popDatas = new List<PopData>();
                   int index = 0;
                   if (options != null && options.Any())
                   {
                       options.ForEach(o =>
                       {
                           popDatas.Add(new PopData { Id = index, Column = o.Text, Column1 = o.Value });
                           index++;
                       });
                   }
                   return Task.FromResult(popDatas);
               });

                _dialogView.Completed += (sender, result) =>
                {
                    try {
                    if (result != null)
                    {
                        Settings.DefaultPricePlan = result.Column1;
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);

            }
            catch (Exception ex)
            {
                Alert($"Error:{ex.Message}");
            }
        }

        /// <summary>
        /// 科目选择
        /// </summary>
        /// <param name="call"></param>
        public async Task SelectCostAccounting(Action<AccountingModel> call, BillTypeEnum type)
        {
            try
            {
                /*
                var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择科目", "", async () =>
                {
                    var results = await _accountingService.GetDefaultAccountingAsync((int)type, this.ForceRefresh);
                    var popDatas = results?.Item2?.Select(s =>
                    {
                        if (s != null)
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name,
                                Column1 = (s?.AccountCodeTypeId ?? 0).ToString(),
                                Column1Enable = false
                            };
                        }
                        else
                        {
                            return new PopData();
                        }
                    })?.ToList();
                    return popDatas;
                });

                if (result != null)
                {
                    Accounting.AccountingOptionId = result.Id;
                    Accounting.Name = result.Column;
                    Accounting.AccountingOptionName = result.Column;
                    Accounting.AccountCodeTypeId = int.Parse(result.Column1);
                    call?.Invoke(this.Accounting);
                }
                */

                var _dialogView = new PopRadioButtonPage("选择员工", "", async () =>
                {
                    var results = await _accountingService.GetDefaultAccountingAsync((int)type, this.ForceRefresh);
                    var popDatas = results?.Item2?.Select(s =>
                    {
                        if (s != null)
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name,
                                Column1 = (s?.AccountCodeTypeId ?? 0).ToString(),
                                Column1Enable = false
                            };
                        }
                        else
                        {
                            return new PopData();
                        }
                    })?.ToList();
                    return popDatas;
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try {
                    if (result != null)
                    {
                        Accounting.AccountingOptionId = result.Id;
                        Accounting.Name = result.Column;
                        Accounting.AccountingOptionName = result.Column;
                        Accounting.AccountCodeTypeId = int.Parse(result.Column1);
                        call?.Invoke(this.Accounting);
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 科目选择
        /// </summary>
        /// <param name="call"></param>
        public async Task SelectAccounting(Action<AccountingModel> call, int billTypeId = 0, bool loadTuple = false)
        {
            try
            {
                /*
                var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择科目", "", async () =>
                {
                    var options = new List<AccountingModel>();
                    IList<AccountingOptionModel> result = null;

                    if (loadTuple)
                    {
                        var defaults = await _accountingService.GetDefaultAccountingAsync(billTypeId, this.ForceRefresh);
                        result = defaults?.Item2?.Select(a =>
                        {
                            return new AccountingOptionModel()
                            {
                                Id = a.Id,
                                AccountCodeTypeId = a.AccountCodeTypeId ?? 0,
                                Selected = false,
                                Name = a.Name,
                                Number = a.Number
                            };
                        }).ToList();
                    }
                    else
                    {
                        result = await _accountingService.GetPaymentMethodsAsync(billTypeId, this.ForceRefresh);
                    }

                    if (result != null)
                    {
                        switch (BillType)
                        {
                            case BillTypeEnum.SaleBill://销售单(收款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();

                                }
                                break;
                            case BillTypeEnum.SaleReservationBill://销售订单(收款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.ReturnBill://退货单(收款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.ReturnReservationBill://退货订单(收款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.CashReceiptBill://收款单(收款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.PaymentReceiptBill://付款单(付款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.AdvanceReceiptBill://预收款单(收款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Id = a.Id,
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.AdvancePaymentBill://预付款单(付款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.PurchaseBill://采购单(付款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.PurchaseReturnBill://采购退货单(付款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.CostExpenditureBill://费用支出（支出账户） 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.FinancialIncomeBill://财务收入（收款账户）  配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                        }
                    }

                    var popDatas = new List<PopData>();
                    if (options != null)
                    {
                        Accounts = new ObservableCollection<AccountingModel>(options);
                        popDatas = options.Select(s =>
                        {
                            if (s != null)
                            {
                                return new PopData
                                {
                                    Id = s?.Id ?? 0,
                                    Column = s?.Name,
                                    Column1 = (s?.AccountCodeTypeId ?? 0).ToString(),
                                    Column1Enable = false
                                };
                            }
                            else
                            {
                                return new PopData();
                            }
                        }).ToList();
                    }
                    return popDatas;
                });

                if (result != null)
                {
                    Accounting.AccountingOptionId = result.Id;
                    Accounting.Name = result.Column;
                    Accounting.AccountingOptionName = result.Column;
                    Accounting.AccountCodeTypeId = int.Parse(result.Column1);
                    call?.Invoke(Accounting);
                }
                */

                var _dialogView = new PopRadioButtonPage("选择科目", "", async () =>
                {
                    var options = new List<AccountingModel>();
                    IList<AccountingOptionModel> result = null;

                    if (loadTuple)
                    {
                        var defaults = await _accountingService.GetDefaultAccountingAsync(billTypeId, this.ForceRefresh);
                        result = defaults?.Item2?.Select(a =>
                        {
                            return new AccountingOptionModel()
                            {
                                Id = a.Id,
                                AccountCodeTypeId = a.AccountCodeTypeId ?? 0,
                                Selected = false,
                                Name = a.Name,
                                Number = a.Number
                            };
                        }).ToList();
                    }
                    else
                    {
                        result = await _accountingService.GetPaymentMethodsAsync(billTypeId, this.ForceRefresh);
                    }

                    if (result != null)
                    {
                        switch (BillType)
                        {
                            case BillTypeEnum.SaleBill://销售单(收款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();

                                }
                                break;
                            case BillTypeEnum.SaleReservationBill://销售订单(收款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.ReturnBill://退货单(收款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.ReturnReservationBill://退货订单(收款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.CashReceiptBill://收款单(收款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.PaymentReceiptBill://付款单(付款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.AdvanceReceiptBill://预收款单(收款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Id = a.Id,
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.AdvancePaymentBill://预付款单(付款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.PurchaseBill://采购单(付款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.PurchaseReturnBill://采购退货单(付款账户) 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.CostExpenditureBill://费用支出（支出账户） 配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                            case BillTypeEnum.FinancialIncomeBill://财务收入（收款账户）  配置
                                {
                                    options = result.Select(a =>
                                    {
                                        return new AccountingModel()
                                        {
                                            Default = a.IsDefault,
                                            AccountingOptionId = a.Id,
                                            AccountCodeTypeId = a.AccountCodeTypeId,
                                            Selected = false,
                                            AccountingOptionName = a.Name,
                                            Name = a.Name
                                        };
                                    }).ToList();
                                }
                                break;
                        }
                    }

                    var popDatas = new List<PopData>();
                    if (options != null)
                    {
                        Accounts = new ObservableCollection<AccountingModel>(options);
                        popDatas = options.Select(s =>
                        {
                            if (s != null)
                            {
                                return new PopData
                                {
                                    Id = s?.Id ?? 0,
                                    Column = s?.Name,
                                    Column1 = (s?.AccountCodeTypeId ?? 0).ToString(),
                                    Column1Enable = false
                                };
                            }
                            else
                            {
                                return new PopData();
                            }
                        }).ToList();
                    }
                    return popDatas;
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try {
                    if (result != null)
                    {
                        Accounting.AccountingOptionId = result.Id;
                        Accounting.Name = result.Column;
                        Accounting.AccountingOptionName = result.Column;
                        Accounting.AccountCodeTypeId = int.Parse(result.Column1);
                        call?.Invoke(Accounting);
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 仓库选择
        /// </summary>
        /// <param name="call"></param>
        public async Task SelectStock(Action<WareHouseModel> call, BillTypeEnum type)
        {
            try
            {
                /*
                var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择库存", "", async () =>
                {
                    var result = await _wareHousesService.GetWareHousesAsync(type, force: this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });
                if (result != null)
                {
                    WareHouse.Id = result?.Id ?? 0;
                    WareHouse.Name = result?.Column;
                    call?.Invoke(WareHouse);
                }
                */

                var _dialogView = new PopRadioButtonPage("选择库存", "", async () =>
                {
                    var result = await _wareHousesService.GetWareHousesAsync(type, force: this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try
                    { 
                    if (result != null)
                    {
                        WareHouse.Id = result?.Id ?? 0;
                        WareHouse.Name = result?.Column;
                        call?.Invoke(WareHouse);
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string> { { "Line:", "2137" } });
            }
        }

        /// <summary>
        /// 仓库默认选择
        /// </summary>
        /// <param name="call"></param>
        /// <param name="type"></param>
        public async Task DefaultSelectStock(Action<WareHouseModel> call, BillTypeEnum type)
        {
            try
            {
                /*
                var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择库存", "", async () =>
                {
                    var results = await _wareHousesService.GetWareHousesAsync(type, force: this.ForceRefresh);
                    if (results != null && results.Any())
                    {
                        var popDatas = results?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });

                if (result != null)
                {
                    WareHouse.Id = result.Id;
                    WareHouse.Name = result.Column;
                    call?.Invoke(WareHouse);
                }
                */

                var _dialogView = new PopRadioButtonPage("选择库存", "", async () =>
                {
                    var results = await _wareHousesService.GetWareHousesAsync(type, force: this.ForceRefresh);
                    if (results != null && results.Any())
                    {
                        var popDatas = results?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try
                    {
                    if (result != null)
                    {
                        WareHouse.Id = result.Id;
                        WareHouse.Name = result.Column;
                        call?.Invoke(WareHouse);
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 片区选择
        /// </summary>
        public async Task SelectDistrict(Action<DistrictModel> call, int defaultValue = 0)
        {
            try
            {
                /*
                var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择片区", "", async () =>
                {
                    var result = await _terminalService.GetDistrictsAsync(this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name,
                                Selected = (s?.Id ?? 0) == defaultValue
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });
                if (result != null)
                {
                    District.Id = result.Id;
                    District.Name = result.Column;
                    call?.Invoke(District);
                }
                */
                var _dialogView = new PopRadioButtonPage("选择片区", "", async () =>
                {
                    var result = await _terminalService.GetDistrictsAsync(true);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name,
                                Selected = (s?.Id ?? 0) == defaultValue
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try
                    { 
                    if (result != null)
                    {
                        District.Id = result.Id;
                        District.Name = result.Column;
                        call?.Invoke(District);
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 渠道选择
        /// </summary>
        /// <param name="call"></param>
        public async Task SelectChannel(Action<ChannelModel> call, int defaultValue = 0)
        {
            try
            {
                /*
                var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择渠道", "", async () =>
                {
                    var result = await _terminalService.GetChannelsAsync(this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            if (s != null)
                            {
                                return new PopData
                                {
                                    Id = s?.Id ?? 0,
                                    Column = s?.Name,
                                    Selected = (s?.Id ?? 0) == defaultValue
                                };
                            }
                            else
                            {
                                return new PopData();
                            }
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });
                if (result != null)
                {
                    Channel.Id = result.Id;
                    Channel.Name = result.Column;
                    call?.Invoke(Channel);
                }
                */

                var _dialogView = new PopRadioButtonPage("选择渠道", "", async () =>
                {
                    var result = await _terminalService.GetChannelsAsync(this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            if (s != null)
                            {
                                return new PopData
                                {
                                    Id = s?.Id ?? 0,
                                    Column = s?.Name,
                                    Selected = (s?.Id ?? 0) == defaultValue
                                };
                            }
                            else
                            {
                                return new PopData();
                            }
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try {
                    if (result != null)
                    {
                        Channel.Id = result.Id;
                        Channel.Name = result.Column;
                        call?.Invoke(Channel);
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 使用线路选择
        /// </summary>
        /// <param name="call"></param>
        public async Task SelectLine(Action<LineTierModel> call, int defaultValue = 0)
        {
            try
            {
                /*
                var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择线路", "", async () =>
                {
                    var result = await _terminalService.GetLineTiersAsync(this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Where(s => s != null).Select(s =>
                            {
                                return new PopData
                                {
                                    Id = s?.Id ?? 0,
                                    Column = s?.Name,
                                    Selected = (s?.Id ?? 0) == defaultValue
                                };
                            })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });
                if (result != null)
                {
                    LineTier.Id = result.Id;
                    LineTier.Name = result.Column;
                    call?.Invoke(LineTier);
                }
                */

                var _dialogView = new PopRadioButtonPage("选择线路", "", async () =>
                {
                    var result = await _terminalService.GetLineTiersAsync(this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Where(s => s != null).Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name,
                                Selected = (s?.Id ?? 0) == defaultValue
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try
                    {
                    if (result != null)
                    {
                        LineTier.Id = result.Id;
                        LineTier.Name = result.Column;
                        call?.Invoke(LineTier);
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 用户线路选择
        /// </summary>
        /// <param name="call"></param>
        public async Task SelectUserLine(Action<LineTierModel> call, int defaultValue = 0)
        {
            try
            {
                /*
                var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择线路", "", async () =>
                {
                    var result = await _terminalService.GetLineTiersByUserAsync(this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name,
                                Data = s?.Terminals,
                                Selected = (s?.Id ?? 0) == defaultValue
                            };
                        }).ToList();
                        return popDatas;
                    }
                    else
                    {

                        return null;
                    }
                });

                if (result != null)
                {
                    LineTier.Id = result.Id;
                    LineTier.Name = result.Column;

                    if (result.Data is List<TerminalModel> lines)
                    {
                        LineTier.Terminals = lines;
                    }

                    call?.Invoke(LineTier);
                }
                */

                var _dialogView = new PopRadioButtonPage("选择线路", "", async () =>
                {
                    var result = await _terminalService.GetLineTiersByUserAsync(this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name,
                                Data = s?.Terminals,
                                Selected = (s?.Id ?? 0) == defaultValue
                            };
                        }).ToList();
                        return popDatas;
                    }
                    else
                    {

                        return null;
                    }
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try
                    {
                    if (result != null)
                    {
                        LineTier.Id = result.Id;
                        LineTier.Name = result.Column;

                        if (result.Data is List<TerminalModel> lines)
                        {
                            LineTier.Terminals = lines;
                        }

                        call?.Invoke(LineTier);
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        ///  等级选择
        /// </summary>
        /// <param name="call"></param>
        public async Task SelectRank(Action<RankModel> call, int defaultValue = 0)
        {
            try
            {
                /*
                var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("等级选择", "", async () =>
                {
                    var result = await _terminalService.GetRanksAsync(this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name,
                                Selected = (s?.Id ?? 0) == defaultValue
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });

                if (result != null)
                {
                    Rank.Id = result.Id;
                    Rank.Name = result.Column;
                    call?.Invoke(Rank);
                }
                */
                var _dialogView = new PopRadioButtonPage("等级选择", "", async () =>
                {
                    var result = await _terminalService.GetRanksAsync(this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name,
                                Selected = (s?.Id ?? 0) == defaultValue
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try
                    {
                    if (result != null)
                    {
                        Rank.Id = result.Id;
                        Rank.Name = result.Column;
                        call?.Invoke(Rank);
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 类别选择
        /// </summary>
        public async Task SelectCatagory(Action<CategoryModel> call, int defaultValue = 0)
        {
            try
            {
                /*
                var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择商品类别", "", async () =>
                {
                    var result = await _productService.GetAllCategoriesAsync(this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name,
                                Selected = (s?.Id ?? 0) == defaultValue
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });
                if (result != null)
                {
                    Category.Id = result.Id;
                    Category.Name = result.Column;
                    call?.Invoke(Category);
                }
                */

                var _dialogView = new PopRadioButtonPage("选择商品类别", "", async () =>
                {
                    var result = await _productService.GetAllCategoriesAsync(this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name,
                                Selected = (s?.Id ?? 0) == defaultValue
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try { 
                    if (result != null)
                    {
                        Category.Id = result.Id;
                        Category.Name = result.Column;
                        call?.Invoke(Category);
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 品牌选择
        /// </summary>
        public async Task SelectBrand(Action<BrandModel> call, int defaultValue = 0)
        {
            try
            {
                /*
                var result = await CrossDiaglogKit.Current.GetRadioButtonResultAsync("选择品牌", "", async () =>
                {
                    var result = await _productService.GetBrandsAsync("", 0, 50, this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name,
                                Selected = (s?.Id ?? 0) == defaultValue
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });
                if (result != null)
                {
                    Brand.Id = result.Id;
                    Brand.Name = result.Column;
                    call?.Invoke(Brand);
                }
                */

                var _dialogView = new PopRadioButtonPage("选择品牌", "", async () =>
                {
                    var result = await _productService.GetBrandsAsync("", 0, 50, this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name,
                                Selected = (s?.Id ?? 0) == defaultValue
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try {
                    if (result != null)
                    {
                        Brand.Id = result.Id;
                        Brand.Name = result.Column;
                        call?.Invoke(Brand);
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        /// <summary>
        /// 单位选择
        /// </summary>
        /// <param name="call"></param>
        /// <param name="unitType"></param>
        public async Task SelectSpecification(Action<SpecificationAttributeOptionModel> call, int unitType)
        {
            try
            {
                var _dialogView = new PopRadioButtonPage("选择单位", "", async () =>
                {
                    var result = await _productService.GetSpecificationAttributeOptionsAsync(this.ForceRefresh);
                    if (result != null)
                    {
                        var options = new List<SpecificationAttributeOptionModel>();
                        if (result != null)
                        {
                            switch (unitType)
                            {
                                case 0:
                                    options = result.smallOptions?.ToList();
                                    break;
                                case 1:
                                    options = result.strokOptions?.ToList();
                                    break;
                                case 2:
                                    options = result.bigOptions?.ToList();
                                    break;
                            }
                        }
                        var popDatas = options?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s?.Id ?? 0,
                                Column = s?.Name
                            };
                        }).ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try
                    { 
                    if (result != null)
                    {
                        Specification.Id = result.Id;
                        Specification.Name = result.Column;
                        call?.Invoke(Specification);
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private readonly static object _ulock = new object();
        /// <summary>
        /// 检查签到,是否有，没签退客户
        /// </summary>
        /// <param name="terminalId"></param>
        /// <param name="businessUserId"></param>
        public async Task<bool> CheckSignIn()
        {

            //获取最后一次签到未签退记录
            var result = await _terminalService.GetOutVisitStoreAsync(Settings.UserId);
            if (result != null && (result?.Id ?? 0) > 0)
            {
                this.OutVisitStore = result;
                this.OutVisitStore.Id = result.Id;
                //记录下最近一次已经签到但未签退的客户Id
                Settings.LastSigninCoustmerId = result.TerminalId;
                Settings.LastSigninCoustmerName = result.TerminalName;
                Settings.LastSigninId = result.Id;
                return true;
            }
            //没有没签退的客户时
            else
            {
                //清除记录
                this.OutVisitStore = null;
                Settings.LastSigninCoustmerId = 0;
                Settings.LastSigninCoustmerName = "";
                Settings.LastSigninId = 0;
                return false;
            }
        }


        /// <summary>
        /// 备注选择
        /// </summary>
        /// <param name="call"></param>
        /// <param name="defaultValue"></param>
        public async Task SelectRemark(Action<RemarkConfig> call, int defaultValue = 0)
        {
            try
            {

                var _dialogView = new PopRadioButtonPage("选择备注", "", async () =>
                    {
                        var _settingService = App.Resolve<ISettingService>();
                        var result = await _settingService.GetRemarkConfigListSetting();
                        var popDatas = result?.Select(s =>
                        {
                            return new PopData
                            {
                                Id = s.Key,
                                Column = s.Value,
                                Selected = s.Key == defaultValue
                            };
                        })?.ToList();
                        return popDatas;
                    });

                _dialogView.Completed += (sender, result) =>
                {
                    try
                    {
                    if (result != null)
                    {
                        RemarkConfig.Id = result.Id;
                        RemarkConfig.Name = result.Column;
                        call?.Invoke(RemarkConfig);
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #endregion
    }

    /// <summary>
    /// 用于表示自定义Bill的VM基类
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class ViewModelBaseCutom<TData> : ViewModelBaseCutom where TData : AbstractBill
    {
        [Reactive] public TData Bill { get; set; }
        [Reactive] public bool StockSelectedChange { get; set; }
        public ReactiveCommand<object, Unit> StockSelected { get; set; }
        public ReactiveCommand<object, Unit> AddProductCommand { get; set; }
        public bool CheckBill()
        {
            if (this.Bill.ReversedStatus)
            {
                _dialogService.ShortAlert("已红冲单据不能操作");
                return false;
            }

            if (this.Bill.AuditedStatus)
            {
                _dialogService.ShortAlert("已审核单据不能操作");
                return false;
            }

            return true;
        }

        /*
        public async void ClearCacheDataByType(BillTypeEnum typeId)
        {
            try
            {
                var db = App.Resolve<ILiteDbService<CacheBillData>>();
                var result = await db.Table.FindAsync(c => c.TypeId == (int)typeId);
                if (result != null && result.Any())
                {
                    foreach (var item in result)
                    {
                        await db.Table.DeleteAsync(item.Id);
                    }
                }

                var db2 = App.Resolve<ILiteDbService<CachePaymentMethod>>();
                var paymentOldCache = await db2.Table.FindAsync(c => c.BillTypeId == (int)typeId);

                if (paymentOldCache != null && paymentOldCache.Any())
                {
                    foreach (var item in paymentOldCache)
                    {
                        await db2.Table.DeleteAsync(item.Id);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        public async void SetCacheDataInfo(BillTypeEnum typeId, AbstractBill bill, PaymentMethodBaseModel payment)
        {
            try
            {
                var db = App.Resolve<ILiteDbService<CacheBillData>>();
                var data = await db.Table.FindAsync(c => c.TypeId == (int)typeId);
                var result = data.OrderByDescending(c => c.Id).FirstOrDefault();
                if (result != null)
                    await db.Table.DeleteAsync(result.Id);

                var cacheData = new CacheBillData
                {
                    TypeId = (int)typeId,
                    DataValue = JsonConvert.SerializeObject(bill)
                };
                await db.InsertAsync(cacheData);


                var db2 = App.Resolve<ILiteDbService<CachePaymentMethod>>();
                var data2 = await db2.Table.FindAsync(c => c.BillTypeId == (int)typeId);
                var paymentOldCache = data2.ToList();
                if (paymentOldCache != null && paymentOldCache.Any())
                {
                    foreach (var item in paymentOldCache)
                    {
                        await db2.Table.DeleteAsync(item.Id);
                    }
                }
                if (payment != null)
                {
                    var cachePaymentData = new CachePaymentMethod
                    {
                        BillTypeId = (int)typeId,
                        BillGuid = bill.GUID,
                        DataValue = JsonConvert.SerializeObject(payment)
                    };
                    await db2.InsertAsync(cachePaymentData);
                }
            }
            catch (Exception)
            {
            }
        }
        public async Task<Tuple<AbstractBill, PaymentMethodBaseModel>> GetAbstractBillByType(BillTypeEnum typeId)
        {
            try
            {
                var db = App.Resolve<ILiteDbService<CacheBillData>>();

                var data = await db.Table.FindAsync(c => c.TypeId == (int)typeId);
                if (null == data)
                {
                    return null;
                }

                var result = data.OrderByDescending(c => c.Id).FirstOrDefault();

                AbstractBill cacheData = null;
                switch (typeId)
                {
                    case BillTypeEnum.None:
                        break;
                    case BillTypeEnum.ExchangeBill:
                        break;
                    case BillTypeEnum.SaleReservationBill:
                        cacheData = JsonConvert.DeserializeObject<SaleReservationBillModel>(result?.DataValue);
                        break;
                    case BillTypeEnum.SaleBill:
                        cacheData = JsonConvert.DeserializeObject<SaleBillModel>(result?.DataValue);
                        break;
                    case BillTypeEnum.ReturnReservationBill:
                        cacheData = JsonConvert.DeserializeObject<ReturnReservationBillModel>(result?.DataValue);
                        break;
                    case BillTypeEnum.ReturnBill:
                        cacheData = JsonConvert.DeserializeObject<ReturnBillModel>(result?.DataValue);
                        break;
                    case BillTypeEnum.CarGoodBill:
                        break;
                    case BillTypeEnum.FinanceReceiveAccount:
                        break;
                    case BillTypeEnum.PickingBill:
                        break;
                    case BillTypeEnum.DispatchBill:
                        break;
                    case BillTypeEnum.ChangeReservation:
                        break;
                    case BillTypeEnum.PurchaseReservationBill:
                        break;
                    case BillTypeEnum.PurchaseBill:
                        break;
                    case BillTypeEnum.PurchaseReturnReservationBill:
                        break;
                    case BillTypeEnum.PurchaseReturnBill:
                        break;
                    case BillTypeEnum.BackStockBill:
                        break;
                    case BillTypeEnum.AllocationBill:
                        break;
                    case BillTypeEnum.InventoryProfitLossBill:
                        break;
                    case BillTypeEnum.CostAdjustmentBill:
                        break;
                    case BillTypeEnum.ScrapProductBill:
                        break;
                    case BillTypeEnum.InventoryAllTaskBill:
                        break;
                    case BillTypeEnum.InventoryPartTaskBill:
                        break;
                    case BillTypeEnum.CombinationProductBill:
                        break;
                    case BillTypeEnum.SplitProductBill:
                        break;
                    case BillTypeEnum.InventoryReportBill:
                        break;
                    case BillTypeEnum.CashReceiptBill:
                        break;
                    case BillTypeEnum.PaymentReceiptBill:
                        break;
                    case BillTypeEnum.AdvanceReceiptBill:
                        break;
                    case BillTypeEnum.AdvancePaymentBill:
                        break;
                    case BillTypeEnum.CostExpenditureBill:
                        break;
                    case BillTypeEnum.CostContractBill:
                        break;
                    case BillTypeEnum.FinancialIncomeBill:
                        break;
                    case BillTypeEnum.AllLoadBill:
                        break;
                    case BillTypeEnum.ZeroLoadBill:
                        break;
                    case BillTypeEnum.AllZeroMergerBill:
                        break;
                    case BillTypeEnum.AccountingVoucher:
                        break;
                    case BillTypeEnum.StockReport:
                        break;
                    case BillTypeEnum.SaleSummeryReport:
                        break;
                    case BillTypeEnum.TransferSummaryReport:
                        break;
                    case BillTypeEnum.SaleSummeryProductReport:
                        break;
                    case BillTypeEnum.RecordingVoucher:
                        break;
                    case BillTypeEnum.LoanGoodsBill:
                        break;
                    case BillTypeEnum.ReturnGoodsBill:
                        break;
                    case BillTypeEnum.Other:
                        break;
                    default:
                        break;
                }
                PaymentMethodBaseModel payment = null;
                if (cacheData != null)
                {
                    var billGuid = cacheData?.GUID;
                    var db2 = App.Resolve<ILiteDbService<CachePaymentMethod>>();
                    var data2 = await db2.Table.FindAsync(c => c.BillTypeId == (int)typeId && billGuid == c.BillGuid);
                    var paymentCache = data2.FirstOrDefault();
                    if (null != paymentCache && !string.IsNullOrEmpty(paymentCache?.DataValue))
                    {
                        payment = JsonConvert.DeserializeObject<PaymentMethodBaseModel>(paymentCache?.DataValue);
                    }
                }

                return Tuple.Create(cacheData, payment);
            }
            catch (Exception)
            {
                return null;
            }
        }
        */
        public ViewModelBaseCutom(INavigationService navigationService,
            IProductService productService,
            ITerminalService terminalService,
            IUserService userService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IDialogService dialogService) : base(navigationService,
                productService,
                terminalService,
                userService,
                wareHousesService,
                accountingService,
                dialogService)
        {
            //客户选择
            this.CustomSelected = ReactiveCommand.CreateFromTask<object>(async e =>
            {
                if (CheckBill())
                    await this.NavigateAsync("SelectCustomerPage");
            });
            //供应商选择
            this.ManufacturerSelected = ReactiveCommand.CreateFromTask<object>(async e =>
            {
                if (CheckBill())
                    await this.NavigateAsync("SelectManufacturerPage");
            });
            //员工选择
            this.DeliverSelected = ReactiveCommand.CreateFromTask<object>(async e =>
           {
               if (CheckBill())
                   await SelectUser(Bill, UserRoleType.Delivers, true);
           });
            //选择仓库
            this.StockSelected = ReactiveCommand.CreateFromTask<object>(async e =>
            {
                if (e == null) return;
                if (CheckBill())
                {
                    await SelectStock((result) =>
                    {
                        if (result != null && WareHouse != null && Bill != null)
                        {
                            WareHouse.Id = result.Id;
                            WareHouse.Name = result.Column;

                            Bill.WareHouseId = result.Id;
                            Bill.WareHouseName = result.Column;

                            if (e.ToString() == "0")
                            {
                                Bill.ShipmentWareHouseId = result.Id;
                                Bill.ShipmentWareHouseName = result.Column;
                            }
                            else if (e.ToString() == "1")
                            {
                                Bill.IncomeWareHouseId = result.Id;
                                Bill.IncomeWareHouseName = result.Column;
                            }

                            this.OnArchiveing();
                            this.StockSelectedChange = true;
                        }

                    }, BillType);
                }
            });

            //审核禁用
            this.WhenAnyValue(x => x.Bill.AuditedStatus).Where(x => x == true)
                .Subscribe(x =>
                {
                    this.EnableOperation = false;
                    this.ShowAddProduct = true;
                    this.EnabledSubmitBtn = true;
                })
                .DisposeWith(this.DeactivateWith);

            //红冲禁用
            this.WhenAnyValue(x => x.Bill.ReversedStatus).Where(x => x == true)
               .Subscribe(x =>
               {
                   this.EnabledSubmitBtn = true;
               })
               .DisposeWith(this.DeactivateWith);

            this.DeliverSelected.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.CustomSelected.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.StockSelected.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.ManufacturerSelected.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
        }

        /// <summary>
        /// 留存拍照
        /// </summary>
        /// <returns></returns>
        public async Task TakePhotograph(Action<UploadResult, MediaFile> action)
        {
            try
            {
                var httpClientHelper = new HttpClientHelper();
                var mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    CompressionQuality = 60,
                    PhotoSize = PhotoSize.Medium,
                    Location = new PLU.Location
                    {
                        Latitude = GlobalSettings.Latitude ?? 0,
                        Longitude = GlobalSettings.Longitude ?? 0
                    }
                });

                if (mediaFile == null)
                {
                    return;
                }

                Stream convertStream = mediaFile.GetStream();

                //上传图片
                using (UserDialogs.Instance.Loading("上传中..."))
                {
                    try
                    {
                        var scb = new StreamContent(convertStream);
                        var content = new MultipartFormDataContent { { scb, "\"file\"", $"\"{mediaFile?.Path}\"" } };

                        var url = $"{GlobalSettings.FileCenterEndpoint}document/reomte/fileupload/HRXHJS";
                        var result = await httpClientHelper.PostAsync(url, content);
                        var uploadResult = new UploadResult();
                        if (!string.IsNullOrEmpty(result))
                        {
                            uploadResult = JsonConvert.DeserializeObject<UploadResult>(result);
                        }

                        if (uploadResult != null)
                        {
                            action.Invoke(uploadResult, mediaFile);
                        }

                        if (scb != null)
                            scb.Dispose();

                        if (content != null)
                            content.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                    finally
                    {
                        if (mediaFile != null)
                            mediaFile.Dispose();

                        if (convertStream != null)
                            convertStream.Dispose();
                    }
                };
            }
            catch (Exception ex)
            {
                _dialogService.LongAlert(ex.Message);
            }
        }

        public async Task SelectStock(TData bill, BillTypeEnum type)
        {
            try
            {
                var _dialogView = new PopRadioButtonPage("选择库存", "", async () =>
                {
                    var result = await _wareHousesService.GetWareHousesAsync(type);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Where(s => s != null)
                        .Select(s =>
                        {
                            return new PopData
                            {
                                Id = s.Id,
                                Column = s.Name
                            };
                        })?.ToList();
                        return popDatas;
                    }
                    else
                    {
                        return null;
                    }
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try { 
                    if (result != null)
                    {
                        WareHouse.Id = result.Id;
                        WareHouse.Name = result.Column;
                        bill.WareHouseId = result.Id;
                        bill.WareHouseName = result.Column;
                        bill.WareHouseId = result.Id;
                        bill.WareHouseName = result.Column;
                        bill.WareHouseId = result.Id;
                        bill.WareHouseName = result.Column;
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string> { { "Line:", "2766" } });
            }
        }
        public async Task SelectStock(Action<PopData> action, BillTypeEnum type)
        {
            try
            {
                var _dialogView = new PopRadioButtonPage("选择库存", "", async () =>
                {
                    var result = await _wareHousesService.GetWareHousesAsync(type, force: this.ForceRefresh);
                    if (result != null && result.Any())
                    {
                        var popDatas = result?.Where(s => s != null).Select(s =>
                        {
                            return new PopData
                            {
                                Id = s.Id,
                                Column = s.Name
                            };
                        })?.ToList();

                        return popDatas ?? new List<PopData>();
                    }
                    else
                    {
                        return new List<PopData>();
                    }
                });

                _dialogView.Completed += (sender, result) =>
                {
                    try
                    {
                    if (result != null)
                    {
                        action?.Invoke(result);
                    }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };

                await PopupNavigation.Instance.PushAsync(_dialogView);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string> { { "Line:", "2801" } });
            }
        }

        public async Task SelectUser(TData bill, UserRoleType userRoleType, bool include = false)
        {

            var _dialogView = new PopRadioButtonPage("选择员工", "", async () =>
            {
                var users = await GetUser(userRoleType, include);
                var popDatas = new List<PopData>();
                if (users != null && users.Any())
                {
                    popDatas = users?.Where(s => s != null)
                    .Select(s => { return new PopData { Id = s.Id, Column = s.UserRealName }; })?.ToList();
                }
                return popDatas;
            });

            _dialogView.Completed += (sender, result) =>
           {
               try
               {
                   if (result != null)
                   {
                       switch (userRoleType)
                       {
                           case UserRoleType.Delivers:
                               bill.DeliveryUserId = result?.Id ?? 0;
                               bill.DeliveryUserName = result?.Column;
                               break;
                           case UserRoleType.Employees:
                               bill.BusinessUserId = result?.Id ?? 0;
                               bill.BusinessUserName = result?.Column;
                               break;
                       }

                       this.OnArchiveing();
                   }
               }
               catch (Exception ex)
               {
                   Crashes.TrackError(ex);
               }
           };
            await PopupNavigation.Instance.PushAsync(_dialogView);
        }

        public void AppendMenus(AbstractBill bill)
        {
            //已审核未红冲
            if (bill.AuditedStatus && !bill.ReversedStatus)
            {
                //隐藏保存按钮
                ShowSubmitBtn = false;
                //控制显示菜单
                _popupMenu?.Show(5, 35, 36);
            }
            //未审核时
            else if (!bill.AuditedStatus && !bill.ReversedStatus)
            {
                _popupMenu?.Show(0, 2, 3, 4, 5, 6, 7, 34);
            }
            //已经红冲时
            else if (bill.ReversedStatus)
            {    //隐藏保存按钮
                ShowSubmitBtn = false;
                //控制显示菜单
                _popupMenu?.Show(5);
            }
        }
    }

    /// <summary>
    /// 用于表示Bill类的VM基类
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class ViewModelBaseOrder<TData> : ViewModelBase where TData : AbstractBill
    {
        protected IGlobalService _globalService;
        protected IAllocationService _allocationService;
        protected IAdvanceReceiptService _advanceReceiptService;
        protected IReceiptCashService _receiptCashService;
        protected ICostContractService _costContractService;
        protected ICostExpenditureService _costExpenditureService;
        protected IInventoryService _inventoryService;
        protected IPurchaseBillService _purchaseBillService;
        protected IReturnReservationBillService _returnReservationBillService;
        protected IReturnBillService _returnBillService;
        protected ISaleReservationBillService _saleReservationBillService;
        protected ISaleBillService _saleBillService;

        [Reactive] public ObservableCollection<TData> Bills { get; set; }
        public ReactiveCommand<TData, Unit> SelectedCommand { get; set; }
        [Reactive] public TData Selecter { get; set; }


        public ViewModelBaseOrder(INavigationService navigationService,
            IGlobalService globalService,
            IAllocationService allocationService,
            IAdvanceReceiptService advanceReceiptService,
            IReceiptCashService receiptCashService,
            ICostContractService costContractService,
            ICostExpenditureService costExpenditureService,
            IInventoryService inventoryService,
            IPurchaseBillService purchaseBillService,
            IReturnReservationBillService returnReservationBillService,
            IReturnBillService returnBillService,
            ISaleReservationBillService saleReservationBillService,
            ISaleBillService saleBillService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            _globalService = globalService;

            _allocationService = allocationService;
            _advanceReceiptService = advanceReceiptService;
            _receiptCashService = receiptCashService;
            _costContractService = costContractService;
            _costExpenditureService = costExpenditureService;
            _inventoryService = inventoryService;
            _purchaseBillService = purchaseBillService;
            _returnReservationBillService = returnReservationBillService;
            _returnBillService = returnBillService;
            _saleReservationBillService = saleReservationBillService;
            _saleBillService = saleBillService;

            Bills = new ObservableCollection<TData>();

        }
    }

    /// <summary>
    /// 用于表示Chart类的VM基类
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class ViewModelBaseChart<TData> : ViewModelBase, IReportTemplate where TData : ReactiveObject
    {
        public readonly IProductService _productService;
        public readonly IReportingService _reportingService;

        [Reactive] public ObservableCollection<TData> RankSeries { get; set; }
        [Reactive] public ChartViewConfig ChartConfig { get; set; }

        public ChartPageEnum PageType { get; set; }

        public ViewModelBaseChart(INavigationService navigationService,
            IProductService productService,
            IReportingService reportingService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            _productService = productService;
            _reportingService = reportingService;

            RankSeries = new ObservableCollection<TData>();
        }
    }
}

//msbuild DCMS.Client.Android.csproj -p:LinkerDumpDependencies = true
//msbuild DCMS.Client.Android.csproj -p:Configuration = Release

//    msbuild  -p:Configuration = Release;
