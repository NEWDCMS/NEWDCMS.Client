using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
namespace Wesley.Client.Pages.Market
{

    public partial class CustomerArchivesPage : BaseContentPage<CustomerArchivesPageViewModel>
    {
        public CustomerArchivesPage()
        {
            try
            {
                InitializeComponent();
                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems4<CustomerArchivesPageViewModel>(ViewModel).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
