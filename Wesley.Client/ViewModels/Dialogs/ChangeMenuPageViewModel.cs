using Wesley.Client.Services;
using Prism.Navigation;
using ReactiveUI;


namespace Wesley.Client.ViewModels
{
    public class ChangeMenuPageViewModel : ViewModelBase
    {
        public IReactiveCommand AddAPPCommand => this.Navigate("AddAppPage");
        public IReactiveCommand ScanCommand => this.Navigate("ScanBarcodePage", ("action", "add"));

        public ChangeMenuPageViewModel(INavigationService navigationService,
            IDialogService dialogService

            ) : base(navigationService, dialogService)
        {

        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
