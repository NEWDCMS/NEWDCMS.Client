using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Order
{

    public partial class CostExpenditurePage : BaseContentPage<CostExpenditurePageViewModel>
    {
        public CostExpenditurePage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }

        }
    }
}
