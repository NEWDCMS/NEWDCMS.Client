using Acr.UserDialogs;
using Wesley.Client.Models.Media;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SignaturePad.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Wesley.Client.CustomViews
{

    /// <summary>
    /// 手写签字版
    /// </summary>

    public partial class PopSignaturePadView : ContentView
    {

        private IEnumerable<Point> currentSignature;
        private Point[] savedSignature;


        public PopSignaturePadView(string title, string message, Keyboard keyboard = null, string defaultValue = "", string placeHolder = "请输入值...")
        {
            try
            {
                InitializeComponent();
                //BindingContext = new BindingViewModel(signaturePadView.GetImageStreamAsync);
                BindingContext = new
                {
                    Title = title,
                    Message = message,
                    DefaultValue = defaultValue,
                    PlaceHolder = placeHolder
                };
                GetImageStreamAsync = signaturePadView.GetImageStreamAsync;
                //SaveVectorCommand = new Command(OnSaveVector);
                //LoadVectorCommand = new Command(OnLoadVector);
                //SaveImageCommand = new Command(OnSaveImage);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public IEnumerable<Point> CurrentSignature
        {
            get => currentSignature;
            set
            {
                currentSignature = value;
                OnPropertyChanged();
            }
        }

        public Point[] SavedSignature
        {
            get => savedSignature;
            set
            {
                savedSignature = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSavedSignature));
            }
        }
        public bool HasSavedSignature => SavedSignature?.Length > 0;

        //public ICommand SaveVectorCommand { get; }
        //public ICommand LoadVectorCommand { get; }
        //public ICommand SaveImageCommand { get; }

        private Func<SignatureImageFormat, ImageConstructionSettings, Task<Stream>> GetImageStreamAsync { get; }


        /// <summary>
        /// 保存矢量图
        /// </summary>
        private void OnSaveVector()
        {
            SavedSignature = CurrentSignature.ToArray();
        }

        /// <summary>
        /// 载入矢量
        /// </summary>
        private void OnLoadVector()
        {
            CurrentSignature = SavedSignature;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        private async Task<string> OnSaveImage()
        {
            string ImageId = "";
            var settings = new ImageConstructionSettings
            {
                StrokeColor = Color.Black,
                BackgroundColor = Color.White,
                StrokeWidth = 1f
            };
            using (var bitmap = await GetImageStreamAsync(SignatureImageFormat.Png, settings))
            {
                var httpClientHelper = new HttpClientHelper();
                //上传图片
                using (UserDialogs.Instance.Loading("上传中..."))
                {
                    try
                    {
                        var scb = new StreamContent(bitmap);
                        var content = new MultipartFormDataContent { { scb, "\"file\"", $"\"signature.png\"" } };
                        var url = $"{GlobalSettings.FileCenterEndpoint}document/reomte/fileupload/HRXHJS";
                        var result = await httpClientHelper.PostAsync(url, content);
                        var uploadResult = new UploadResult();
                        if (!string.IsNullOrEmpty(result))
                        {
                            uploadResult = JsonConvert.DeserializeObject<UploadResult>(result);
                        }
                        if (uploadResult != null)
                        {
                            ImageId = uploadResult.Id;
                        }

                        if (scb != null)
                            scb.Dispose();

                        if (content != null)
                            content.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                };
            }

            return ImageId;
        }


        /// <summary>
        /// 定义选择事件
        /// </summary>
        public event EventHandler<string> Picked;

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Confirm_Clicked(object sender, EventArgs e)
        {
            var imageId = await OnSaveImage();
            Picked?.Invoke(this, imageId);
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