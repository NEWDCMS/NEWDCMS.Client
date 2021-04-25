using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace Wesley.Client.CustomViews.Views
{

    public partial class PopUpgradeView : ContentView
    {

        public PopUpgradeView(string title, string message)
        {
            try { InitializeComponent(); } catch (Exception ex) { Crashes.TrackError(ex); }
            BindingContext = new { Title = title, Message = message };
            //var closeTGR = new TapGestureRecognizer();
            //var confirmTGR = new TapGestureRecognizer();
            //closeTGR.Tapped += (s, e) => Cancel_Clicked(s, (TappedEventArgs)e);
            //confirmTGR.Tapped += (s, e) => Confirm_Clicked(s, (TappedEventArgs)e);
            //CalcleBtn.GestureRecognizers.Add(closeTGR);
            //ConfirmBtn.GestureRecognizers.Add(confirmTGR);
        }

        public event EventHandler<bool> Completed;

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="args"></param>
        public void Cancel_Clicked(Object Sender, EventArgs args)
        {
            Completed?.Invoke(this, false);
        }

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="args"></param>
        public void Confirm_Clicked(Object Sender, EventArgs args)
        {
            Completed?.Invoke(this, true);
        }


    }

}