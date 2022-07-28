using Wesley.Client.Models;
using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace Wesley.Client.Pages.Archive
{

    public partial class BillSummaryPage : BaseTabbedPage<BillSummaryPageViewModel>
    {

        public BillSummaryPage()
        {
            try
            {
                InitializeComponent();

                string currentPage = "";
                try
                {
                    if (this.CurrentPage.GetType().GenericTypeArguments.Count() > 1)
                        currentPage = this.CurrentPage.GetType().GenericTypeArguments[1].ToString();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                this.SetTabsToolBarItems(ViewModel, currentPage, true,
                    ("Reference", "Tabbedpage"),
                    ("Filter", new FilterModel()
                    {
                        BusinessUserId = Settings.UserId,
                        BusinessUserName = Settings.UserRealName,
                        BusinessUserEnable = true,
                        TerminalEnable = true,
                        SerchKeyEnable = true,
                        SelectedTab = 0
                    }));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            SelectedTabIndex = this.Children.IndexOf(this.CurrentPage);

            if (ViewModel != null)
                ViewModel.SelectedTab = SelectedTabIndex;

            string currentPage = "";
            try
            {
                if (this.CurrentPage.GetType().GenericTypeArguments.Count() > 1)
                    currentPage = this.CurrentPage.GetType().GenericTypeArguments[1].ToString();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            //更新
            this.ToolbarItems.ToList().ForEach(b =>
            {
                if (b.ClassId == "Filter")
                {

                    b.Command = new Command(async () =>
                   {
                       if (ViewModel != null)
                       {
                           await ViewModel.NavigateAsync("FilterPage",
                                 ("Reference", "Tabbedpage"),
                                 ("Filter", new FilterModel()
                                 {
                                     BusinessUserId = Settings.UserId,
                                     BusinessUserName = Settings.UserRealName,
                                     BusinessUserEnable = true,
                                     TerminalEnable = true,
                                     SerchKeyEnable = true,
                                     SelectedTab = SelectedTabIndex
                                 }));
                       }
                   });
                }
            });

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel != null)
            {
                if (BillSummaryTabPage.Children.Any()&& BillSummaryTabPage.Children.Count > ViewModel.SelectedTab)
                    BillSummaryTabPage.CurrentPage = BillSummaryTabPage.Children[ViewModel.SelectedTab];
            }
        }
    }
}
