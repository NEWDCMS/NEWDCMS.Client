using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Reporting
{

    public partial class ReconciliationHistoryPage : BaseContentPage<ReconciliationHistoryPageViewModel>
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
                        try
                        {
                            InitializeComponent();
                        }
                        catch (Exception ex) { Crashes.TrackError(ex); }

                        ToolbarItems.Clear();
                        foreach (var toolBarItem in this.GetToolBarItems(ViewModel, ("Filter", new Models.FilterModel() { BrandEnable = true })).ToList())
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
