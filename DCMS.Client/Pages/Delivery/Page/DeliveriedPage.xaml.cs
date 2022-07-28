using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Market
{

    public partial class DeliveriedPage : BaseContentPage<DeliveriedPageViewModel>
    {
        public DeliveriedPage()
        {
            try { InitializeComponent(); } catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
