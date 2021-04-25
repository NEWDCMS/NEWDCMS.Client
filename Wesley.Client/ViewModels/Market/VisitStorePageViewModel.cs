using Acr.UserDialogs;
using Wesley.Client.Enums;
using Wesley.Client.Models.Census;
using Wesley.Client.Models.Configuration;
using Wesley.Client.Models.Media;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Pages.Archive;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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
using Xamarin.Forms;
using PLU = Plugin.Media.Abstractions;

namespace Wesley.Client.ViewModels
{
    public class VisitStorePageViewModel : ViewModelBaseCutom
    {
        private readonly IMediaPickerService _mediaPickerService;
        private readonly LocalDatabase _conn;
        private readonly IPermissionsService _permissionsService;
        [Reactive] public VisitStore Bill { get; set; } = new VisitStore();

        [Reactive] public bool SignInEnabled { get; set; } = true;
        [Reactive] public bool SignOutEnabled { get; set; } = false;
        [Reactive] public string DistanceFormat { get; set; }
        [Reactive] public string LocationAddress { get; set; }
        [Reactive] public decimal Amount { get; set; }

        public IReactiveCommand OpenSignInCommend { get; set; }
        public IReactiveCommand CancelSignIn { get; set; }
        public IReactiveCommand SignInCommend { get; set; }
        public IReactiveCommand SignOutCommend { get; set; }
        public IReactiveCommand CameraPhotoCmd { get; set; }
        public IReactiveCommand CorrectPositionCommend { get; set; }

        public ReactiveCommand<string, Unit> InvokeAppCommand { get; }
        public ReactiveCommand<string, Unit> OpenViewCommend { get; set; }
        public ReactiveCommand<string, Unit> RemoveDisplayPathCommand { get; set; }
        public ReactiveCommand<string, Unit> RemoveStoragePathCommand { get; set; }

        public IReactiveCommand OrientationCmd { get; set; }
        public HttpClientHelper httpClientHelper;

        public VisitStorePageViewModel(INavigationService navigationService,
           IProductService productService,
           ITerminalService terminalService,
           IUserService userService,
           IWareHousesService wareHousesService,
           IAccountingService accountingService,
           IMediaPickerService mediaPickerService,
           LocalDatabase conn,
           IPermissionsService permissionsService,
           IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            _permissionsService = permissionsService;
            _mediaPickerService = mediaPickerService;
            _conn = conn;

            Title = "拜访门店";

            httpClientHelper = new HttpClientHelper();

            this.SubmitText = "\uf017";

            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    var check = await CheckSignIn();
                    this.SignInEnabled = !check;

                    if (!string.IsNullOrWhiteSpace(Settings.DisplayPhotos))
                    {
                        var displayPhotos = JsonConvert.DeserializeObject<List<DisplayPhoto>>(Settings.DisplayPhotos);
                        this.Bill.DisplayPhotos = new ObservableCollection<DisplayPhoto>(displayPhotos);
                    }

                    if (!string.IsNullOrWhiteSpace(Settings.DoorheadPhotos))
                    {
                        var doorheadPhotos = JsonConvert.DeserializeObject<List<DoorheadPhoto>>(Settings.DoorheadPhotos);
                        this.Bill.DoorheadPhotos = new ObservableCollection<DoorheadPhoto>(doorheadPhotos);
                    }

                    //重新获取客户信息
                    if (this.Terminal.Location_Lat == 0 && this.Terminal.Location_Lat == 0)
                    {
                        //获取客户
                        if (Settings.LastSigninCoustmerId > 0)
                        {
                            var tt = await _terminalService.GetTerminalAsync(Settings.LastSigninCoustmerId);
                            if (tt != null)
                            {
                                this.Terminal = tt;
                                this.Terminal.Distance = MapHelper.CalculateDistance(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0, tt.Location_Lat ?? 0, tt.Location_Lng ?? 0);
                            }
                        }
                    }

                    //有没签退信息时
                    if (this.OutVisitStore != null)
                    {
                        //刷新状态
                        Refresh(this.OutVisitStore);
                    }

                    //如果上次签到客户存在时
                    var terminalId = Settings.LastSigninCoustmerId > 0 ? Settings.LastSigninCoustmerId : Bill.TerminalId;
                    if (terminalId > 0)
                    {
                        var result = await _terminalService.GetLastVisitStoreAsync(terminalId, Settings.UserId);
                        if (result != null && result.Id > 0)
                        {
                            try
                            {
                                if (result.SigninDateTime != null)
                                {
                                    var seconds = (int)DateTime.Now.Subtract(result.SigninDateTime).TotalSeconds;
                                    var coms = CommonHelper.ConvetToSeconds(seconds);
                                    if (!string.IsNullOrEmpty(coms))
                                        this.Bill.LastSigninDateTimeName = coms;
                                }

                                if (result.LastPurchaseDate != null)
                                {
                                    var seconds = (int)DateTime.Now.Subtract(result.LastPurchaseDate).TotalSeconds;
                                    var coms = CommonHelper.ConvetToSeconds(seconds);
                                    if (!string.IsNullOrEmpty(coms))
                                        this.Bill.LastPurchaseDateTimeName = coms;
                                }

                                if (result.SigninDateTime != null)
                                    this.Bill.SigninDateTime = result.SigninDateTime;
                            }
                            catch (Exception) { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });

            //历史记录选择
            this.HistoryCommand = ReactiveCommand.Create<object>(async e => await this.NavigateAsync($"{nameof(VisitRecordsPage)}", null));

            //到店签到
            this.OpenSignInCommend = ReactiveCommand.CreateFromTask(async () =>
            {
                if (Bill.TerminalId == 0)
                {
                    await this.NavigateAsync("CurrentCustomerPage");
                    return Unit.Default;
                }

                var loc = await _permissionsService.GetLocationConsent();
                if (loc != PermissionStatus.Granted)
                {
                    await _dialogService.ShowAlertAsync("你的位置服务没有开启，请打开GPS", "定位", "确定");
                    return Unit.Default;
                }

                if (Bill.TerminalId == 0 || string.IsNullOrEmpty(Bill.TerminalName))
                {
                    this.Alert("未选择客户...");
                    return Unit.Default;
                }

                if (IsFooterVisible)
                {
                    IsVisible = true;
                    IsExpanded = true;
                    //载入位置
                    ReloadLocation();
                    IsFooterVisible = false;
                }
                else
                {
                    IsVisible = false;
                    IsExpanded = false;
                    IsFooterVisible = true;
                }

                return Unit.Default;
            });

            //取消签到
            this.CancelSignIn = ReactiveCommand.Create(() =>
            {
                IsVisible = false;
                IsExpanded = false;
                IsFooterVisible = true;
            });

            //签到
            this.SignInCommend = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    if (!IsFastClick())
                        return Unit.Default;

                    bool continueTodo = true;
                    if ((Terminal.Distance ?? 0) > 50)
                    {
                        continueTodo = await UserDialogs.Instance.ConfirmAsync($"你确定要在{Terminal.Distance.Value:#.00}米外签到吗？", "警告", cancelText: "不签到", okText: "继续签到");
                        Bill.Abnormal = true;
                        Bill.Distance = Math.Round(Terminal.Distance.Value, 2);
                    }


                    var lat = GlobalSettings.Latitude ?? 0;
                    var lan = GlobalSettings.Longitude ?? 0;


                    if (!continueTodo)
                    {
                        return Unit.Default;
                    }

                    //签到
                    Bill.StoreId = Settings.StoreId;
                    Bill.BusinessUserId = Settings.UserId;
                    Bill.BusinessUserName = Settings.UserRealName;
                    Bill.ChannelId = Terminal.ChannelId;
                    Bill.DistrictId = Terminal.DistrictId;
                    Bill.SigninDateTimeEnable = true;
                    Bill.SigninDateTime = DateTime.Now;
                    Bill.SignOutDateTime = DateTime.Now;
                    Bill.VisitTypeId = 2;//计划内
                    Bill.SignTypeId = 1;
                    Bill.SignType = Enums.SignEnum.CheckIn;
                    Bill.Remark = LocationAddress;

                    //获取坐标
                    Bill.Latitude = lat;
                    Bill.Longitude = lan;

                    return await SubmitAsync(Bill, _terminalService.SignInVisitStoreAsync, (result) =>
                    {
                        if (!result.Success)
                        {
                            this.IsVisible = false;
                            this.IsExpanded = false;
                            this.IsFooterVisible = true;
                        }
                        else
                        {
                            if (result.Data is VisitStore data)
                            {
                                this.SignInEnabled = false;
                                this.SignOutEnabled = true;
                                this.OutVisitStore = null;

                                //drawer
                                this.IsVisible = false;
                                this.IsExpanded = false;
                                this.IsFooterVisible = true;

                                //记录下签到ID
                                Settings.LastSigninId = data.Id;
                                Settings.LastSigninCoustmerId = Bill.TerminalId;
                                Settings.LastSigninCoustmerName = Bill.TerminalName;

                                this.Bill.LastSigninDateTime = data.LastSigninDateTime;
                                this.Bill.LastPurchaseDateTime = data.LastPurchaseDateTime;
                            }
                        }

                    }, goBack: false);
                }
                catch (Exception)
                {
                    this.IsVisible = false;
                    this.IsExpanded = false;
                    this.IsFooterVisible = true;

                    await ShowAlert(false, $"出错啦,内部异常！");
                    return Unit.Default;
                }
            });

            //校准位置
            this.CorrectPositionCommend = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    var lat = GlobalSettings.Latitude ?? 0;
                    var lan = GlobalSettings.Longitude ?? 0;

                    if (Terminal == null || Terminal.Id == 0)
                    {
                        await ShowAlert(false, $"无效操作！");
                        return;
                    }

                    if (lat != 0 && lan != 0)
                    {
                        var distance = MapHelper.CalculateDistance(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0, this.Terminal?.Location_Lat ?? 0, this.Terminal?.Location_Lng ?? 0);
                        Terminal.Location_Lat = GlobalSettings.Latitude;
                        Terminal.Location_Lng = GlobalSettings.Longitude;
                        Terminal.Distance = distance;
                        await _terminalService.UpdateterminalAsync(Terminal.Id, GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0);
                    }

                    await ShowAlert(true, $"校准成功！");
                }
                catch (Exception)
                {
                    await ShowAlert(false, $"出错啦,内部异常！");
                }
            });


            //离店签退
            this.SignOutCommend = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    int onStoreStopSeconds = 3;
                    double subtract = DateTime.Now.Subtract(Bill?.SigninDateTime ?? DateTime.Now).TotalMinutes;
                    if (!string.IsNullOrEmpty(Settings.CompanySetting))
                    {
                        var companySetting = JsonConvert.DeserializeObject<CompanySettingModel>(Settings.CompanySetting);
                        if (companySetting != null)
                        {
                            onStoreStopSeconds = companySetting.OnStoreStopSeconds;
                        }
                    }


                    if (Settings.LastSigninId == 0)
                    {
                        this.Alert("签退无效,无法确定记录");
                        return Unit.Default;
                    }

                    if (Bill.TerminalId == 0 || string.IsNullOrEmpty(Bill.TerminalName))
                    {
                        this.Alert("选择客户");
                        return Unit.Default;
                    }

                    if (Bill.DoorheadPhotos == null || Bill.DoorheadPhotos.Count == 0)
                    {
                        this.Alert("请拍摄门头照片");
                        return Unit.Default;
                    }

                    if (Bill.DisplayPhotos == null || Bill.DisplayPhotos.Count == 0)
                    {
                        this.Alert("请拍摄陈列照片");
                        return Unit.Default;
                    }

                    if (BillId > 0)
                    {
                        switch (BillType)
                        {
                            case BillTypeEnum.SaleReservationBill:
                                this.Bill.SaleReservationBillId = BillId;
                                this.Bill.SaleOrderAmount = Amount;
                                break;
                            case BillTypeEnum.SaleBill:
                                this.Bill.SaleBillId = BillId;
                                this.Bill.SaleAmount = Amount;
                                break;
                            case BillTypeEnum.ReturnReservationBill:
                                this.Bill.ReturnReservationBillId = BillId;
                                this.Bill.ReturnOrderAmount = Amount;
                                break;
                            case BillTypeEnum.ReturnBill:
                                this.Bill.ReturnBillId = BillId;
                                this.Bill.ReturnAmount = Amount;
                                break;
                        }
                    }

                    //签退
                    Bill.Id = Settings.LastSigninId;
                    Bill.SignOutDateTime = DateTime.Now;
                    Bill.SignTypeId = 2;

                    await SubmitAsync(Bill, _terminalService.SignOutVisitStoreAsync, (result) =>
                    {
                        if (result.Success)
                        {
                            this.SignOutEnabled = false;
                            this.Bill.SigninDateTimeEnable = false;
                            this.OutVisitStore = null;
                            Settings.LastSigninId = 0;
                            Settings.LastSigninCoustmerId = 0;
                            Settings.LastSigninCoustmerName = "";
                            //添加记录
                            _conn.InsertAsync(this.Bill);

                        }
                    });

                    await _navigationService.GoBackAsync();

                    return Unit.Default;

                }
                catch (Exception)
                {
                    await ShowAlert(false, $"出错啦,内部异常！");
                    return Unit.Default;
                }
                finally
                {
                    Settings.DisplayPhotos = "";
                    Settings.DoorheadPhotos = "";
                }
            });

            //应用选择执行
            this.InvokeAppCommand = ReactiveCommand.CreateFromTask<string>(async (r) =>
            {
                try
                {
                    if (Settings.LastSigninId != 0 && Settings.LastSigninCoustmerId != 0)
                    {
                        await this.NavigateAsync(r.ToString(),
                            ("TerminalId", Bill.TerminalId),
                            ("TerminalName", Bill.TerminalName));
                    }
                    else
                    {
                        await ShowAlert(false, "需要先签到后操作");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });

            //拍照选择
            this.CameraPhotoCmd = ReactiveCommand.Create<string>((r) =>
            {
                if (!IsFastClick())
                    return;

                RapidTapPreventor(async () =>
                {
                    await CrossMedia.Current.Initialize();

                    if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
                    {
                        await _dialogService.ShowAlertAsync("抱歉,没有检测到相机...", "提示", "取消");
                        return;
                    }
                    else
                    {
                        if (GlobalSettings.IsNotConnected)
                        {
                            _dialogService.LongAlert("操作失败，没有检测到网络！");
                            return;
                        }

                        switch (r)
                        {
                            case "DoorheadPhotos":
                                await TakePhotograph((u, m) =>
                                {
                                    var photo = new DoorheadPhoto
                                    {
                                        StoragePath = $"{GlobalSettings.FileCenterEndpoint}HRXHJS/document/image/" + u.Id + "",
                                        ThumbnailPhoto = Xamarin.Forms.ImageSource.FromStream(() =>
                                        {
                                            var stream = m?.GetStream();
                                            return stream;
                                        })
                                    };
                                    this.Bill.DoorheadPhotos.Add(photo);
                                    Settings.DoorheadPhotos = JsonConvert.SerializeObject(this.Bill.DoorheadPhotos);
                                });
                                break;
                            case "DisplayPhotos":
                                await TakePhotograph((u, m) =>
                                {
                                    var photo = new DisplayPhoto
                                    {
                                        DisplayPath = $"{GlobalSettings.FileCenterEndpoint}HRXHJS/document/image/" + u.Id + "",
                                        ThumbnailPhoto = Xamarin.Forms.ImageSource.FromStream(() =>
                                        {
                                            var stream = m?.GetStream();
                                            return stream;
                                        })
                                    };
                                    this.Bill.DisplayPhotos.Add(photo);
                                    Settings.DisplayPhotos = JsonConvert.SerializeObject(this.Bill.DisplayPhotos);
                                });
                                break;
                        }
                    }
                });
            });


            //删除照片
            this.RemoveDisplayPathCommand = ReactiveCommand.Create<string>(async x =>
            {
                var ok = await _dialogService.ShowConfirmAsync("是否要删除该图片?", okText: "确定", cancelText: "取消");
                if (ok)
                {
                    var temp = this.Bill.DisplayPhotos.FirstOrDefault(s => s.DisplayPath == x);
                    if (temp != null)
                    {
                        this.Bill.DisplayPhotos.Remove(temp);
                        Settings.DisplayPhotos = JsonConvert.SerializeObject(this.Bill.DisplayPhotos);
                    }
                }
            });
            this.RemoveStoragePathCommand = ReactiveCommand.Create<string>(async x =>
            {
                var ok = await _dialogService.ShowConfirmAsync("是否要删除该图片?", okText: "确定", cancelText: "取消");
                if (ok)
                {
                    var temp = this.Bill.DoorheadPhotos.FirstOrDefault(s => s.StoragePath == x);
                    if (temp != null)
                    {
                        this.Bill.DoorheadPhotos.Remove(temp);
                        Settings.DoorheadPhotos = JsonConvert.SerializeObject(this.Bill.DoorheadPhotos);
                    }
                }
            });

            //定位
            this.OrientationCmd = ReactiveCommand.Create(() =>
           {
               Device.BeginInvokeOnMainThread(() =>
               {
                   ReloadLocation();
               });
           });


            //预览照片
            this.OpenViewCommend = ReactiveCommand.Create<string>(async (r) =>
           {
               try
               {
                   if (string.IsNullOrEmpty(r))
                   {
                       await this.ShowAlert(true, "无效图片链接！");
                       return;
                   }

                   var images = new List<DisplayPhoto>
                   {
                        new DisplayPhoto() { DisplayPath = r }
                   };
                   await this.NavigateAsync("ImageViewerPage", ("ImageInfos", images));
               }
               catch (Exception ex)
               {
                   Crashes.TrackError(ex);
               }
           });

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        /// <summary>
        /// 陈列/门头拍照
        /// </summary>
        /// <returns></returns>
        private async Task TakePhotograph(Action<UploadResult, MediaFile> action)
        {
            try
            {
                var mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    CompressionQuality = 60,
                    PhotoSize = PhotoSize.Medium,
                    Location = new PLU.Location
                    {
                        Latitude = GlobalSettings.Latitude ?? 0,
                        Longitude = GlobalSettings.Longitude ?? 0
                    }
                });

                if (mediaFile == null)
                {
                    return;
                }

                Stream convertStream = mediaFile.GetStream();

                //上传图片
                using (UserDialogs.Instance.Loading("上传中..."))
                {
                    try
                    {
                        var content = new MultipartFormDataContent
                        {
                                { new StreamContent(convertStream),
                                "\"file\"", $"\"{mediaFile?.Path}\"" }
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
                            action.Invoke(uploadResult, mediaFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                    finally
                    {
                        if (mediaFile != null)
                            mediaFile.Dispose();

                        if (convertStream != null)
                            convertStream.Dispose();
                    }
                };
            }
            catch (Exception ex)
            {
                _dialogService.LongAlert(ex.Message);
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                if (parameters.ContainsKey("Terminaler"))
                {
                    parameters.TryGetValue("Terminaler", out TerminalModel tt);
                    if (tt != null)
                    {
                        this.Terminal = tt;
                        this.Terminal.Id = tt.Id;
                        this.Bill.SigninDateTimeEnable = false;
                        this.Bill.TerminalId = Terminal.Id;
                        this.Bill.TerminalName = Terminal.Name;
                    }
                }

                if (parameters.ContainsKey("BillTypeId") && parameters.ContainsKey("BillId") && parameters.ContainsKey("Amount")) //拜访开单
                {
                    parameters.TryGetValue<int>("BillId", out int billId);
                    parameters.TryGetValue<BillTypeEnum>("BillTypeId", out BillTypeEnum billTypeId);
                    parameters.TryGetValue<decimal>("Amount", out decimal amount);
                    BillType = billTypeId;
                    BillId = billId;
                    Amount = amount;
                }

                if (this.Terminal != null)
                {
                    ((ICommand)Load)?.Execute(null);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public async void ReloadLocation()
        {
            try
            {
                var gps = await _conn.LocationSyncEvents.OrderByDescending(s => s.Id)?.FirstOrDefaultAsync();
                if (gps != null)
                {
                    var distance = MapHelper.CalculateDistance(GlobalSettings.Latitude ?? 0, GlobalSettings.Longitude ?? 0, this.Terminal?.Location_Lat ?? 0, this.Terminal?.Location_Lng ?? 0);
                    this.DistanceFormat = $"当前距离店铺{distance:#0.00}米.";
                    this.LocationAddress = string.IsNullOrEmpty(gps.Address) ? "没有获取到位置信息！" : gps.Address;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void Refresh(VisitStore result)
        {
            //是否有最近签到
            if (result.Id != 0)
            {
                Settings.LastSigninId = result.Id;
                this.Bill.TerminalId = result.TerminalId;
                this.Bill.TerminalName = result.TerminalName;

                //已经签到
                if (result.SignTypeId == 1)
                {
                    this.SignInEnabled = false;
                    this.SignOutEnabled = true;
                    //显示签到时间
                    this.Bill.SigninDateTimeEnable = true;
                }
                //已经签退
                else if (result.SignTypeId == 2)
                {
                    this.SignInEnabled = true;
                    this.SignOutEnabled = false;
                }

                if (result.SigninDateTime != null)
                {
                    var name = CommonHelper.ConvetToSeconds((int)DateTime.Now.Subtract(result.SigninDateTime).TotalSeconds);

                    if (!string.IsNullOrEmpty(name))
                        this.Bill.LastSigninDateTimeName = name;
                }

                if (result.LastPurchaseDate != null)
                {
                    var name = CommonHelper.ConvetToSeconds((int)DateTime.Now.Subtract(result.LastPurchaseDate).TotalSeconds);

                    if (!string.IsNullOrEmpty(name))
                        this.Bill.LastPurchaseDateTimeName = name;
                }
                this.Bill.SigninDateTime = result.SigninDateTime;
            }
            else
            {
                this.SignInEnabled = true;
                this.SignOutEnabled = false;
                this.Bill.SigninDateTimeEnable = false;
            }
        }

        public override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                _permissionsService?.RequestLocationAndCameraPermission();

            }
            catch (Exception ex)
            {

                Crashes.TrackError(ex);
            }
        }
    }
}

