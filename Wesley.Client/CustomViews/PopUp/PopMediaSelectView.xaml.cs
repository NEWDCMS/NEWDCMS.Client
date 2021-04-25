using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.IO;
using System.Threading;
using Xamarin.Forms;
namespace Wesley.Client.CustomViews.Views
{

    public partial class PopMediaSelectView : ContentView
    {

        public PopMediaSelectView(string title, string message)
        {
            try { InitializeComponent(); } catch (Exception ex) { Crashes.TrackError(ex); }
            BindingContext = new
            {
                Title = title,
                Message = message
            };
        }

        public event EventHandler Completed;

        public object SelectecItem { get; set; }

        private void Confirm_Clicked(object sender, EventArgs e)
        {
            Completed?.Invoke(this, new EventArgs());
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Completed?.Invoke(this, new EventArgs());
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

                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(10));

                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Full
                });

                if (file == null)
                {
                    return;
                }

                // var path = file.Path;

                var base64Str = "";
                using (Stream mediaStream = file.GetStream())
                using (MemoryStream memStream = new MemoryStream())
                {
                    await mediaStream.CopyToAsync(memStream);
                    base64Str = Convert.ToBase64String(memStream.ToArray());
                }
                SelectecItem = base64Str;
                //_loggingService.Error("Debug:",path);
                //var bitmap = BitmapFactory.DecodeFile(file.Path);
                //image.SetImageBitmap(bitmap);
                if (file != null)
                {
                    file.Dispose();
                }
            }
            catch (Exception ex)
            {
                await dialogService.ShowAlertAsync(ex.Message, "File Location", "OK");
            }
            finally
            {
                Completed?.Invoke(this, new EventArgs());
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
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await dialogService.ShowConfirmAsync("摄像头不能使用", "无摄像头", "确定");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    //Directory = "Sample",
                    //Name = "userface.jpg"
                });

                if (file == null)
                {
                    return;
                }

                var base64Str = "";
                using (Stream mediaStream = file.GetStream())
                using (MemoryStream memStream = new MemoryStream())
                {
                    await mediaStream.CopyToAsync(memStream);
                    base64Str = Convert.ToBase64String(memStream.ToArray());
                }

                SelectecItem = base64Str;

                if (file != null)
                {
                    file.Dispose();
                }
            }
            catch (Exception ex)
            {
                await dialogService.ShowAlertAsync(ex.Message, "File Location", "OK");
            }
            finally
            {
                Completed?.Invoke(this, new EventArgs());
            }

        }
    }
}