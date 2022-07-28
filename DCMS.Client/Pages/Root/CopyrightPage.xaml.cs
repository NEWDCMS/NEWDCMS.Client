using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages
{

    public partial class CopyrightPage : BaseContentPage<CopyrightPageViewModel>
    {
        public CopyrightPage()
        {
            try { InitializeComponent(); } catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
