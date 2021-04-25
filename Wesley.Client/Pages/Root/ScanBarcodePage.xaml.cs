using Wesley.Client.ViewModels;
using System;
using Xamarin.Forms;

namespace Wesley.Client.Pages
{
    public partial class ScanBarcodePage : BaseContentPage<ScanBarcodePageViewModel>
    {        //初始位置
        private int Ypoi = -150;
        public ScanBarcodePage()
        {
            InitializeComponent();

            Device.StartTimer(TimeSpan.FromMilliseconds(10), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Ypoi += 2;
                    AbsoluteLayout.SetLayoutBounds(redline, new Rectangle(1, Ypoi, 1, 1));
                    if (Ypoi > 200)
                    {
                        Ypoi = -150;
                    }
                });
                return true;
            });
        }

        private void MyZXingOverlay_FlashButtonClicked(Button sender, EventArgs e)
        {
            try
            {
                sender.BackgroundColor = Color.FromHex("7fadf7");
                if (!zxing.IsTorchOn)
                {
                    sender.Text = "关灯";
                    zxing.IsTorchOn = true;
                }
                else
                {
                    sender.Text = "开灯";
                    zxing.IsTorchOn = false;
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
