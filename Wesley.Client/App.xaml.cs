using Wesley.Client.CustomViews;
using Wesley.Client.CustomViews.Views;
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

using Prism.Plugin.Popups;
using Prism.Services;
using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Shiny;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.DryIoc;
using DryIoc;
using Prism;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
[assembly: ExportFont("fa-solid-900.otf", Alias = "FAS")]
[assembly: ExportFont("fa-regular-400.ttf", Alias = "FAR")]
[assembly: ExportFont("fa-brands-400.ttf", Alias = "FAB")]
namespace Wesley.Client
{

    public partial class App : PrismApplication
    {
        public static IContainer AppContainer { get; set; }

        public static bool IsInBackgrounded { get; private set; }

        public App(IPlatformInitializer initializer) : base(initializer)
        {

        }

        #region override

        protected async override void OnInitialized()
        {
            try
            {
                Settings.IsInitData = false;
                Settings.IsNextTimeUpdate = false;

                InitializeComponent();

                if (!string.IsNullOrEmpty(Settings.AccessToken))
                    await this.NavigationService.NavigateAsync($"NavigationPage/{nameof(MainLayoutPage)}");
                else
                    await this.NavigationService.NavigateAsync($"NavigationPage/{nameof(LoginPage)}");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        protected override void OnStart()
        {
            base.OnStart();
            GlobalSettings.IsSleeping = false;
            AppCenter.Start("android=03804b2b-3759-4bab-9286-ae78dcd60abe;", typeof(Analytics), typeof(Crashes));
        }

        protected override void OnResume()
        {
            base.OnResume();
            App.IsInBackgrounded = false;
        }
        protected override void OnSleep()
        {
            // Handle when your app sleeps
            base.OnSleep();
            GlobalSettings.IsSleeping = true;
            App.IsInBackgrounded = true;
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

        /// <summary>
        /// 注册依赖容器
        /// </summary>
        /// <param name="ctr"></param>
        protected override void RegisterTypes(IContainerRegistry ctReg)
        {
            #region  //注册导航

            ctReg.RegisterForNavigation<NavigationPage>();
            ctReg.RegisterForNavigation<MainLayoutPage, MainLayoutPageViewModel>();
            ctReg.RegisterForNavigation<BillSummaryPage>();
            ctReg.RegisterForNavigation<LoginPage>();
            ctReg.RegisterForNavigation<SelectCustomerPage>();
            ctReg.RegisterForNavigation<DeliveryReceiptPage>();
            ctReg.RegisterForNavigation<NewsPage>();
            ctReg.RegisterForNavigation<ViewSubscribePage>();

            //注册对话框 
            ctReg.RegisterPopupNavigationService();
            ctReg.RegisterPopupDialogService();
            ctReg.RegisterForNavigation<PopCheckBoxView, PopCheckBoxViewModel>();
            ctReg.RegisterForNavigation<PopRadioButtonView, PopRadioButtonViewModel>();

            #endregion

            #region //注册Page

            ctReg.Register<Toolbar>();
            ctReg.Register<MainLayoutPage>();
            ctReg.Register<AddAppPage>();
            ctReg.Register<SaleOrderBillPage>();
            ctReg.Register<ReturnOrderBillPage>();
            ctReg.Register<ReturnBillPage>();
            ctReg.Register<StockQueryPage>();
            ctReg.Register<AllocationBillPage>();
            ctReg.Register<TrackAllocationBillPage>();
            ctReg.Register<AddBackStockBillPage>();
            ctReg.Register<PurchaseOrderBillPage>();

            #endregion

            #region //注册主框架布局

            ctReg.Register<IMyTabbedPageSelectedTab, TopTabMessagesPageViewModel>();

   
            //ctReg.RegisterForNavigation<MainLayoutPage, HomePageViewModel>();
            //ctReg.RegisterForNavigation<MainLayoutPage, AllfunPageViewModel>();
            //ctReg.RegisterForNavigation<MainLayoutPage, NotificationsPageViewModel>();
            //ctReg.RegisterForNavigation<MainLayoutPage, ProfilePageViewModel>();
            //ctReg.RegisterForNavigation<MainLayoutPage, MessagesPageViewModel>();

            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, HomeView>, HomePageViewModel>(nameof(HomeView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, MessagesView>, MessagesPageViewModel>(nameof(MessagesView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, AllfunView>, AllfunPageViewModel>(nameof(AllfunView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, NotificationsView>, NotificationsPageViewModel>(nameof(NotificationsView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, ProfileView>, ProfilePageViewModel>(nameof(ProfileView));
          

            //ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, ContentView>, MessagesPageViewModel>(nameof(ContentView));
            ctReg.RegisterForNavigation<NewsPage, NewsPageViewModel>();


            #endregion

            #region //注册签收布局

            ctReg.RegisterForNavigation<DeliveryReceiptPage, DeliveryReceiptPageViewModel>();
            //已签收
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, DeliveriedView>, DeliveriedPageViewModel>(nameof(DeliveriedView));
            //未签收
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, UnOrderView>, UnOrderPageViewModel>(nameof(UnOrderView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, UnCostExpenditureView>, UnCostExpenditurePageViewModel>(nameof(UnCostExpenditureView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, UnSaleView>, UnSalePageViewModel>(nameof(UnSaleView));

            #endregion

            #region //查看单据

            //查看单据 Master
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

            //查看单据（异步视图）
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, SaleView>, SalePageViewModel>(nameof(SaleView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, SaleOrderView>, SaleOrderPageViewModel>(nameof(SaleOrderView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, AdvanceReceiptView>, AdvanceReceiptPageViewModel>(nameof(AdvanceReceiptView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, AllocationView>, AllocationPageViewModel>(nameof(AllocationView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, CostContractView>, CostContractPageViewModel>(nameof(CostContractView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, CashReceiptView>, CashReceiptPageViewModel>(nameof(CashReceiptView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, InventoryView>, InventoryPageViewModel>(nameof(InventoryView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, InventoryReportView>, InventoryReportViewPageViewModel>(nameof(InventoryReportView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, PurchaseView>, PurchasePageViewModel>(nameof(PurchaseView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, ReturnOrderView>, ReturnOrderPageViewModel>(nameof(ReturnOrderView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, ReturnView>, ReturnPageViewModel>(nameof(ReturnView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, CostExpenditureView>, CostExpenditurePageViewModel>(nameof(CostExpenditureView));

            #endregion

            #region //汇总单据

            //单据汇总 Master
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

            //汇总单据（异步视图）
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, SaleSummeryView>, SaleSummeryPageViewModel>(nameof(SaleSummeryView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, SaleOrderSummeryView>, SaleOrderSummeryPageViewModel>(nameof(SaleOrderSummeryView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, AdvanceReceiptSummeryView>, AdvanceReceiptSummeryPageViewModel>(nameof(AdvanceReceiptSummeryView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, AllocationSummeryView>, AllocationSummeryPageViewModel>(nameof(AllocationSummeryView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, CostContractSummeryView>, CostContractSummeryPageViewModel>(nameof(CostContractSummeryView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, CashReceiptSummeryView>, CashReceiptSummeryPageViewModel>(nameof(CashReceiptSummeryView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, InventorySummeryView>, InventorySummeryPageViewModel>(nameof(InventorySummeryView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, PurchaseSummeryView>, PurchaseSummeryPageViewModel>(nameof(PurchaseSummeryView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, ReturnOrderSummeryView>, ReturnOrderSummeryPageViewModel>(nameof(ReturnOrderSummeryView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, ReturnSummeryView>, ReturnSummeryPageViewModel>(nameof(ReturnSummeryView));
            ctReg.RegisterForNavigation<LazyContentPage<LoadingContentView, CostExpenditureSummeryView>, CostExpenditureSummeryPageViewModel>(nameof(CostExpenditureSummeryView));

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

            ctReg.RegisterForNavigation<SystemSettingPage, SystemSettingPageViewModel>();
            ctReg.RegisterForNavigation<FeedbackPage, FeedbackPageViewModel>();
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


            #endregion

            #region  //服务相关

            // Network
            ctReg.Register<MakeRequest, MakeRequest>();
            ctReg.Register<ICrashlyticsService, CrashlyticsService>();

            //Common
            ctReg.RegisterSingleton<IPageDialogService, PageDialogService>();
            ctReg.RegisterSingleton<IAuthenticationService, AuthenticationService>();
            ctReg.RegisterSingleton<IGlobalService, GlobalService>();

            //QA
            ctReg.RegisterSingleton<IConversationsDataStore, ConversationsDataStore>();
            ctReg.RegisterSingleton<IUserDataStores, UserDataStores>();
            ctReg.RegisterSingleton<IMessagesDataStore, MessagesDataStore>();
            ctReg.RegisterSingleton<IFeedbackService, FeedbackService>();


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

            AppContainer = ctReg.GetContainer();

        }

        #endregion

        protected override IContainerExtension CreateContainerExtension()
        {
            var container = new Container(this.CreateContainerRules());
            ShinyHost.Populate((serviceType, func, lifetime) =>
                container.RegisterDelegate(
                    serviceType,
                    _ => func(),
                    Reuse.Singleton
                )
            );
            return new DryIocContainerExtension(container);
        }

        public static T Resolve<T>()
        {
            try
            {
                T t = default;

                if (AppContainer != null)
                    t = AppContainer.Resolve<T>();
                else
                    t = ShinyHost.Resolve<T>() ?? default;

                return t;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
