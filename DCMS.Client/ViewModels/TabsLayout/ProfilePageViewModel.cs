using Acr.UserDialogs;
using DCMS.BitImageEditor;
using DCMS.Client.CustomViews;
using DCMS.Client.Models;
using DCMS.Client.Models.Media;
using DCMS.Client.Models.Report;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.Media;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace DCMS.Client.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        private readonly IReportingService _reportingService;
        private readonly IUserService _userService;

        [Reactive] public string ProfileName { get; set; }
        [Reactive] public string UserMobile { get; set; }
        [Reactive] public string UserFace { get; set; } = "profile_placeholder.png";
        [Reactive] public DashboardReport Analysis { get; set; } = new DashboardReport();
        [Reactive] public IList<CutMenus> Menus { get; set; } = new ObservableCollection<CutMenus>();
        [Reactive] public CutMenus Selecter { get; set; }
        public IReactiveCommand LoadDashboardReport { get; set; }
        public ReactiveCommand<string, Unit> UpdateFaceCommand { get; set; }

        public HttpClientHelper httpClientHelper;

        public ProfilePageViewModel(INavigationService navigationService,
            IReportingService reportingService,
            IDialogService dialogService,
            IUserService userService) : base(navigationService, dialogService)
        {
            _userService = userService;
            _reportingService = reportingService;
            httpClientHelper = new HttpClientHelper();


            //菜单选择
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
                .Skip(1)
                .Where(x => x != null)
                .SubOnMainThread(async item =>
               {
                   if ("SystemSettingPage" == item.Url)
                   {
                       using (UserDialogs.Instance.Loading("加载中..."))
                       {
                           await Task.Delay(200);
                           await this.NavigateAsync(item.Url, null);
                           return;
                       }
                   }
                   else
                   {
                       await this.NavigateAsync(item.Url, null);
                   }

                   this.Selecter = null;
               })
                .DisposeWith(DeactivateWith);

            //加载数据
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(() =>
            {
                try
                {
                    var menus = new List<CutMenus>()
                    {
                        new CutMenus
                        {
                            Type=0,
                            Name="系统设置",
                            Icon = "&#xf013;",
                            Url="SystemSettingPage" ,
                            ShowSeparator= true,
                            Describe=""
                        },
                        new CutMenus
                        {
                            Type=0,
                            Name="个人信息",
                            Icon = "&#xf007;",
                            Url="MyInfoPage" ,
                            ShowSeparator= true,
                            Describe=""
                        },
                        new CutMenus
                        {
                            Type=0,
                            Name="账号安全",
                            Icon = "&#xf2bb;",
                            Url="SecurityPage",
                            ShowSeparator= true,
                            Describe=""
                        },
                        new CutMenus
                        {
                            Type=8,
                            Name="打印设置",
                            Icon = "&#xf02f;",
                            Url="PrintSettingPage",
                              ShowSeparator= true,
                            //adapter.Address = "02:00:00:00:00:00"
                            Describe = App.BtAddress
                        },
                        new CutMenus
                        {
                            Type=0,
                            Name="关于我们",
                            Icon = "&#xf2dc;",
                            Url="AboutPage",
                            ShowSeparator= true,
                            Describe=""
                        },
                        new CutMenus
                        {
                            Type=0,
                            Name="版本更新",
                            Icon = "&#xf019;",
                            Url="UpdatePage",
                            ShowSeparator= true,
                            Describe = ""
                        },
                        new CutMenus
                        {
                            Type=1,
                            Name="技术支持",
                            Icon = "&#xf086;",
                            Url="ConversationsPage",
                            ShowSeparator= true,
                            Describe=""
                        },
                        new CutMenus{
                            Type=1,
                            Name="问题反馈",
                            Icon = "&#xf059;",
                            Url="FeedbackPage",
                            ShowSeparator= false,
                            Describe=""
                        }
                    };
                    this.Menus = new ObservableCollection<CutMenus>(menus);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }));

            //统计
            this.LoadDashboardReport = ReactiveCommand.CreateFromTask(() => Task.Run(() =>
            {
                try
                {
                    this.ProfileName = Settings.UserRealName;
                    this.StoreName = Settings.StoreName;
                    this.UserMobile = Settings.UserMobile;

                    if (!string.IsNullOrEmpty(Settings.FaceImage) && Settings.FaceImage.StartsWith("http"))
                        this.UserFace = Settings.FaceImage;

                    _reportingService.Rx_GetDashboardReportAsync(new System.Threading.CancellationToken())
                  .Subscribe((results) =>
                  {
                      if (results != null && results?.Code >= 0)
                      {
                          var pending = results?.Data;
                          if (pending != null)
                              this.Analysis = pending;
                      }
                  });

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }));

            //选择照片
            this.UpdateFaceCommand = ReactiveCommand.CreateFromTask<string>(async (e) =>
            {
                var succ = await CrossMedia.Current.Initialize();
                if (succ)
                {
                    if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
                    {
                        _dialogService.LongAlert("抱歉,没有检测到相机...");
                        return;
                    }
                    else
                    {
                        try
                        {
                            CrossDiaglogKit.Current.GetMediaResultAsync("选择照片", "");
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                        }
                    }
                }
            });


            MessageBus
             .Current
             .Listen<byte[]?>(string.Format(Constants.CAMERA_KEY, "UpdateCutFace"))
             .Subscribe(bit =>
             {
                 try
                 {
                     if (bit != null && bit.Length > 0)
                     {
                         var stream = new MemoryStream(bit);
                         UploadPhotograph(stream);

                         ////编辑照片
                         //var display = DeviceDisplay.MainDisplayInfo;
                         //var config = new ImageEditorConfig(
                         //    canAddText: false,
                         //    canFingerPaint: false,
                         //    backgroundType: BackgroundType.StretchedImage,
                         //    outImageHeight: (int)display.Height,
                         //    outImageWidht: (int)display.Width,
                         //    aspect: BBAspect.Auto);

                         //using (var bitmap = SKBitmap.Decode(stream))
                         //{
                         //    byte[] data = await ImageEditor.Instance.GetEditedImage(bitmap, config);
                         //    var resultStream = new MemoryStream(data);
                         //    UploadPhotograph(resultStream);
                         //}
                     }
                 }
                 catch (Exception ex)
                 {
                     _dialogService.LongAlert("服务器错误，编辑失败！");
                 }
             }).DisposeWith(DeactivateWith);


            this.BindBusyCommand(Load);
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

            ThrottleLoad(() =>
            {
                ((ICommand)LoadDashboardReport)?.Execute(null);
                ((ICommand)Load)?.Execute(null);

            }, (this.Menus?.Count ?? 0) == 0);
        }
    }
}
