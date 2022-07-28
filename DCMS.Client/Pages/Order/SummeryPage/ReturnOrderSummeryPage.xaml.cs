using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Order
{

    public partial class ReturnOrderSummeryPage : BaseContentPage<ReturnOrderSummeryPageViewModel>
    {
        public ReturnOrderSummeryPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
