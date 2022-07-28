using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace DCMS.Client.Pages.Market
{

    public partial class UnSaleView : ContentView
    {

        public UnSaleView()
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
