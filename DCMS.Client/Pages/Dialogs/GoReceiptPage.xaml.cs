using Wesley.Client.Services;
using Wesley.Client.ViewModels;
using Prism.Navigation;


namespace Wesley.Client.Pages
{
    public partial class GoReceiptPage : BasePopupPage<GoReceiptPageViewModel>
    {
        public GoReceiptPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!(this.BindingContext is GoReceiptPageViewModel))
            {
                var navigationService = App.Resolve<INavigationService>();
                var dialogService = App.Resolve<IDialogService>();
                //var connectivity = App.Resolve<IConnectivity>();
                GoReceiptPageViewModel vm = new GoReceiptPageViewModel(navigationService, dialogService);
                this.ViewModel = vm;
            }
        }
    }
}