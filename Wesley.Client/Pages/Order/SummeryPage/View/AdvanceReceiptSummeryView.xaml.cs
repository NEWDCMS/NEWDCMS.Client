using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Order
{

    public partial class AdvanceReceiptSummeryView : ContentView
    {
        public AdvanceReceiptSummeryView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
