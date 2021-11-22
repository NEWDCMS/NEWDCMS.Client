using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Plugin.Media;
using Plugin.Media.Abstractions;
using ReactiveUI;
using Rg.Plugins.Popup.Services;
using System;
using System.IO;
using System.Threading;
using Xamarin.Forms;

namespace Wesley.Client.CustomViews
{
    public partial class PopMediaSelectView : ContentView
    {
        public PopMediaSelectView(string title, string message)
        {
            try 
            { 
                InitializeComponent();
                BindingContext = new
                {
                    Title = title,
                    Message = message
                };
            } 
            catch (Exception ex) 
            { 
                Crashes.TrackError(ex);
            }
        }

        public event EventHandler Completed;
        private void Confirm_Clicked(object sender, EventArgs e)
        {
            Completed?.Invoke(this, new EventArgs());
            CloseAllPopup();
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Completed?.Invoke(this, new EventArgs());
            CloseAllPopup();
        }

        /// <summary>
        /// 从相册选取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PickMediaOptions_Tapped(object sender, EventArgs e)
        {
            var dialogService = App.Resolve<IDialogService>();
            try
            {
                CloseAllPopup();

                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(10));

                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Full,
                    CompressionQuality = 50
                });

                if (file == null)
                {
                    return;
                }

                byte[]? bit;
                using (Stream mediaStream = file.GetStream())
                using (MemoryStream memStream = new MemoryStream())
                {
                    await mediaStream.CopyToAsync(memStream);
                    bit = memStream.ToArray();
                }

                MessageBus.Current.SendMessage(bit, string.Format(Constants.CAMERA_KEY, "UpdateCutFace"));

                if (file != null)
                {
                    file.Dispose();
                }
            }
            catch (Exception ex)
            {
                await dialogService.ShowAlertAsync(ex.Message, "File Location", "OK");
            }
        }

        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StoreCameraMediaOptions_Tapped(object sender, EventArgs e)
        {
            var dialogService = App.Resolve<IDialogService>();
            try
            {
                CloseAllPopup();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await dialogService.ShowConfirmAsync("摄像头不能使用", "无摄像头", "确定");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    CompressionQuality = 50
                });

                if (file == null)
                {
                    return;
                }

                byte[]? bit;
                using (Stream mediaStream = file.GetStream())
                using (MemoryStream memStream = new MemoryStream())
                {
                    await mediaStream.CopyToAsync(memStream);
                    bit = memStream.ToArray();
                }

                MessageBus.Current.SendMessage(bit, string.Format(Constants.CAMERA_KEY, "UpdateCutFace"));

                if (file != null)
                {
                    file.Dispose();
                }
            }
            catch (Exception ex)
            {
                await dialogService.ShowAlertAsync(ex.Message, "File Location", "OK");
            }
        }

        private async void CloseAllPopup()
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                await PopupNavigation.Instance.PopAllAsync();
        }
    }
}