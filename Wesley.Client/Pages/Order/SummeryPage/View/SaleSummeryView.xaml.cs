using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Order
{

    public partial class SaleSummeryView : ContentView
    {

        public SaleSummeryView()
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
