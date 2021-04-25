using Wesley.Client.CustomViews;
using Wesley.Client.Services;

using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
namespace Wesley.Client.ViewModels
{
    public class ViewBillPageViewModel : ViewModelBase, IMyTabbedPageSelectedTab
    {
        [Reactive] public int SelectedTab { get; set; }

        public ViewBillPageViewModel(INavigationService navigationService,
              IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "查看单据";
            _navigationService = navigationService;
            _dialogService = dialogService;

            //菜单选择
            this.SetMenus((x) =>
            {

            }, 8, 9, 14, 21);
        }

        public void SetSelectedTab(int tabIndex)
        {
            SelectedTab = tabIndex;
            //_dialogService.ShortAlert($"SelectedTab:{tabIndex}");
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            //if (parameters.ContainsKey("SelectedTab"))
            //{
            //    //parameters.TryGetValue<int>("SelectedTab", out int SelectedTab);
            //}
        }
    }
}
