using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages
{

    public partial class PrintSettingPage : BaseContentPage<PrintSettingPageViewModel>
    {
        public PrintSettingPage()
        {
            try { InitializeComponent(); } catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
