using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;

namespace Wesley.Client.Pages.Common
{

    public partial class AddReportProductPage : BaseContentPage<AddReportProductPageViewModel>
    {
        public AddReportProductPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
