using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Order
{

    public partial class AdvanceReceiptSummeryPage : BaseContentPage<AdvanceReceiptSummeryPageViewModel>
    {
        public AdvanceReceiptSummeryPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
