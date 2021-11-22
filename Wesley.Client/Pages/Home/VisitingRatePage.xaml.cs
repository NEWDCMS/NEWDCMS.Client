using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;

namespace Wesley.Client.Pages.Home
{
    public partial class VisitingRatePage : BaseContentPage<VisitingRatePageViewModel>
    {
        public VisitingRatePage()
        {
            try
            {
                InitializeComponent();
                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems6(ViewModel, ("Filter", new Models.FilterModel() { BusinessUserEnable = true })).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }


    }
}
