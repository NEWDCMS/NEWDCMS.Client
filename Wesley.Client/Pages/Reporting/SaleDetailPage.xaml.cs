using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
namespace Wesley.Client.Pages.Reporting
{

    public partial class SaleDetailPage : BaseContentPage<SaleDetailPageViewModel>
    {
        public SaleDetailPage()
        {
            try
            {
                InitializeComponent();
                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems6(ViewModel, ("Filter", new Models.FilterModel()
                {
                    DistrictEnable = true,
                    BusinessUserEnable = true,
                    StartTimeEnable = true,
                    EndTimeEnable = true
                })).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }

        }
    }
}
