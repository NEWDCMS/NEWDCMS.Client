using Wesley.Client.CustomViews;
using Wesley.Client.Services;

using Prism.Navigation;
using ReactiveUI.Fody.Helpers;

namespace Wesley.Client.ViewModels
{
    /// <summary>
    /// 单据签收主体
    /// </summary>
    public class DeliveryReceiptPageViewModel : ViewModelBase, IMyTabbedPageSelectedTab
    {
        [Reactive] public int SelectedTab { get; set; }
        [Reactive] public string PageTitle1 { get; set; } = "订单";
        [Reactive] public string PageTitle2 { get; set; } = "销单";
        [Reactive] public string PageTitle3 { get; set; } = "费用";
        [Reactive] public string PageTitle4 { get; set; } = "已签";

        public DeliveryReceiptPageViewModel(INavigationService navigationService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "单据签收";
        }

        public void SetSelectedTab(int tabIndex)
        {
            SelectedTab = tabIndex;
        }
    }
}
