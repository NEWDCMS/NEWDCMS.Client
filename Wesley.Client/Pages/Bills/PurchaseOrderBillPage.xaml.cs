using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
namespace Wesley.Client.Pages.Archive
{

    public partial class PurchaseOrderBillPage : BaseContentPage<PurchaseOrderBillPageViewModel>
    {
        public PurchaseOrderBillPage()
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
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
