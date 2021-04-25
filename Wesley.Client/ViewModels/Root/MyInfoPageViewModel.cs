using Acr.UserDialogs;
using Wesley.BitImageEditor;
using Wesley.Client.CustomViews;
using Wesley.Client.Models.Media;
using Wesley.Client.Models.Users;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SkiaSharp;
using System;
using System.IO;
using System.Net.Http;
using System.Reactive;
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


        public MyInfoPageViewModel(INavigationService navigationService,
           IUserService userService,
            IPermissionsService permissionsService,
              IDialogService dialogService) : base(navigationService, dialogService)
        {

            _userService = userService;
            _permissionsService = permissionsService;


            Title = "个人信息";

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
            this.UpdateFaceCommand = ReactiveCommand.Create<string>(async (e) =>
            {
                try
                {
                    var resultMedia = await CrossDiaglogKit.Current.GetMediaResultAsync("请选择", "");
                    if (resultMedia != null)
                    {

                        //编辑照片
                        var display = DeviceDisplay.MainDisplayInfo;
                        var config = new ImageEditorConfig(
                            canAddText: false,
                            canFingerPaint: false,
                            backgroundType: BackgroundType.StretchedImage,
                            outImageHeight: (int)display.Height,
                            outImageWidht: (int)display.Width,
                            aspect: BBAspect.Auto);

                        byte[] base64Stream = Convert.FromBase64String(resultMedia.ToString());
                        var stream = new MemoryStream(base64Stream);
                        if (stream == null)
                        {
                            return;
                        }

                        SKBitmap bitmap = SKBitmap.Decode(stream);

                        byte[] data = await ImageEditor.Instance.GetEditedImage(bitmap, config);
                        if (data != null)
                        {
                            var resultStream = new MemoryStream(data);

                            //上传图片
                            using (UserDialogs.Instance.Loading("更新中..."))
                            {
                                var content = new MultipartFormDataContent
                                    {
                                        { new StreamContent(resultStream), "\"file\"", $"\"{Settings.UserId}_userface.jpg\"" }
                                    };

                                using (var httpClient = new HttpClient())
                                {
                                    var uploadServiceBaseAddress = $"{GlobalSettings.FileCenterEndpoint}document/reomte/fileupload/HRXHJS";
                                    var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);
                                    var result = await httpResponseMessage.Content.ReadAsStringAsync();
                                    var uploadResult = new UploadResult();
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            uploadResult = JsonConvert.DeserializeObject<UploadResult>(result);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        uploadResult = null;
                                    }

                                    if (httpResponseMessage != null)
                                        httpResponseMessage.Dispose();

                                    var fPath = $"{GlobalSettings.FileCenterEndpoint}HRXHJS/document/image/" + uploadResult.Id + "";
                                    Settings.FaceImage = fPath;

                                    UserFace = fPath;

                                    await _userService.UpLoadFaceImageAsync(fPath);
                                }

                                if (resultStream != null)
                                    resultStream.Dispose();
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }


        public override void OnAppearing()
        {
            base.OnAppearing();

            _permissionsService.RequestLocationAndCameraPermission();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
