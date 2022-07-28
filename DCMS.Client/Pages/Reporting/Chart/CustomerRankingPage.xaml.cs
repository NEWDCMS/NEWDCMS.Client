using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
namespace Wesley.Client.Pages.Reporting
{

    public partial class CustomerRankingPage : BaseContentPage<CustomerRankingPageViewModel>
    {
        public CustomerRankingPage()
        {
            try
            {
                InitializeComponent();

                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems(ViewModel, ("Filter", new Models.FilterModel() { BusinessUserEnable = true, DistrictEnable = true, TerminalEnable = true })).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


    }
}
