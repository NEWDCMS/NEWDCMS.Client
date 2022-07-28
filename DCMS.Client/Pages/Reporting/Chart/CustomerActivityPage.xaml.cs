using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;

namespace Wesley.Client.Pages.Archive
{
    public partial class CustomerActivityPage : BaseContentPage<CustomerActivityPageViewModel>
    {
        public CustomerActivityPage()
        {
            try
            {
                InitializeComponent();

                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems6(ViewModel, ("Filter",
                    new Models.FilterModel()
                    {
                        BusinessUserEnable = true,
                        DistrictEnable = true,
                        TerminalEnable = true
                    })).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
