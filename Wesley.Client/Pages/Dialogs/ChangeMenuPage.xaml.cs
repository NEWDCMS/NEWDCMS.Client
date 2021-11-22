using Wesley.Client.Services;
using Wesley.Client.ViewModels;
using Prism.Navigation;


namespace Wesley.Client.Pages
{
    public partial class ChangeMenuPage : BasePopupPage<ChangeMenuPageViewModel>
    {
        public ChangeMenuPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!(this.BindingContext is ChangeMenuPageViewModel))
            {
                var navigationService = App.Resolve<INavigationService>();
                var dialogService = App.Resolve<IDialogService>();
                //var connectivity = App.Resolve<IConnectivity>();
                this.ViewModel = new ChangeMenuPageViewModel(navigationService, dialogService);
            }
        }
    }
}