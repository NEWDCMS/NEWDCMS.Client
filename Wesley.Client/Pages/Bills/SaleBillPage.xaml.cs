using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;

namespace Wesley.Client.Pages.Bills
{

    public partial class SaleBillPage : BaseContentPage<SaleBillPageViewModel>
    {
        public SaleBillPage()
        {
            try
            {
                InitializeComponent();
               
                //存储记录

                ToolbarItems?.Clear();

                foreach (var toolBarItem in this.GetToolBarItems(ViewModel, true, "保存").ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
