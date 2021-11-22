using Wesley.Client.Models;
using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.ComponentModel;
using System.Linq;
namespace Wesley.Client.Pages.Archive
{

    [DesignTimeVisible(false)]
    public partial class VisitRecordsPage : BaseContentPage<VisitRecordsPageViewModel>
    {
        public VisitRecordsPage()
        {
            try
            {
                InitializeComponent();
                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems(ViewModel, ("Filter", new FilterModel()
                {
                    TerminalEnable = true,
                    DistrictEnable = true,
                    BusinessUserEnable = true
                })).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }

            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }
    }
}
