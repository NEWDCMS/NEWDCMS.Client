using Acr.UserDialogs;
using DCMS.Client.CustomViews;
using DCMS.Client.Models;
using DCMS.Client.Models.Census;
using DCMS.Client.Models.Media;
using DCMS.Client.Services;
using DCMS.Client.Services.QA;
using DCMS.Infrastructure.Helpers;
using Newtonsoft.Json;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
namespace DCMS.Client.ViewModels
{
    public class FeedbackPageViewModel : ViewModelBase
    {
        [Reactive] public string IssueDescribe { get; set; }
        [Reactive] public bool IsEnabled { get; set; }
        [Reactive] public int SelectedSegment { get; set; }
        [Reactive] public string Contacts { get; set; }
        [Reactive] public string TextCounterTxt { get; set; }

        public ReactiveCommand<string, Unit> TextChangedCommand { get; set; }
        public IReactiveCommand SubmitCommand { get; set; }
        public IReactiveCommand CameraPhotoCmd { get; set; }
        public ReactiveCommand<string, Unit> RemoveCommand { get; set; }

        [Reactive] public ObservableCollection<DisplayPhoto> DisplayPhotos { get; set; } = new ObservableCollection<DisplayPhoto>();

        private readonly IFeedbackService _feedbackService;
        private readonly IMediaPickerService _mediaPickerService;



        public FeedbackPageViewModel(INavigationService navigationService,


            IDialogService dialogService,
            IFeedbackService feedbackService,
            IMediaPickerService mediaPickerService
            ) : base(navigationService, dialogService)
        {
            _feedbackService = feedbackService;
            _mediaPickerService = mediaPickerService;


            Title = "问题反馈";
            this.TextCounterTxt = "0/500";
            this.Contacts = Settings.UserEmall ?? (Settings.UserMobile ?? Settings.UserRealName);

            this.WhenAnyValue(x => x.IssueDescribe)
                .Skip(1)
                .Where(x => x != null)
                .Select(x =>
            {
                if (!string.IsNullOrEmpty(x))
                    return $"{x.Length}/500";
                else
                    return $"0/500";
            }).Subscribe(x => { this.TextCounterTxt = x; }).DisposeWith(DeactivateWith);


            this.TextChangedCommand = ReactiveCommand.Create<string>((e) =>
            {
                if (IssueDescribe.Length > 500)
                {
                    IsEnabled = false;
                    _dialogService.ShortAlert("字数超过限制！");
                }
                else
                    IsEnabled = true;
            });

            //验证
            var valid_Describe = this.ValidationRule(x => x.IssueDescribe, _isDefined, "请先输入内容");
            var valid_Contracts = this.ValidationRule(x => x.Contacts, _isDefined, "请先输入联系方式");
            var valid_Photos = this.ValidationRule(x => x.DisplayPhotos.Count, _isZero, "请先上传截图");
            var canExcute = this.WhenAnyValue(x => x.DisplayPhotos.Count).Select(x => x <= 4).Do(x => { if (!x) { this.Alert("最多只能五张哦！"); } });

            //提交
            this.SubmitCommand = ReactiveCommand.CreateFromTask<object>(async e =>
           {
               try
               {
                   if (!valid_Describe.IsValid) { this.Alert(valid_Describe.Message[0]); return; }
                   if (!valid_Contracts.IsValid) { this.Alert(valid_Contracts.Message[0]); return; }
                   if (!valid_Photos.IsValid) { this.Alert(valid_Photos.Message[0]); return; }

                   var feedback = new FeedBack
                   {
                       FeedbackTyoe = this.SelectedSegment,
                       IssueDescribe = this.IssueDescribe,
                       Contacts = this.Contacts,
                       CreatedOnUtc = DateTime.Now
                   };

                   if (DisplayPhotos != null && DisplayPhotos.Count > 0)
                   {
                       switch (DisplayPhotos.Count)
                       {
                           case 1:
                               feedback.Screenshot1 = DisplayPhotos[0] == null ? "" : DisplayPhotos[0].DisplayPath;
                               break;
                           case 2:
                               feedback.Screenshot1 = DisplayPhotos[0] == null ? "" : DisplayPhotos[0].DisplayPath;
                               feedback.Screenshot2 = DisplayPhotos[1] == null ? "" : DisplayPhotos[1].DisplayPath;
                               break;
                           case 3:
                               feedback.Screenshot1 = DisplayPhotos[0] == null ? "" : DisplayPhotos[0].DisplayPath;
                               feedback.Screenshot2 = DisplayPhotos[1] == null ? "" : DisplayPhotos[1].DisplayPath;
                               feedback.Screenshot3 = DisplayPhotos[2] == null ? "" : DisplayPhotos[2].DisplayPath;
                               break;
                           case 4:
                               feedback.Screenshot1 = DisplayPhotos[0] == null ? "" : DisplayPhotos[0].DisplayPath;
                               feedback.Screenshot2 = DisplayPhotos[1] == null ? "" : DisplayPhotos[1].DisplayPath;
                               feedback.Screenshot3 = DisplayPhotos[2] == null ? "" : DisplayPhotos[2].DisplayPath;
                               feedback.Screenshot4 = DisplayPhotos[3] == null ? "" : DisplayPhotos[3].DisplayPath;
                               break;
                           case 5:
                               feedback.Screenshot1 = DisplayPhotos[0] == null ? "" : DisplayPhotos[0].DisplayPath;
                               feedback.Screenshot2 = DisplayPhotos[1] == null ? "" : DisplayPhotos[1].DisplayPath;
                               feedback.Screenshot3 = DisplayPhotos[2] == null ? "" : DisplayPhotos[2].DisplayPath;
                               feedback.Screenshot4 = DisplayPhotos[3] == null ? "" : DisplayPhotos[3].DisplayPath;
                               feedback.Screenshot5 = DisplayPhotos[4] == null ? "" : DisplayPhotos[4].DisplayPath;
                               break;
                       }
                   }

                   await SubmitAsync(feedback, _feedbackService.CreateOrUpdateAsync, (result) =>
                   {
                       if (result.Success)
                       {
                           this.IssueDescribe = "";
                           this.SelectedSegment = 0;
                           this.Contacts = "";
                       }
                   });
               }
               catch (Exception)
               {
                   await ShowAlert(false, $"出错啦,内部异常！");
                   return;
               }
           });

            //拍照选择
            this.CameraPhotoCmd = ReactiveCommand.CreateFromTask<string>(async (r) =>
            {
                try
                {
                    //var resultMedia = await CrossDiaglogKit.Current.GetMediaResultAsync("请选择", "");

                    //if (resultMedia != null)
                    //{
                    //    //上传图片
                    //    using (UserDialogs.Instance.Loading("上传中..."))
                    //    {
                    //        byte[] base64Stream = Convert.FromBase64String(resultMedia.ToString());

                    //        Stream resultStream = null;

                    //        //裁切图片481X480
                    //        using (var imageEditor = await _mediaPickerService.CreateImageAsync(base64Stream))
                    //        {
                    //            imageEditor.Resize(480, 480);
                    //            resultStream = CommonHelper.BytesToStream(imageEditor.ToJpeg());
                    //        }

                    //        if (resultStream == null)
                    //        {
                    //            return;
                    //        }

                    //        var content = new MultipartFormDataContent
                    //                {
                    //                     { new StreamContent(resultStream), "\"file\"", $"\"{Settings.UserId}_feedBack.jpg\"" }
                    //                };

                    //        using (var httpClient = new HttpClient())
                    //        {
                    //            var uploadServiceBaseAddress = $"{GlobalSettings.FileCenterEndpoint}document/reomte/fileupload/HRXHJS";
                    //            var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);
                    //            var result = await httpResponseMessage.Content.ReadAsStringAsync();
                    //            var uploadResult = new UploadResult();

                    //            try
                    //            {
                    //                if (!string.IsNullOrEmpty(result))
                    //                {
                    //                    uploadResult = JsonConvert.DeserializeObject<UploadResult>(result);
                    //                }
                    //            }
                    //            catch (Exception)
                    //            {
                    //                uploadResult = null;
                    //            }
                    //            finally
                    //            {
                    //                if (httpResponseMessage != null)
                    //                    httpResponseMessage.Dispose();
                    //            }

                    //            if (uploadResult != null)
                    //            {
                    //                var displayPhoto = new DisplayPhoto
                    //                {
                    //                    DisplayPath = $"{GlobalSettings.FileCenterEndpoint}HRXHJS/document/image/" + uploadResult.Id + ""
                    //                };

                    //                DisplayPhotos.Add(displayPhoto);
                    //            }
                    //        }

                    //        if (resultStream != null)
                    //            resultStream.Dispose();
                    //    };
                    //}
                }
                catch (Exception)
                {
                    await _dialogService.ShowAlertAsync("上传失败，服务器错误！", "提示", "取消");
                }
            }, canExcute);

            //删除
            this.RemoveCommand = ReactiveCommand.Create<string>(async x =>
            {
                var ok = await _dialogService.ShowConfirmAsync("是否要删除该图片?", okText: "确定", cancelText: "取消");
                if (ok)
                {
                    var temp = DisplayPhotos.FirstOrDefault(c => c.DisplayPath == x);
                    if (temp != null)
                        DisplayPhotos.Remove(temp);
                }
            });


        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
