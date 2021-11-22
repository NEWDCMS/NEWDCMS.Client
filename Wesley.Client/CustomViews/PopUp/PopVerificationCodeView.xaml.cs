using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace Wesley.Client.CustomViews
{


    public partial class PopVerificationCodeView : ContentView
    {
        private System.Threading.Timer _timer;
        public int Counter { get; set; } = 30;

        public PopVerificationCodeView(string title, string message, Keyboard keyboard = null, string defaultValue = "", string placeHolder = "请输入值...")
        {
            try { InitializeComponent(); } catch (Exception ex) { Crashes.TrackError(ex); }
            BindingContext = new
            {
                Title = title,
                Message = message,
                DefaultValue = defaultValue,
                PlaceHolder = placeHolder
            };

            txtInput.Keyboard = keyboard;
        }

        /// <summary>
        /// 定义选择事件
        /// </summary>
        public event EventHandler<string> Picked;

        /// <summary>
        /// 焦点输入
        /// </summary>
        public void FocusEntry()
        {
            txtInput.Focus();
        }

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtInput.Text))
            {
                Picked?.Invoke(this, txtInput.Text);
            }
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

        private void TxtInput_Focused(object sender, FocusEventArgs e)
        {
            txtInput.Focus();
        }

        private void btnVerificationCode_Clicked(object sender, EventArgs e)
        {
            _timer = new System.Threading.Timer((state) =>
            {
                if (Counter > 0)
                {
                    Counter--;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        btnVerificationCode.IsEnabled = false;
                        btnVerificationCode.Text = $"{ Counter} 秒后获取";
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        txtInput.Text = "123456";
                        btnVerificationCode.IsEnabled = true;
                        btnVerificationCode.Text = "获取";
                    });
                    _timer.Dispose();
                }
            }, null, 0, 1000);
        }
    }
}