using Wesley.Client.CustomViews;
using Wesley.Client.Services;

using Prism.Navigation;
using ReactiveUI.Fody.Helpers;

namespace Wesley.Client.ViewModels
{
    public class TopTabMessagesPageViewModel : ViewModelBase, IMyTabbedPageSelectedTab
    {
        [Reactive] public int SelectedTab { get; set; }

        public TopTabMessagesPageViewModel(INavigationService navigationService,
              IDialogService dialogService) : base(navigationService, dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            this.ExceptionsSubscribe();
        }

        public override void OnActiveTabChangedAsync()
        {
            base.OnActiveTabChangedAsync();
        }

        public void SetSelectedTab(int tabIndex)
        {
            SelectedTab = tabIndex;
        }
    }
}
