using Wesley.Client.CustomViews;
using Wesley.Client.Models;
using Wesley.Client.Models.Census;
using Wesley.Client.Pages;
using Wesley.Client.Pages.Archive;
using Wesley.Client.Pages.Bills;
using Wesley.Client.Pages.Common;
using Wesley.Client.Pages.Home;
using Wesley.Client.Pages.Market;
using Wesley.Client.Pages.Order;
using Wesley.Client.Pages.Reporting;
using Wesley.Client.Services;
using Wesley.Client.Services.QA;
using Wesley.Client.Services.Sales;
using Wesley.Client.ViewModels;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Prism;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Plugin.Popups;
using Prism.Unity;
using System;
using System.Reflection;
using Unity;
using Unity.Microsoft.Logging;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Wesley.Client.Models.Products;
using Wesley.Client.Models.Terminals;

[assembly: ExportFont("fa-solid-900.otf", Alias = "FAS")]
namespace Wesley.Client
{
    /// <summary>
    /// 使用PrismApplication接替，默认关闭自动导航注册 
    /// </summary>
    public partial class App : PrismApplication
    {
        public static readonly int MIN_CLICK_DELAY_TIME = 2000;
        public static readonly int MIN_LOAD_DELAY_TIME = 10000;
        public static double ScreenWidth;
        public static double ScreenHeight;
        public static long LastClickTime { get; set; }
        public static long LastLoadTime { get; set; }
        public static string BtAddress { get; set; } = "";
        public static string BtName { get; set; } = "";

        private static IUnityContainer _appContainer;

        public App(IPlatformInitializer initializer) : base(initializer) {}

        #region override



        protected async override void OnInitialized()
        {
            try
            {
                Settings.IsInitData = false;
                Settings.IsNextTimeUpdate = false;

                InitializeComponent();

                if (Settings.IsAuthenticated)
                    await NavigationService.NavigateAsync($"NavigationPage/{nameof(MainLayoutPage)}");
                else
                    await NavigationService.NavigateAsync($"NavigationPage/{nameof(LoginPage)}");

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 应用程序启动时调用
        /// </summary>
        protected override void OnStart()
        {
            GlobalSettings.IsSleeping = false;
            AppCenter.Start("android=03804b2b-3759-4bab-9286-ae78dcd60abe;", typeof(Analytics), typeof(Crashes));
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                if (string.IsNullOrWhiteSpace(viewType.FullName))
                    return null;
                var viewModelAssemblyName = typeof(ViewModelBase).GetTypeInfo().Assembly.FullName;
                var viewModelNamespace = typeof(ViewModelBase).Namespace;
                var viewModelTypeName = viewType.Name.Replace("View", "PageViewModel").Replace("Page", "PageViewModel");
                var viewModelName = $"{viewModelNamespace}.{viewModelTypeName}, {viewModelAssemblyName}";
                var viewModelType = Type.GetType(viewModelName);
                return viewModelType;
            });
        }

        /*
        private void CreateJob()
        {
            Task.Run(async () =>
            {
                try
                {
                    var _jobManager = Resolve<IJobManager>();
                    var jobs = await _jobManager.GetJobs();
                    if (jobs != null)
                    {
                        var jobRuns = jobs.Select(s => s.Identifier.ToUpper()).ToArray();

                        #region //数据同步Job

                        if (!jobRuns.Contains("DATASYNCJOB"))
                        {
                            await _jobManager.Register(new JobInfo(typeof(DataSyncJob), "DataSyncJob")
                            {
                                Repeat = true,
                                //指定设备电池电量低于阀值时是否启动任务
                                BatteryNotLow = true,
                                //充电时是否启动任务
                                DeviceCharging = true,
                                //是否运行在前台
                                RunOnForeground = true,
                                //是否要求网路连接
                                RequiredInternetAccess = InternetAccess.Any,
                                //每30秒运行次 SetParameter
                                Parameters = new Dictionary<string, object> { { "SecondsToRun", 30 } }
                            });
                        }

                        #endregion

                        #region //MQ消息订阅Job

                        //if (!jobRuns.Contains("MQSUBSRIBEJOB"))
                        //{
                        //    await _jobManager.Register(new JobInfo(typeof(MQSubsribeJob), "MQSubsribeJob")
                        //    {
                        //        Repeat = true,
                        //        //指定设备电池电量低于阀值时是否启动任务
                        //        BatteryNotLow = true,
                        //        //充电时是否启动任务
                        //        DeviceCharging = true,
                        //        //是否运行在前台
                        //        RunOnForeground = true,
                        //        //是否要求网路连接
                        //        RequiredInternetAccess = InternetAccess.Any,
                        //        //每10秒运行次 SetParameter
                        //        Parameters = new Dictionary<string, object> { { "SecondsToRun", 10 } }
                        //    });
                        //}

                        #endregion

                        #region //位置上报Job

                        if (!jobRuns.Contains("TRACKINGJOB"))
                        {
                            await _jobManager.Register(new JobInfo(typeof(TrackingJob), "TrackingJob")
                            {
                                Repeat = true,
                                //指定设备电池电量低于阀值时是否启动任务
                                BatteryNotLow = true,
                                //充电时是否启动任务
                                DeviceCharging = true,
                                //是否运行在前台
                                RunOnForeground = true,
                                //是否要求网路连接
                                RequiredInternetAccess = InternetAccess.Any,
                                //每10秒运行次 SetParameter
                                Parameters = new Dictionary<string, object> { { "SecondsToRun", 5 } }
                            });
                        }

                        #endregion
                    }

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });
        }
        */


        /// <summary>
        /// 在应用程序恢复后被发送到后台时调用
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
        }

        /// <summary>
        ///  每次应用程序进入后台调用
        /// </summary>
        protected override void OnSleep()
        {
            base.OnSleep();
            GlobalSettings.IsSleeping = true;
        }


        /// <summary>
        /// 注册依赖容器
        /// </summary>
        /// <param name="ctReg"></param>
        protected override void RegisterTypes(IContainerRegistry ctReg)
        {
            ctReg.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            ctReg.RegisterForNavigation<NavigationPage>();


            #region //注册弹出窗

            ctReg.RegisterPopupDialogService();
            ctReg.RegisterPopupNavigationService();

            ctReg.RegisterForNavigation<PopupMenu, PopupMenuViewModel>();
            ctReg.RegisterForNavigation<GoReceiptPage, GoReceiptPageViewModel>();
            ctReg.RegisterForNavigation<ChangeMenuPage, ChangeMenuPageViewModel>();
            ctReg.RegisterForNavigation<PopCheckBoxPage, PopCheckBoxPageViewModel>();
            ctReg.RegisterForNavigation<PopRadioButtonPage, PopRadioButtonPageViewModel>();
            ctReg.RegisterForNavigation<ProductCategoryPage, ProductCategoryPageViewModel>();
            #endregion

            #region //注册TabbedPage布局

            ctReg.Register<IMyTabbedPageSelectedTab, TopTabMessagesPageViewModel>();
            ctReg.RegisterForNavigation<MainLayoutPage, MainLayoutPageViewModel>();
            ctReg.RegisterForNavigation<HomePage, HomePageViewModel>();
            ctReg.RegisterForNavigation<MessagesPage, MessagesPageViewModel>();
            ctReg.RegisterForNavigation<AllfunPage, AllfunPageViewModel>();
            ctReg.RegisterForNavigation<NotificationsPage, NotificationsPageViewModel>();
            ctReg.RegisterForNavigation<NewsPage, NewsPageViewModel>();
            ctReg.RegisterForNavigation<ProfilePage, ProfilePageViewModel>();

            #endregion

            #region //注册签收布局

            ctReg.Register<DeliveriedPage>();
            ctReg.Register<UnOrderPage>();
            ctReg.Register<UnCostExpenditurePage>();
            ctReg.Register<UnSalePage>();
            
            ctReg.RegisterForNavigation<CameraViewPage, CameraPageViewModel>();
            ctReg.RegisterForNavigation<DeliveryReceiptPage, DeliveryReceiptPageViewModel>();
            ctReg.RegisterForNavigation<DeliveriedPage, DeliveriedPageViewModel>();
            ctReg.RegisterForNavigation<UnOrderPage, UnOrderPageViewModel>();
            ctReg.RegisterForNavigation<UnCostExpenditurePage, UnCostExpenditurePageViewModel>();
            ctReg.RegisterForNavigation<UnSalePage, UnSalePageViewModel>();

            #endregion

            #region //查看单据

            //查看单据
            ctReg.RegisterForNavigation<ViewBillPage, ViewBillPageViewModel>();
            ctReg.RegisterForNavigation<AllocationPage, AllocationPageViewModel>();
            ctReg.RegisterForNavigation<SalePage, SalePageViewModel>();
            ctReg.RegisterForNavigation<SaleOrderPage, SaleOrderPageViewModel>();
            ctReg.RegisterForNavigation<AdvanceReceiptPage, AdvanceReceiptPageViewModel>();
            ctReg.RegisterForNavigation<CostContractPage, CostContractPageViewModel>();
            ctReg.RegisterForNavigation<CashReceiptPage, CashReceiptPageViewModel>();
            ctReg.RegisterForNavigation<CostExpenditurePage, CostExpenditurePageViewModel>();
            ctReg.RegisterForNavigation<InventoryPage, InventoryPageViewModel>();
            ctReg.RegisterForNavigation<InventoryReportPage, InventoryReportPageViewModel>();
            ctReg.RegisterForNavigation<PurchasePage, PurchasePageViewModel>();
            ctReg.RegisterForNavigation<ReturnOrderPage, ReturnOrderPageViewModel>();
            ctReg.RegisterForNavigation<ReturnPage, ReturnPageViewModel>();

            #endregion

            #region //汇总单据

            //单据汇总
            ctReg.RegisterForNavigation<BillSummaryPage, BillSummaryPageViewModel>();
            ctReg.RegisterForNavigation<AllocationSummeryPage, AllocationSummeryPageViewModel>();
            ctReg.RegisterForNavigation<SaleSummeryPage, SaleSummeryPageViewModel>();
            ctReg.RegisterForNavigation<SaleOrderSummeryPage, SaleOrderSummeryPageViewModel>();
            ctReg.RegisterForNavigation<AdvanceReceiptSummeryPage, AdvanceReceiptSummeryPageViewModel>();
            ctReg.RegisterForNavigation<CostContractSummeryPage, CostContractSummeryPageViewModel>();
            ctReg.RegisterForNavigation<CashReceiptSummeryPage, CashReceiptSummeryPageViewModel>();
            ctReg.RegisterForNavigation<CostExpenditureSummeryPage, CostExpenditureSummeryPageViewModel>();
            ctReg.RegisterForNavigation<InventorySummeryPage, InventorySummeryPageViewModel>();
            ctReg.RegisterForNavigation<PurchaseSummeryPage, PurchaseSummeryPageViewModel>();
            ctReg.RegisterForNavigation<ReturnOrderSummeryPage, ReturnOrderSummeryPageViewModel>();
            ctReg.RegisterForNavigation<ReturnSummeryPage, ReturnSummeryPageViewModel>();

            #endregion

            #region //注册单据

            ctReg.RegisterForNavigation<AdvanceReceiptBillPage, AdvanceReceiptBillPageViewModel>();
            ctReg.RegisterForNavigation<CostContractBillPage, CostContractBillPageViewModel>();
            ctReg.RegisterForNavigation<AddInventoryProductPage, AddInventoryProductPageViewModel>();
            ctReg.RegisterForNavigation<InventoryOPBillPage, InventoryOPBillPageViewModel>();

            ctReg.RegisterForNavigation<ExchangeBillPage, ExchangeBillPageViewModel>();
            ctReg.RegisterForNavigation<SaleOrderBillPage, SaleOrderBillPageViewModel>();
            ctReg.RegisterForNavigation<SaleBillPage, SaleBillPageViewModel>();
            ctReg.RegisterForNavigation<ReturnOrderBillPage, ReturnOrderBillPageViewModel>();
            ctReg.RegisterForNavigation<ReturnBillPage, ReturnBillPageViewModel>();
            ctReg.RegisterForNavigation<AddBackStockBillPage, AddBackStockBillPageViewModel>();
            ctReg.RegisterForNavigation<AllocationBillPage, AllocationBillPageViewModel>();
            ctReg.RegisterForNavigation<TrackAllocationBillPage, TrackAllocationBillPageViewModel>();
            ctReg.RegisterForNavigation<BackStockBillPage, BackStockBillPageViewModel>();
            ctReg.RegisterForNavigation<ReceiptBillPage, ReceiptBillPageViewModel>();
            ctReg.RegisterForNavigation<CostExpenditureBillPage, CostExpenditureBillPageViewModel>();
            ctReg.RegisterForNavigation<PurchaseOrderBillPage, PurchaseOrderBillPageViewModel>();
            ctReg.RegisterForNavigation<InventoryBillPage, InventoryBillPageViewModel>();

            #endregion

            #region //注册ViewModel 

            ctReg.RegisterForNavigation<AdvancedPage, AdvancedPageViewModel>();
            ctReg.RegisterForNavigation<SystemSettingPage, SystemSettingPageViewModel>();
            ctReg.RegisterForNavigation<FeedbackPage, FeedbackPageViewModel>();
            ctReg.RegisterForNavigation<MarketFeedbackPage, MarketFeedbackPageViewModel>();
            ctReg.RegisterForNavigation<ConversationsPage, ConversationsPageViewModel>();
            ctReg.RegisterForNavigation<MessagerPage, MessagerPageViewModel>();
            ctReg.RegisterForNavigation<ImageViewerPage, ImageViewerPageViewModel>();
            ctReg.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            ctReg.RegisterForNavigation<AdvanceReceiptPage, AdvanceReceiptPageViewModel>();
            ctReg.RegisterForNavigation<CostContractPage, CostContractPageViewModel>();
            ctReg.RegisterForNavigation<AddCostPage, AddCostPageViewModel>();
            ctReg.RegisterForNavigation<AddCostContractProductPage, AddCostContractProductPageViewModel>();

            ctReg.RegisterForNavigation<SelectLocationPage, SelectLocationPageViewModel>();
            ctReg.RegisterForNavigation<AddAppPage, AddAppPageViewModel>();
            ctReg.RegisterForNavigation<ScanBarcodePage, ScanBarcodePageViewModel>();
            ctReg.RegisterForNavigation<ScanLoginPage, ScanLoginPageViewModel>();
            ctReg.RegisterForNavigation<CopyrightPage, CopyrightPageViewModel>();
            ctReg.RegisterForNavigation<SelectContractPage, SelectContractPageViewModel>();
            ctReg.RegisterForNavigation<AddSubscribePage, AddSubscribePageViewModel>();
            ctReg.RegisterForNavigation<SelectUserPage, SelectUserPageViewModel>();
            ctReg.RegisterForNavigation<CurrentCustomerPage, CurrentCustomerPageViewModel>();
            ctReg.RegisterForNavigation<VisitingRatePage, VisitingRatePageViewModel>();
            ctReg.RegisterForNavigation<SalesRatePage, SalesRatePageViewModel>();
            ctReg.RegisterForNavigation<NewCustomersPage, NewCustomersPageViewModel>();
            ctReg.RegisterForNavigation<NewOrderPage, NewOrderPageViewModel>();
            ctReg.RegisterForNavigation<CustomerArchivesPage, CustomerArchivesPageViewModel>();
            ctReg.RegisterForNavigation<VisitStorePage, VisitStorePageViewModel>();
            ctReg.RegisterForNavigation<RedeemPage, RedeemPageViewModel>();
            ctReg.RegisterForNavigation<RedeemedPage, RedeemedPageViewModel>();
            ctReg.RegisterForNavigation<ViewRedeemPage, ViewRedeemPageViewModel>();


            ctReg.RegisterForNavigation<CustomerRankingPage, CustomerRankingPageViewModel>();
            ctReg.RegisterForNavigation<SalesRankingPage, SalesRankingPageViewModel>();
            ctReg.RegisterForNavigation<BrandRankingPage, BrandRankingPageViewModel>();
            ctReg.RegisterForNavigation<HotSalesRankingPage, HotSalesRankingPageViewModel>();
            ctReg.RegisterForNavigation<SaleTrendChatPage, SaleTrendChatPageViewModel>();
            ctReg.RegisterForNavigation<StockQueryPage, StockQueryPageViewModel>();
            ctReg.RegisterForNavigation<UnsalablePage, UnsalablePageViewModel>();
            ctReg.RegisterForNavigation<ReceivablesPage, ReceivablesPageViewModel>();
            ctReg.RegisterForNavigation<SalesProfitRankingPage, SalesProfitRankingPageViewModel>();
            ctReg.RegisterForNavigation<CustomerVisitRankPage, CustomerVisitRankPageViewModel>();
            ctReg.RegisterForNavigation<ViewSubscribePage, ViewSubscribePageViewModel>();
            ctReg.RegisterForNavigation<AddProductArchivePage, AddProductArchivePageViewModel>();
            ctReg.RegisterForNavigation<AddReportProductPage, AddReportProductPageViewModel>();
            ctReg.RegisterForNavigation<AddCustomerPage, AddCustomerPageViewModel>();
            ctReg.RegisterForNavigation<CustomerActivityPage, CustomerActivityPageViewModel>();
            ctReg.RegisterForNavigation<HotOrderRankingPage, HotOrderRankingPageViewModel>();
            ctReg.RegisterForNavigation<OrderQuantityAnalysisPage, OrderQuantityAnalysisPageViewModel>();
            ctReg.RegisterForNavigation<MyReportingPage, MyReportingPageViewModel>();
            ctReg.RegisterForNavigation<AddReportPage, AddReportPageViewModel>();
            ctReg.RegisterForNavigation<VisitReportPage, VisitReportPageViewModel>();


            //对账
            ctReg.RegisterForNavigation<ReconciliationProductsPage, ReconciliationProductsPageViewModel>();
            ctReg.RegisterForNavigation<ReconciliationDetailPage, ReconciliationDetailPageViewModel>();
            ctReg.RegisterForNavigation<ReconciliationHistoryPage, ReconciliationHistoryPageViewModel>();
            ctReg.RegisterForNavigation<ReconciliationORreceivablesPage, ReconciliationORreceivablesPageViewModel>();

            //ctReg.RegisterForNavigation<FieldTrackPage, FieldTrackPageViewModel>();
            ctReg.RegisterForNavigation<VisitRecordsPage, VisitRecordsPageViewModel>();
            ctReg.RegisterForNavigation<ProductArchivesPage, ProductArchivesPageViewModel>();

            //公共
            ctReg.RegisterForNavigation<SelectAllocationProductPage, SelectAllocationProductPageViewModel>();
            ctReg.RegisterForNavigation<EditProductPage, EditProductPageViewModel>();
            ctReg.RegisterForNavigation<EditAllocationProductPage, EditAllocationProductPageViewModel>();
            ctReg.RegisterForNavigation<SelectCustomerPage, SelectCustomerPageViewModel>();
            ctReg.RegisterForNavigation<SelectAreaPage, SelectAreaPageViewModel>();
            ctReg.RegisterForNavigation<SelectProductPage, SelectProductPageViewModel>();
            ctReg.RegisterForNavigation<SelectGiftsPage, SelectGiftsPageViewModel>();
            ctReg.RegisterForNavigation<AddGiftProductPage, AddGiftProductPageViewModel>();

            ctReg.RegisterForNavigation<AddExchangeProductPage, AddExchangeProductPageViewModel>();
            ctReg.RegisterForNavigation<AddProductPage, AddProductPageViewModel>();
            ctReg.RegisterForNavigation<AddPurchaseProductPage, AddPurchaseProductPageViewModel>();
            ctReg.RegisterForNavigation<PaymentMethodPage, PaymentMethodPageViewModel>();
            ctReg.RegisterForNavigation<MorePaymentPage, MorePaymentPageViewModel>();
            ctReg.RegisterForNavigation<FilterPage, FilterPageViewModel>();
            ctReg.RegisterForNavigation<AddAllocationProductPage, AddAllocationProductPageViewModel>();


            //资料
            ctReg.RegisterForNavigation<MyInfoPage, MyInfoPageViewModel>();
            ctReg.RegisterForNavigation<SecurityPage, SecurityPageViewModel>();
            ctReg.RegisterForNavigation<PrintSettingPage, PrintSettingPageViewModel>();
            ctReg.RegisterForNavigation<AboutPage, AboutPageViewModel>();
            ctReg.RegisterForNavigation<ResetPasswordPage, ResetPasswordPageViewModel>();
            ctReg.RegisterForNavigation<AgreementPage, AgreementPageViewModel>();
            ctReg.RegisterForNavigation<UpdatePage, UpdatePageViewModel>();
            ctReg.RegisterForNavigation<NewsViewerPage, NewsViewerPageViewModel>();
            ctReg.RegisterForNavigation<SelectManufacturerPage, SelectManufacturerPageViewModel>();

            //商品销售明细
            ctReg.RegisterForNavigation<SaleDetailPage, SaleDetailPageViewModel>();

            #endregion

            #region  //服务相关

            // Network
            ctReg.Register<MakeRequest, MakeRequest>();
            ctReg.Register<ICrashlyticsService, CrashlyticsService>();

            //Common
            //ctReg.RegisterSingleton<IPagedialogService PageDialogService>();
            ctReg.RegisterSingleton<IDialogKit, DialogKit>();
            ctReg.RegisterSingleton<IAuthenticationService, AuthenticationService>();
            ctReg.RegisterSingleton<IGlobalService, GlobalService>();

            //QA
            ctReg.RegisterSingleton<IConversationsDataStore, ConversationsDataStore>();
            ctReg.RegisterSingleton<IUserDataStores, UserDataStores>();
            ctReg.RegisterSingleton<IMessagesDataStore, MessagesDataStore>();
            ctReg.RegisterSingleton<IFeedbackService, FeedbackService>();
            ctReg.RegisterSingleton<IQueuedMessageService, QueuedMessageService>();
            
            //Setting
            ctReg.RegisterSingleton<IAccountingService, AccountingService>();
            ctReg.RegisterSingleton<ISettingService, SettingService>();
            ctReg.RegisterSingleton<IUpdateService, UpdateService>();


            //DataServices
            ctReg.RegisterSingleton<INewsService, NewsService>();
            ctReg.RegisterSingleton<IUserService, UserService>();
            ctReg.RegisterSingleton<IManufacturerService, ManufacturerService>();
            ctReg.RegisterSingleton<IProductService, ProductService>();
            ctReg.RegisterSingleton<IWareHousesService, WareHousesService>();
            ctReg.RegisterSingleton<ICampaignService, CampaignService>();
            ctReg.RegisterSingleton<ITerminalService, TerminalService>();
            ctReg.RegisterSingleton<IPurchaseBillService, PurchaseBillService>();
            ctReg.RegisterSingleton<ISaleBillService, SaleBillService>();
            ctReg.RegisterSingleton<IExchangeBillService, ExchangeBillService>();

            ctReg.RegisterSingleton<ISaleReservationBillService, SaleReservationBillService>();
            ctReg.RegisterSingleton<IReturnBillService, ReturnBillService>();
            ctReg.RegisterSingleton<IReturnReservationBillService, ReturnReservationBillService>();
            ctReg.RegisterSingleton<IReportingService, ReportingService>();
            ctReg.RegisterSingleton<ICostExpenditureService, CostExpenditureService>();
            ctReg.RegisterSingleton<IInventoryService, InventoryService>();
            ctReg.RegisterSingleton<ICostContractService, CostContractService>();
            ctReg.RegisterSingleton<IAdvanceReceiptService, AdvanceReceiptService>();
            ctReg.RegisterSingleton<IAllocationService, AllocationService>();
            ctReg.RegisterSingleton<IReceiptCashService, ReceiptCashService>();
            ctReg.RegisterSingleton<IFinanceReceiveAccountService, FinanceReceiveAccountService>();

            #endregion

            _appContainer = ctReg.GetContainer();
        }


        #endregion

        protected override IContainerExtension CreateContainerExtension()
        {
            var container = new UnityContainer();

            //注册ILogger<T>
            container.AddExtension(new LoggingExtension());


            //本地存储服务
            //注意：（ LiteDbAsyncService 必须注册单例）
            container.RegisterSingleton<ILiteDbAsyncService, LiteDbAsyncService>();
            container.RegisterType(typeof(ILiteDbService<TrackingModel>), typeof(LiteDbService<TrackingModel>));
            container.RegisterType(typeof(ILiteDbService<VisitStore>), typeof(LiteDbService<VisitStore>));
            container.RegisterType(typeof(ILiteDbService<NotificationEvent>), typeof(LiteDbService<NotificationEvent>));
            container.RegisterType(typeof(ILiteDbService<MessageInfo>), typeof(LiteDbService<MessageInfo>));
            container.RegisterType(typeof(ILiteDbService<CacheBillData>), typeof(LiteDbService<CacheBillData>));
            container.RegisterType(typeof(ILiteDbService<CachePaymentMethod>), typeof(LiteDbService<CachePaymentMethod>));
            container.RegisterType(typeof(ILiteDbService<PushEvent>), typeof(LiteDbService<PushEvent>));
            container.RegisterType(typeof(ILiteDbService<ProductModel>), typeof(LiteDbService<ProductModel>));
            container.RegisterType(typeof(ILiteDbService<TerminalModel>), typeof(LiteDbService<TerminalModel>));

            return new UnityContainerExtension(container);
        }


        /// <summary>
        /// 解析服务，你可以直接使用App.Resolve<T>
        /// </summary>
        /// <typeparam name="T">interface</typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            try
            {
                T t = default;

                if (_appContainer != null)
                    t = _appContainer.Resolve<T>();

                return t;
            }
            catch (Exception)
            {
                return default;
            }
        }

    }
}
