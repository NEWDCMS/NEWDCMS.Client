using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Order
{

    public partial class CostExpenditureSummeryPage : BaseContentPage<CostExpenditureSummeryPageViewModel>
    {
        public CostExpenditureSummeryPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
