using Shiny.Logging;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace DCMS.Client.Pages.Market
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnDeliverView : ContentView
    {
        public UnDeliverView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
