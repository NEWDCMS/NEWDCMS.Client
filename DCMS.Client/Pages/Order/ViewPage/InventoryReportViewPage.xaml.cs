using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Order
{

    public partial class InventoryReportViewPage : BaseContentPage<InventoryReportViewPageViewModel>
    {
        public InventoryReportViewPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }
    }
}
