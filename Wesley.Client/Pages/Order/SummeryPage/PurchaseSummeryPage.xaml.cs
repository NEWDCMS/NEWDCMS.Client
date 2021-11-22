using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Order
{

    public partial class PurchaseSummeryPage : BaseContentPage<PurchaseSummeryPageViewModel>
    {
        public PurchaseSummeryPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }

        }

    }
}
