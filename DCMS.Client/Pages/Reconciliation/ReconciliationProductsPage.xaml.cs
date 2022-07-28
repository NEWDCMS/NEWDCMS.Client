using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Reporting
{

    public partial class ReconciliationProductsPage : BaseContentPage<ReconciliationProductsPageViewModel>
    {
        public ReconciliationProductsPage()
        {
            try
            {
                InitializeComponent();
                ToolbarItems?.Clear();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }

        }

    }
}
