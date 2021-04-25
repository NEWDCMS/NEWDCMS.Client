using Wesley.Client.CustomViews;
using Wesley.Client.Models;
using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace Wesley.Client.Pages.Common
{

    public partial class MorePaymentPage : BaseContentPage<MorePaymentPageViewModel>
    {
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Content == null)
            {
                try
                {
                    InitializeComponent();

                    //var accs = new List<AccountingModel>()
                    //{
                    //    new AccountingModel{ Name="1", Number=1, ParentId=0},
                    //    new AccountingModel{ Name="1", Number=2, ParentId=1},
                    //    new AccountingModel{ Name="1",Number=2, ParentId=1},
                    //};
                    //var rootNodes = ProcessXamlItemGroups(GroupData(accs));
                    //TheTreeView.RootNodes = rootNodes;

                    MessagingCenter.Subscribe<ObservableCollection<TreeViewNode>>(this, Constants.MorePayments, (s) =>
                    {
                        TheTreeView.RootNodes = s;
                    });

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                return;
            }
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaymentListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as AccountingModel;
            if (ViewModel != null)
            {
                if (ViewModel.Accounts.Where(a => a.Selected == true).Count() > 2)
                {
                    item.Selected = false;
                    ViewModel.Alert("请确保最多支持2种支付方式");
                    return;
                }
                else
                {
                    if (item != null)
                    {
                        item.Selected = !item.Selected;
                    }
                }
            }
        }


        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<ObservableCollection<TreeViewNode>>(this, Constants.MorePayments);
            base.OnDisappearing();
        }
    }
}
