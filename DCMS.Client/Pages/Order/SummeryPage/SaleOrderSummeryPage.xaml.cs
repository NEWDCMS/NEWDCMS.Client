using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Order
{

    public partial class SaleOrderSummeryPage : BaseContentPage<SaleOrderSummeryPageViewModel>
    {
        public SaleOrderSummeryPage()
        {
            try
            {
                InitializeComponent();

            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
