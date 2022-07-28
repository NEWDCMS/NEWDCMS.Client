using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;

namespace Wesley.Client.Pages.Bills
{

    public partial class SaleOrderBillPage : BaseContentPage<SaleOrderBillPageViewModel>
    {
        public SaleOrderBillPage()
        {

            try
            {
                InitializeComponent();
                ToolbarItems?.Clear();
                //存储记录

                string btnIco = "保存";
                foreach (var toolBarItem in this.GetToolBarItems(ViewModel, true, btnIco).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
