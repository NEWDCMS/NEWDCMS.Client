using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
namespace Wesley.Client.Pages.Reporting
{

    public partial class SalesRankingPage : BaseContentPage<SalesRankingPageViewModel>
    {
        public SalesRankingPage()
        {
            try
            {
                InitializeComponent();

                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems(ViewModel, showSubMit: false, showPrint: false).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }

        }
    }
}
