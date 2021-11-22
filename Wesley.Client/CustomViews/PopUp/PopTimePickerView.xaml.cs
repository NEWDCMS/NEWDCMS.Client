using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace Wesley.Client.CustomViews
{


    public partial class PopTimePickerView : ContentView
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Title { get; set; }


        public PopTimePickerView(string title, Keyboard keyboard = null)
        {
            Title = title;

            try { InitializeComponent(); } catch (Exception ex) { Crashes.TrackError(ex); }
            BindingContext = this;
        }

        /// <summary>
        /// 定义选择事件
        /// </summary>
        public event EventHandler<Tuple<DateTime, DateTime>> Picked;

        ///// <summary>
        ///// 焦点输入
        ///// </summary>
        //public void FocusEntry()
        //{
        //    txtInput.Focus();
        //}

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Clicked(object sender, EventArgs e)
        {
            Picked?.Invoke(this, new Tuple<DateTime, DateTime>(startDateTimeInput.Date, endDateTimeInput.Date));
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Picked?.Invoke(this, null);
        }
    }
}