using Wesley.Client.Services;
using Prism.Navigation;
using ReactiveUI;


namespace Wesley.Client.ViewModels
{
    public class GoReceiptPageViewModel : ViewModelBase
    {
        public IReactiveCommand GoReceipt => this.Navigate("DeliveryReceiptPage", ("SelectedTab", "2"));
        public GoReceiptPageViewModel(INavigationService navigationService,
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
