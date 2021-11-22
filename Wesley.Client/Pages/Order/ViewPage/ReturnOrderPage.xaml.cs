using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Order
{

    public partial class ReturnOrderPage : BaseContentPage<ReturnOrderPageViewModel>
    {
        public ReturnOrderPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
