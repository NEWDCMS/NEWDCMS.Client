using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace Wesley.Client.CustomViews
{


    public partial class PopInputView : ContentView
    {
        public PopInputView(string title, string message, Keyboard keyboard = null, string defaultValue = "", string placeHolder = "请输入值...")
        {
            try
            {
                InitializeComponent();

                BindingContext = new
                {
                    Title = title,
                    Message = message,
                    DefaultValue = defaultValue,
                    PlaceHolder = string.IsNullOrEmpty(placeHolder) ? "请输入值..." : placeHolder
                };

                txtInput.Keyboard = keyboard;
                if (keyboard == Keyboard.Numeric)
                {
                    Add.IsVisible = true;
                    Remove.IsVisible = true;
                    slContent.ColumnDefinitions[0].Width = new GridLength(30);
                    slContent.ColumnDefinitions[1].Width = GridLength.Star;
                    slContent.ColumnDefinitions[2].Width = new GridLength(30);
                }
                else
                {
                    Add.IsVisible = false;
                    Remove.IsVisible = false;
                    slContent.ColumnDefinitions[0].Width = new GridLength(0);
                    slContent.ColumnDefinitions[1].Width = GridLength.Star;
                    slContent.ColumnDefinitions[2].Width = new GridLength(0);
                }

            }
            catch (Exception ex) { Crashes.TrackError(ex); }
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

        private void Add_Clicked(object sender, EventArgs e)
        {
            int.TryParse(txtInput.Text, out int num);
            num++;
            txtInput.Text = num.ToString();
        }

        private void Remove_Clicked(object sender, EventArgs e)
        {
            int.TryParse(txtInput.Text, out int num);
            num--;
            txtInput.Text = num.ToString();
        }
    }
}