using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace Wesley.Client.CustomViews
{


    public partial class PopConfirmView : ContentView
    {
        public PopConfirmView(string message, bool success)
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
            BindingContext = new
            {
                Message = message,
                Icon = success ? "check-circle" : "times-circle"
            };
            var confirmTGR = new TapGestureRecognizer();
            confirmTGR.Tapped += (s, e) => Confirm_Clicked(s, (TappedEventArgs)e);
            FrameContainer.GestureRecognizers.Add(confirmTGR);
        }

        public event EventHandler<bool> Picked;

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Clicked(object sender, EventArgs e)
        {
            Picked?.Invoke(this, true);
        }

        public void Invoke()
        {
            Picked?.Invoke(this, true);
        }

    }
}