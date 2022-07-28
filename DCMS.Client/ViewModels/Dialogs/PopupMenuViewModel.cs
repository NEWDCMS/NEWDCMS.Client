using DCMS.Client.Services;
using Prism.Navigation;


namespace DCMS.Client.ViewModels
{
    public class PopupMenuViewModel : ViewModelBase
    {
        public PopupMenuViewModel(INavigationService navigationService,
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
