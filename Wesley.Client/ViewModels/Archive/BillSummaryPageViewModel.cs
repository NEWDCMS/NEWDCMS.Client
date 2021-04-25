using Wesley.Client.CustomViews;
using Wesley.Client.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Linq;
using System.Reactive.Linq;

namespace Wesley.Client.ViewModels
{

    public class BillSummaryPageViewModel : ViewModelBase, IMyTabbedPageSelectedTab
    {
        [Reactive] public int SelectedTab { get; set; }

        public BillSummaryPageViewModel(INavigationService navigationService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "单据汇总";

            this.WhenAnyValue(x => x.SelectedTab)
           .Skip(1)
           .SubOnMainThread(tab =>
           {
               //_dialogService.ShortAlert($"{key}");
           });

            //菜单选择
            this.SetMenus(8, 9, 14, 21);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("SelectedTab"))
            {
                //parameters.TryGetValue<int>("Product", out int SelectedTab);
                //SelectedTab = parameters.GetValue<int>("SelectedTab");
            }
        }

        public void SetSelectedTab(int tabIndex)
        {

        }
    }
}
