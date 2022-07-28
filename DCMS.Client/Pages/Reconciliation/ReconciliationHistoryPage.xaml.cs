using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
namespace Wesley.Client.Pages.Reporting
{

    public partial class ReconciliationHistoryPage : BaseContentPage<ReconciliationHistoryPageViewModel>
    {
        public ReconciliationHistoryPage()
        {
            try
            {
                InitializeComponent();

                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems(ViewModel, ("Filter", new Models.FilterModel() { BrandEnable = true })).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
