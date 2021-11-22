using Acr.UserDialogs;
using Wesley.BitImageEditor;
using Wesley.Client.Models.Media;
using Wesley.Client.Models.Users;
using Wesley.Client.Services;
using Newtonsoft.Json;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace Wesley.Client.ViewModels
{

    public class MyInfoPageViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IPermissionsService _permissionsService;
        public ReactiveCommand<string, Unit> UpdateFaceCommand { get; set; }
        [Reactive] public UserAuthenticationModel UserProfile { get; set; } = new UserAuthenticationModel();
        [Reactive] public string UserFace { get; set; } = "profile_placeholder.png";
        public HttpClientHelper httpClientHelper;


        public MyInfoPageViewModel(INavigationService navigationService,
           IUserService userService,
            IPermissionsService permissionsService,
              IDialogService dialogService
            ) : base(navigationService, dialogService)
        {

            _userService = userService;
            _permissionsService = permissionsService;


            Title = "个人信息";

            httpClientHelper = new HttpClientHelper();

            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(() =>
           {
               try
               {
                   UserProfile = new UserAuthenticationModel()
                   {
                       Username = Settings.UserRealName,
                       UserRealName = Settings.UserRealName,
                       FaceImage = "profile_placeholder.png",
                       Email = Settings.UserEmall,
                       MobileNumber = Settings.UserMobile,
                       StoreId = Settings.StoreId,
                       StoreName = Settings.StoreName,

                       DealerNumber = Settings.DealerNumber,
                       MarketingCenter = Settings.MarketingCenter,
                       MarketingCenterCode = Settings.MarketingCenterCode,
                       SalesArea = Settings.SalesArea,
                       SalesAreaCode = Settings.SalesAreaCode,
                       BusinessDepartment = Settings.BusinessDepartment,
                       BusinessDepartmentCode = Settings.BusinessDepartmentCode
                   };

                   if (!string.IsNullOrEmpty(Settings.FaceImage) && Settings.FaceImage.StartsWith("http"))
                   {
                       //ImageSource.FromUri(new Uri(Settings.FaceImage));
                       UserFace = Settings.FaceImage;
                   }
               }
               catch (Exception) { }
           }));

            //选择照片
            this.UpdateFaceCommand = ReactiveCommand.CreateFromTask<string>(async (e) =>
            {
                await this.NavigateAsync("CameraViewPage", ("TakeType", "UpdateFace"));
            });

            this.BindBusyCommand(Load);

            MessageBus
                .Current
                .Listen<byte[]?>(string.Format(Constants.CAMERA_KEY, "UpdateFace"))
                .Subscribe(async bit =>
                {
                    try
                    {
                        if (bit != null && bit.Length > 0)
                        {
                            var stream = new MemoryStream(bit);
                            //编辑照片
                            var display = DeviceDisplay.MainDisplayInfo;
                            var config = new ImageEditorConfig(
                                canAddText: false,
                                canFingerPaint: false,
                                backgroundType: BackgroundType.StretchedImage,
                                outImageHeight: (int)display.Height,
                                outImageWidht: (int)display.Width,
                                aspect: BBAspect.Auto);

                            using (var bitmap = SKBitmap.Decode(stream))
                            {
                                byte[] data = await ImageEditor.Instance.GetEditedImage(bitmap, config);
                                var resultStream = new MemoryStream(data);
                                UploadPhotograph(resultStream);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.StackTrace);
                        _dialogService.LongAlert("服务器错误，编辑失败！");
                    }
                }).DisposeWith(DeactivateWith);
        }

        private async void UploadPhotograph(MemoryStream stream)
        {
            //上传图片
            using (UserDialogs.Instance.Loading("上传中..."))
            {
                try
                {
                    if (stream != null)
                    {
                        var scb = new StreamContent(stream);
                        var content = new MultipartFormDataContent { { scb, "\"file\"", $"\"{Settings.UserId}_userface.jpg\"" } };
                        var url = $"{GlobalSettings.FileCenterEndpoint}document/reomte/fileupload/HRXHJS";
                        var result = await httpClientHelper.PostAsync(url, content);

                        var uploadResult = new UploadResult();
                        if (!string.IsNullOrEmpty(result))
                        {
                            uploadResult = JsonConvert.DeserializeObject<UploadResult>(result);
                        }

                        if (uploadResult != null)
                        {
                            var fPath = $"{GlobalSettings.FileCenterEndpoint}HRXHJS/document/image/" + uploadResult.Id + "";
                            Settings.FaceImage = fPath;
                            UserFace = fPath;
                            await _userService.UpLoadFaceImageAsync(fPath);
                        }

                        if (content != null)
                            content.Dispose();

                        if (scb != null)
                            scb.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.StackTrace);
                    _dialogService.LongAlert("服务器错误，上传失败！");
                }
                finally
                {
                    if (stream != null)
                        stream.Dispose();
                }
            };
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            _permissionsService.RequestLocationAndCameraPermission();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
