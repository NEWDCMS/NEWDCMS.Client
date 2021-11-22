using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Order
{

    public partial class SaleOrderPage : BaseContentPage<SaleOrderPageViewModel>
    {
        public SaleOrderPage()
        {
            try
            {
                InitializeComponent();

            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
