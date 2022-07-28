using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages
{

    public partial class ResetPasswordPage : BaseContentPage<ResetPasswordPageViewModel>
    {
        public ResetPasswordPage()
        {
            try { InitializeComponent(); } catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
