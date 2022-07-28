using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace DCMS.Client.Pages.Market
{

    public partial class UnOrderView : ContentView
    {
        public UnOrderView()
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
