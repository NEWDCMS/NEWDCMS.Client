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

        //private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        //{
        //    RadioButton rb = (RadioButton)sender;
        //    if (rb != null)
        //    {
        //        var p = (Printer)rb.Value;
        //        Settings.SelectedDeviceName = p.Name;
        //    }
        //}
    }
}
