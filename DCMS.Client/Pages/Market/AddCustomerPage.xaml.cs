using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
namespace Wesley.Client.Pages.Common
{

    public partial class AddCustomerPage : BaseContentPage<AddCustomerPageViewModel>
    {
        public AddCustomerPage()
        {
            try
            {
                InitializeComponent();
                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems<AddCustomerPageViewModel>(ViewModel).ToList())
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
