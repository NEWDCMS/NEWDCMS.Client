using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Order
{

    public partial class InventorySummeryView : ContentView
    {

        public InventorySummeryView()
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
