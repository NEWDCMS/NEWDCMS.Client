using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Market
{

    public partial class UnOrderPage : BaseContentPage<UnOrderPageViewModel>
    {
        public UnOrderPage()
        {
            try { InitializeComponent(); } catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
