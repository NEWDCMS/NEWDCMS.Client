using Microsoft.AppCenter.Crashes;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace Wesley.Client.Pages
{

    public partial class MessagesView : ContentView
    {

        public MessagesView()
        {
            try
            {
                InitializeComponent();
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
