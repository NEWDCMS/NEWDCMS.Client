using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Common
{

    public partial class SelectAreaPage : BaseContentPage<SelectAreaPageViewModel>
    {
        public SelectAreaPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
