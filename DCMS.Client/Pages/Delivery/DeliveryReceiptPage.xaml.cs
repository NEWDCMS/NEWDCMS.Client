using Wesley.Client.Models;
using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace Wesley.Client.Pages.Market
{

    public partial class DeliveryReceiptPage : BaseTabbedPage<DeliveryReceiptPageViewModel>
    {
        public DeliveryReceiptPage()
        {
            try
            {
                InitializeComponent();

                //this.SlideMenu = new RightSideMasterPage();
                //if (ViewModel == null)
                //    ViewModel = (DeliveryReceiptPageViewModel)this.BindingContext;

                //string currentPage = "";
                //try
                //{
                //    currentPage = this.CurrentPage.GetType().GenericTypeArguments[1].ToString();
                //}
                //catch (Exception)
                //{
                //}

                //this.SetTabsToolBarItems(ViewModel, new NavigationParameters()
                //{
                //    { "Reference", "Tabbedpage" }
                //}, currentPage, false);
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
                if (b.ClassId == "POP")
                {
                    b.Command = new Command(() =>
                    {
                        if (ViewModel != null)
                        {
                            string key = string.Format("{0}_SELECTEDTAB_{1}", currentPage.ToUpper(), SelectedTabIndex);
                            //((RightSideMasterPage)this.SlideMenu).SetBindMenus(ViewModel.BindMenus, key);
                            //this.ShowMenu();
                        }
                    });
                }
                else if (b.ClassId == "Filter")
                {

                    b.Command = new Command(async () =>
                   {
                       if (ViewModel != null)
                       {
                           await ViewModel.NavigateAsync("FilterPage",
                               ("Reference", "Tabbedpage"),
                               ("Filter", new FilterModel()
                               {
                                   BusinessUserEnable = true,
                                   TerminalEnable = true,
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
                if (CurrentContentView.Children.Any() && CurrentContentView.Children.Count > ViewModel.SelectedTab)
                    CurrentContentView.CurrentPage = CurrentContentView.Children[ViewModel.SelectedTab];
            }
        }

    }
}
