using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;

namespace Wesley.Client.Pages.Archive
{
    public partial class HotOrderRankingPage : BaseContentPage<HotOrderRankingPageViewModel>
    {
        public HotOrderRankingPage()
        {
            try
            {

                InitializeComponent();
                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems(ViewModel,
                    ("Filter", new Models.FilterModel()
                    {
                        TerminalEnable = true,
                        BusinessUserEnable = true,
                        BrandEnable = true,
                        ProductEnable = true,
                        CatagoryEnable = true
                    })).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
