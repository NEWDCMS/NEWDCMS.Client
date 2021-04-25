using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace Wesley.Client.CustomViews.Views
{


    public partial class PopView : ContentView
    {
        public PopView(string title, string message)
        {
            try { InitializeComponent(); } catch (Exception ex) { Crashes.TrackError(ex); }
            BindingContext = new
            {
                Title = title,
                Message = message
            };
        }

        /// <summary>
        /// 定义选择事件
        /// </summary>
        public event EventHandler<bool> Completed;

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Clicked(object sender, EventArgs e)
        {
            Completed?.Invoke(this, true);
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Completed?.Invoke(this, false);
        }
    }
}