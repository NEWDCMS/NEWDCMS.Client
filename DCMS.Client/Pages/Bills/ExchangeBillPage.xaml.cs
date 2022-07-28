using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;

namespace Wesley.Client.Pages.Bills
{

    public partial class ExchangeBillPage : BaseContentPage<ExchangeBillPageViewModel>
    {
        public ExchangeBillPage()
        {
            try
            {
                InitializeComponent();
               
                //存储记录

                ToolbarItems?.Clear();
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
