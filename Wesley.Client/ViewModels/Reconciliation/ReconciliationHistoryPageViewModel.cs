using Wesley.Client.Services;

using Prism.Navigation;

namespace Wesley.Client.ViewModels
{
    public class ReconciliationHistoryPageViewModel : ViewModelBase
    {
        public ReconciliationHistoryPageViewModel(INavigationService navigationService, IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "收款上交记录";

        }

        public override void OnActiveTabChangedAsync()
        {
            if (IsActive)
            {
                //绑定数据
                IsActive = false;
            }
        }
    }
}
