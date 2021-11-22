using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;

namespace Wesley.Client.Pages.Market
{

    public partial class UnCostExpenditurePage : BaseContentPage<UnCostExpenditurePageViewModel>
    {
        public UnCostExpenditurePage()
        {
            try { InitializeComponent(); } catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
