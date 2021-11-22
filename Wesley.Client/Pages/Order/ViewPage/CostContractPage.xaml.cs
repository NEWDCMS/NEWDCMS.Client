using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Order
{

    public partial class CostContractPage : BaseContentPage<CostContractPageViewModel>
    {
        public CostContractPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
