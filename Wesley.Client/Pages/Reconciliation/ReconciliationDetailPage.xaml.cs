using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;

namespace Wesley.Client.Pages.Reporting
{

    public partial class ReconciliationDetailPage : BaseContentPage<ReconciliationDetailPageViewModel>
    {
        public ReconciliationDetailPage()
        {
            try
            {
                InitializeComponent();
                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetPrintToolBarItems(ViewModel, true).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

        private void CheckBox_CheckedChanged(object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.CheckChanged();
            }
        }
    }
}
