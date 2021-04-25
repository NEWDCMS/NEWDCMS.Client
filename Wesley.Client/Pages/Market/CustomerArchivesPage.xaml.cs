using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Market
{

    public partial class CustomerArchivesPage : BaseContentPage<CustomerArchivesPageViewModel>
    {
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Content == null)
            {
                Device.StartTimer(TimeSpan.FromSeconds(0), () =>
                {
                    try
                    {
                        InitializeComponent();
                        ToolbarItems.Clear();
                        foreach (var toolBarItem in this.GetToolBarItems4<CustomerArchivesPageViewModel>(ViewModel).ToList())
                        {
                            ToolbarItems.Add(toolBarItem);
                        }
                    }
                    catch (Exception ex) { Crashes.TrackError(ex); }
                    return false;
                });
                return;
            }

        }
    }
}
