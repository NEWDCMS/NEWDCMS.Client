using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Market
{

    public partial class UnSalePage : BaseContentPage<UnSalePageViewModel>
    {
        public UnSalePage()
        {
            try { InitializeComponent(); } catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
