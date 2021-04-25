using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages
{

    public partial class SecurityPage : BaseContentPage<SecurityPageViewModel>
    {
        public SecurityPage()
        {
            try { InitializeComponent(); } catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
