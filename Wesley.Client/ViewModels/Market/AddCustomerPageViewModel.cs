using Acr.UserDialogs;
using Wesley.Client.CustomViews;
using Wesley.Client.Enums;
using Wesley.Client.Models.Census;
using Wesley.Client.Models.Media;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.Media;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;

using System;
using System.IO;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Wesley.Client.ViewModels
{
    public class AddCustomerPageViewModel : ViewModelBaseCutom
    {
        private readonly IPermissionsService _permissionsService;
        [Reactive] public TerminalModel PostMData { get; set; } = new TerminalModel();
        public ReactiveCommand<object, Unit> ChannelSelected { get; set; }
        public ReactiveCommand<object, Unit> LineSelected { get; set; }
        public ReactiveCommand<object, Unit> RankSelected { get; set; }
        public ReactiveCommand<object, Unit> AddressSelected { get; set; }
        public ReactiveCommand<object, Unit> AddSaveCommand { get; }
        public ICommand UploadFaceCommand { get; set; }
        [Reactive] public string LocationAddress { get; set; }
        public HttpClientHelper httpClientHelper;

        public ReactiveCommand<object, Unit> EntryNameUnfocused { get; }
        public ReactiveCommand<object, Unit> EntryUnfocused { get; }

        public AddCustomerPageViewModel(INavigationService navigationService,
            ITerminalService terminalService,
            IProductService productService,
            IUserService userService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IPermissionsService permissionsService,
            IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "添加客户档案";

            _permissionsService = permissionsService;
            httpClientHelper = new HttpClientHelper();

            this.EntryUnfocused = ReactiveCommand.Create<object>(e =>
            {
                Storage();
            });

            this.EntryNameUnfocused = ReactiveCommand.Create<object>(async e =>
            {
                var s = PostMData.Name;
                var result = await _terminalService.CheckTerminalAsync(s);
                if (result)
                {
                    _dialogService.LongAlert("终端名称已经存在！");
                    PostMData.Name = "";
                }
            });

            //片区选择
            this.DistrictSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectDistrict((data) =>
                 {
                     PostMData.DistrictId = data.Id;
                     PostMData.DistrictName = data.Name;
                     Storage();
                 });
            });

            //渠道选择
            this.ChannelSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectChannel((data) =>
                {
                    PostMData.ChannelId = data.Id;
                    PostMData.ChannelName = data.Name;
                    Storage();
                });

            });

            //线路选择
            this.LineSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectLine((data) =>
                 {
                     PostMData.LineId = data.Id;
                     PostMData.LineName = data.Name;
                     Storage();
                 });
            });

            //地图定位
            this.AddressSelected = ReactiveCommand.Create<object>(async e =>
           {
               await this.NavigateAsync("SelectLocationPage");
           });

            //客户等级
            this.RankSelected = ReactiveCommand.Create<object>(async e =>
            {
                await SelectRank((data) =>
                 {
                     PostMData.RankId = data.Id;
                     PostMData.RankName = data.Name;
                     Storage();
                 });
            });

            //验证
            var valid_Photo = this.ValidationRule(x => x.PostMData.DoorwayPhoto, _isDefined, "请添加门头照片");
            var valid_Name = this.ValidationRule(x => x.PostMData.Name, _isDefined, "客户名称未指定");
            var valid_BossName = this.ValidationRule(x => x.PostMData.BossName, _isDefined, "老板姓名未指定");
            var valid_BossCall = this.ValidationRule(x => x.PostMData.BossCall, _isDefined, "联系电话未指定");
            var valid_ChannelId = this.ValidationRule(x => x.PostMData.ChannelId, _isZero, "渠道未指定");
            var valid_Address = this.ValidationRule(x => x.PostMData.Address, _isDefined, "请填写详细地址");

            //保存
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                await this.Access(AccessGranularityEnum.EndPointListSave);

                if (PostMData.DoorwayPhoto.Equals("nophoto.png"))
                {
                    _dialogService.LongAlert("请添加门头照片！");
                    return Unit.Default;
                }

                var postMData = Storage();
                return await SubmitAsync(postMData, _terminalService.CreateOrUpdateAsync, (result) =>
               {
                   GlobalSettings.TempAddCustomerStore = null;
                   PostMData = new TerminalModel();
               });
            }, this.IsValid());

            this.AddSaveCommand = ReactiveCommand.Create<object>(e =>
            {
                ((ICommand)SubmitDataCommand)?.Execute(null);
            }, this.IsValid());

            //上传门头
            this.UploadFaceCommand = ReactiveCommand.Create(() =>
            {
                Stream convertStream = null;
                RapidTapPreventor(async () =>
                {
                    await CrossMedia.Current.Initialize();

                    if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
                    {
                        await _dialogService.ShowAlertAsync("抱歉,没有检测到相机...", "提示", "取消");
                        return;
                    }

                    if (GlobalSettings.IsNotConnected)
                    {
                        _dialogService.LongAlert("操作失败，没有检测到网络！");
                        return;
                    }

                    var resultMedia = await CrossDiaglogKit.Current.GetMediaResultAsync("请选择", "");
                    if (resultMedia != null)
                    {
                        byte[] base64Stream = Convert.FromBase64String(resultMedia.ToString());
                        convertStream = new MemoryStream(base64Stream);
                        if (convertStream == null)
                        {
                            return;
                        }

                        //上传图片
                        using (UserDialogs.Instance.Loading("上传中..."))
                        {
                            try
                            {
                                var content = new MultipartFormDataContent
                                {
                                    { new StreamContent(convertStream), "\"file\"", $"\"face.jpg\"" }
                                };

                                var url = $"{GlobalSettings.FileCenterEndpoint}document/reomte/fileupload/HRXHJS";
                                var result = await httpClientHelper.PostAsync(url, content);
                                await Task.Delay(1000);
                                var uploadResult = new UploadResult();

                                if (!string.IsNullOrEmpty(result))
                                {
                                    uploadResult = JsonConvert.DeserializeObject<UploadResult>(result);
                                }

                                if (uploadResult != null)
                                {
                                    var facePath = $"{GlobalSettings.FileCenterEndpoint}HRXHJS/document/image/" + uploadResult.Id + "";
                                    PostMData.DoorwayPhoto = facePath;
                                }
                            }
                            catch (Exception ex)
                            {
                                Crashes.TrackError(ex);
                            }
                            finally
                            {
                                if (convertStream != null)
                                    convertStream.Dispose();
                            }
                        };
                    }
                });
            });

            this.DistrictSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.ChannelSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.LineSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.AddressSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.RankSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.AddSaveCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.ExceptionsSubscribe();
        }

        private TerminalModel Storage()
        {
            try
            {
                var postMData = new TerminalModel()
                {
                    Id = PostMData.Id,
                    //客户名称
                    Name = PostMData.Name,
                    //老板姓名
                    BossName = PostMData.BossName,
                    //门头
                    DoorwayPhoto = PostMData.DoorwayPhoto,
                    //联系电话
                    BossCall = PostMData.BossCall,
                    //片区
                    DistrictId = PostMData.DistrictId,
                    DistrictName = PostMData.DistrictName,
                    //渠道
                    ChannelId = PostMData.ChannelId,
                    ChannelName = PostMData.ChannelName,
                    //线路
                    LineId = PostMData.LineId,
                    LineName = PostMData.LineName,
                    //客户等级
                    RankId = PostMData.RankId,
                    RankName = PostMData.RankName,
                    //经度坐标
                    Location_Lng = PostMData.Location_Lng ?? GlobalSettings.Longitude ?? 0,
                    //纬度坐标
                    Location_Lat = PostMData.Location_Lat ?? GlobalSettings.Latitude ?? 0,
                    //终端编码
                    Code = PostMData.Code,
                    //营业编号
                    BusinessNo = PostMData.BusinessNo,
                    //地址
                    Address = PostMData.Address,
                    //备注
                    Remark = PostMData.Remark,
                    FoodBusinessLicenseNo = "",
                    EnterpriseRegNo = "",
                    MaxAmountOwed = 0,
                    Status = true,
                    Deleted = false,
                    //协议
                    IsAgreement = PostMData.IsAgreement,
                    Cooperation = PostMData.Cooperation,
                    IsDisplay = PostMData.IsDisplay,
                    IsVivid = PostMData.IsVivid,
                    IsPromotion = PostMData.IsPromotion,
                    OtherRamark = PostMData.OtherRamark
                };
                GlobalSettings.TempAddCustomerStore = postMData;
                return postMData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return new TerminalModel();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                if (this.Edit)
                {
                    Title = "编辑客户档案";
                    this.PostMData = this.Terminal;
                }
                else
                {
                    if (GlobalSettings.TempAddCustomerStore != null)
                    {
                        this.PostMData = GlobalSettings.TempAddCustomerStore;
                    }
                }

                this.PostMData.DoorwayPhoto = string.IsNullOrEmpty(this.PostMData.DoorwayPhoto) ? "nophoto.png" : this.PostMData.DoorwayPhoto;

                if (parameters.ContainsKey("AddGpsEvent"))
                {
                    parameters.TryGetValue("AddGpsEvent", out TrackingModel gps);
                    PostMData.Address = string.IsNullOrEmpty(gps.Address) ? "没有获取到位置信息！" : gps.Address;
                    PostMData.Location_Lat = gps.Latitude;
                    PostMData.Location_Lng = gps.Longitude;
                }

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            _permissionsService?.RequestLocationAndCameraPermission();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
