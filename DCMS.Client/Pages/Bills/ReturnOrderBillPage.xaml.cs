using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
namespace Wesley.Client.Pages.Bills
{

    public partial class ReturnOrderBillPage : BaseContentPage<ReturnOrderBillPageViewModel>
    {
        public ReturnOrderBillPage()
        {
            try
            {
                InitializeComponent();
               
                //存储记录

                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems(ViewModel, true).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
                //Refresh();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
