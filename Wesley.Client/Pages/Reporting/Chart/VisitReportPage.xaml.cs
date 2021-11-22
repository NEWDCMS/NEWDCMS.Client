using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
namespace Wesley.Client.Pages.Reporting
{

    public partial class VisitReportPage : BaseContentPage<VisitReportPageViewModel>
    {
        public VisitReportPage()
        {
            try
            {
                InitializeComponent();
                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems5(ViewModel).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }

        }
    }
}
