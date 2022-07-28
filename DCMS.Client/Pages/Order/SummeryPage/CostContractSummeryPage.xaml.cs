using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Order
{

    public partial class CostContractSummeryPage : BaseContentPage<CostContractSummeryPageViewModel>
    {
        public CostContractSummeryPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
