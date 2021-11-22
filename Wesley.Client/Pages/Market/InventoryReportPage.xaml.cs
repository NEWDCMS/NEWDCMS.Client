using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
namespace Wesley.Client.Pages.Market
{

    public partial class InventoryReportPage : BaseContentPage<InventoryReportPageViewModel>
    {
        public InventoryReportPage()
        {
            try
            {
                InitializeComponent();

                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems(ViewModel, true).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }

        }
    }
}
