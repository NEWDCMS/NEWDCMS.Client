using Acr.UserDialogs;
using DCMS.Client.Enums;
using DCMS.Client.Models.Census;
using DCMS.Client.Models.Configuration;
using DCMS.Client.Models.Media;
using DCMS.Client.Models.Terminals;
using DCMS.Client.Pages.Archive;
using DCMS.Client.Services;
using DCMS.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Threading;

namespace DCMS.Client.ViewModels
{
    public class VisitStorePageViewModel : ViewModelBaseCutom
    {
        private readonly IMediaPickerService _mediaPickerService;
        private readonly ILiteDbService<TrackingModel> _conn;
        private readonly ILiteDbService<VisitStore> _vsdb;
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

        [Reactive] public DoorheadPhoto DoorheadPhotoSelecter { get; set; }
        [Reactive] public DisplayPhoto DisplayPhotoSelecter { get; set; }

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
           ILiteDbService<TrackingModel> conn,
           ILiteDbService<VisitStore> vsdb,
           IPermissionsService permissionsService,
           IDialogService dialogService) : base(navigationService, 
               productService, 
               terminalService, 
               userService, 
               wareHousesService, 
               accountingService, 
               dialogService)
        {
            _permissionsService = permissionsService;
            _mediaPickerService = mediaPickerService;

            _conn = conn;
            _vsdb = vsdb;

            Title = "拜访门店";

            httpClientHelper = new HttpClientHelper();

            this.SubmitText = "\uf017";

            this.Load = ReactiveCommand.Create(async () =>
            {
                try
                {
                    var check = await CheckSignIn();
                    this.SignInEnabled = !check;

                    if (!string.IsNullOrWhiteSpace(Settings.DisplayPhotos))
                    {
                        var displayPhotos = JsonConvert.DeserializeObject<List<DisplayPhoto>>(Settings.DisplayPhotos);
                        if (displayPhotos != null)
                            this.Bill.DisplayPhotos = new ObservableCollection<DisplayPhoto>(displayPhotos);
                    }

                    if (!string.IsNullOrWhiteSpace(Settings.DoorheadPhotos))
                    {
                        var doorheadPhotos = JsonConvert.DeserializeObject<List<DoorheadPhoto>>(Settings.DoorheadPhotos);
                        if (doorheadPhotos != null)
                            this.Bill.DoorheadPhotos = new ObservableCollection<DoorheadPhoto>(doorheadPhotos);
                    }

                    //重新获取客户信息
                    if (Settings.LastSigninCoustmerId > 0)
                    {
                        var tt = await _terminalService.GetTerminalAsync(Settings.LastSigninCoustmerId);
                        if (tt != null)
                        {
                            this.Terminal = tt;
                            Bill.TerminalId = tt.Id;
                            Bill.TerminalName = tt.Name;
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

                        //获取终端余额
                        _terminalService.Rx_GetTerminalBalance(terminalId, new CancellationToken())?.Subscribe((balance) =>
                        {
                            if (balance != null)
                                this.TBalance = balance;

                        }).DisposeWith(DeactivateWith);


                        //获取上次拜访信息
                        _terminalService.Rx_GetLastVisitStoreAsync(terminalId, Settings.UserId, new CancellationToken())?.Subscribe((result) =>
                    {
                        if (result != null && result.Id > 0)
                        {
                            try
                            {
                                    //上次签到时间
                                    if (result.SigninDateTime != null)
                                {
                                    var seconds = (int)DateTime.Now.Subtract(result.SigninDateTime).TotalSeconds;
                                    var coms = CommonHelper.ConvetToSeconds(seconds);

                                    if (!string.IsNullOrEmpty(coms))
                                        this.Bill.LastSigninDateTimeName = coms;
                                }

                                    //上次采购时间
                                    if (result.LastPurchaseDate != null)
                                {
                                    var seconds = (int)DateTime.Now.Subtract(result.LastPurchaseDate).TotalSeconds;
                                    var coms = CommonHelper.ConvetToSeconds(seconds);
                                    if (!string.IsNullOrEmpty(coms))
                                        this.Bill.LastPurchaseDateTimeName = coms;
                                }

                                if (this.OutVisitStore == null && result.SigninDateTime != null)
                                    this.Bill.SigninDateTime = result.SigninDateTime;


                            }
                            catch (Exception) { }
                        }
                    }).DisposeWith(DeactivateWith);

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
                    if (Terminal.CalcDistance() > 50)
                    {
                        continueTodo = await UserDialogs.Instance.ConfirmAsync($"你确定要在{Terminal.Distance:#.00}米外签到吗？", "警告", cancelText: "不签到", okText: "继续签到");
                        Bill.Abnormal = true;
                        Bill.Distance = Math.Round(Terminal.Distance, 2);
                    }


                    var lat = GlobalSettings.Latitude ?? 0;
                    var lan = GlobalSettings.Longitude ?? 0;


                    if (!continueTodo)
                    {
                        return Unit.Default;
                    }

                    //签到
                    Bill.Id = 0;
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

                    return await SubmitAsync(Bill, _terminalService.SignInVisitStoreAsync, async (result) =>
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

                                //添加签到记录
                                try
                                {
                                    Terminal.Id = Bill.TerminalId;
                                    Terminal.LastSigninDateTimeName = data.SigninDateTime.ToString();
                                    Terminal.SigninDateTime = data.SigninDateTime;
                                    await _terminalService.AddTerminal(Terminal);
                                }
                                catch (Exception) { }
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
                    int onStoreStopSeconds = 0;
                    double subtract = DateTime.Now.Subtract(Bill?.SigninDateTime ?? DateTime.Now).TotalMinutes;
                    if (!string.IsNullOrEmpty(Settings.CompanySetting))
                    {
                        var companySetting = JsonConvert.DeserializeObject<CompanySettingModel>(Settings.CompanySetting);
                        if (companySetting != null)
                        {
                            onStoreStopSeconds = companySetting.OnStoreStopSeconds;
                        }
                    }

                    if (!SignOutEnabled)
                    {
                        this.Alert("还没签到哦！");
                        return Unit.Default;
                    }

                    if (onStoreStopSeconds > 0 && subtract < onStoreStopSeconds)
                    {
                        this.Alert($"签退无效,拜访在店时间必须大于{onStoreStopSeconds}分钟");
                        return Unit.Default;
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

                    await SubmitAsync(Bill, _terminalService.SignOutVisitStoreAsync,  async (result) =>
                    {
                        if (result.Success)
                        {
                            this.SignOutEnabled = false;
                            this.Bill.SigninDateTimeEnable = false;
                            this.OutVisitStore = null;

                            Settings.LastSigninId = 0;
                            Settings.LastSigninCoustmerId = 0;
                            Settings.LastSigninCoustmerName = "";

                            Settings.DisplayPhotos = "";
                            Settings.DoorheadPhotos = "";

                            //更新签到记录
                            try
                            {
                                Terminal.Id = Bill.TerminalId;
                                Terminal.SignOutDateTime = Bill.SignOutDateTime;
                                await _terminalService.UpdateTerminal(Terminal);
                            }catch (Exception ) { }
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
                            ("TerminalName", Bill.TerminalName),
                            ("Reference", this.PageName));
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
            this.CameraPhotoCmd = ReactiveCommand.CreateFromTask<string>(async (r) =>
            {
                if (!IsFastClick())
                    return;

                if (this.SignInEnabled)
                {
                    this.Alert("还没有签到哦！");
                }
                else 
                {
                    await this.NavigateAsync("CameraViewPage", ("TakeType", r));
                }
            });

            //删除门头照片
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

            //删除陈列照片
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

            //定位
            this.OrientationCmd = ReactiveCommand.Create(() =>
           {
               Device.BeginInvokeOnMainThread(() =>
               {
                   ReloadLocation();
               });
           });

            //预览照片
            this.WhenAnyValue(x => x.DoorheadPhotoSelecter)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Skip(1)
                .Where(x => x != null)
                .SubOnMainThread(async item =>
                {
                    var images = new List<string> { item.StoragePath };
                    await this.NavigateAsync("ImageViewerPage", ("ImageInfos", images));
                    DoorheadPhotoSelecter = null;
                }).DisposeWith(DeactivateWith);

            this.WhenAnyValue(x => x.DisplayPhotoSelecter)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Skip(1)
                .Where(x => x != null)
                .SubOnMainThread(async item =>
                {
                    var images = new List<string> { item.DisplayPath };
                    await this.NavigateAsync("ImageViewerPage", ("ImageInfos", images));
                    DisplayPhotoSelecter = null;
                }).DisposeWith(DeactivateWith);

            this.BindBusyCommand(Load);


            //拍照上传
            MessageBus
                .Current
                .Listen<byte[]?>(string.Format(Constants.CAMERA_KEY, "DoorheadPhotos"))
                .Subscribe(bit =>
                {
                    if (bit != null && bit.Length > 0)
                    {
                        UploadPhotograph((u) =>
                        {
                            var photo = new DoorheadPhoto
                            {
                                StoragePath = $"{GlobalSettings.FileCenterEndpoint}HRXHJS/document/image/" + u.Id + ""
                            };
                            this.Bill.DoorheadPhotos.Add(photo);
                            Settings.DoorheadPhotos = JsonConvert.SerializeObject(this.Bill.DoorheadPhotos);
                        }, new MemoryStream(bit));
                    }
                }).DisposeWith(DeactivateWith);


            MessageBus
               .Current
               .Listen<byte[]?>(string.Format(Constants.CAMERA_KEY, "DisplayPhotos"))
               .Subscribe(bit =>
               {
                   if (bit != null && bit.Length > 0)
                   {
                       UploadPhotograph((u) =>
                       { 
                           var photo = new DisplayPhoto
                           {
                               DisplayPath = $"{GlobalSettings.FileCenterEndpoint}HRXHJS/document/image/" + u.Id + ""
                           };
                           this.Bill.DisplayPhotos.Add(photo);
                           Settings.DisplayPhotos = JsonConvert.SerializeObject(this.Bill.DisplayPhotos);
                       }, new MemoryStream(bit));
                   }
               }).DisposeWith(DeactivateWith);

            this.Load.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.HistoryCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.OpenSignInCommend.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.CancelSignIn.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.SignInCommend.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.CorrectPositionCommend.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.SignOutCommend.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.InvokeAppCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.CameraPhotoCmd.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.RemoveStoragePathCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.RemoveDisplayPathCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.OrientationCmd.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
        }

        private async void UploadPhotograph(Action<UploadResult> action, MemoryStream stream)
        {
            //上传图片
            using (UserDialogs.Instance.Loading("上传中..."))
            {
                try
                {
                    if (stream != null)
                    {
                        var scb = new StreamContent(stream);
                        var content = new MultipartFormDataContent { { scb, "\"file\"", $"\"takephotograph.jpg\"" } };
                        var url = $"{GlobalSettings.FileCenterEndpoint}document/reomte/fileupload/HRXHJS";
                        var result = await httpClientHelper.PostAsync(url, content);

                        var uploadResult = new UploadResult();

                        if (!string.IsNullOrEmpty(result))
                        {
                            uploadResult = JsonConvert.DeserializeObject<UploadResult>(result);
                        }

                        if (uploadResult != null)
                        {
                            action.Invoke(uploadResult);
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
                        this.Bill.TerminalId = tt.Id;
                        this.Bill.TerminalName = tt.Name;
                    }
                }

                if (parameters.ContainsKey("BillTypeId") && parameters.ContainsKey("BillId") && parameters.ContainsKey("Amount")) //拜访开单
                {
                    parameters.TryGetValue<int>("BillId", out int billId);
                    parameters.TryGetValue("BillTypeId", out BillTypeEnum billTypeId);
                    parameters.TryGetValue<decimal>("Amount", out decimal amount);

                    BillType = billTypeId;
                    BillId = billId;
                    Amount = amount;
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
                var data = await _conn.Table.FindAllAsync();
                var gps = data.OrderByDescending(s => s.Id)?.FirstOrDefault();
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
            if (result != null && result.Id != 0)
            {
                Settings.LastSigninId = result.Id;
                this.Bill.TerminalId = result.TerminalId;
                this.Bill.TerminalName = result.TerminalName;

                //已经签到
                if (result.SignTypeId == 1)
                {
                    this.SignInEnabled = false;
                    this.SignOutEnabled = true;
                    this.Bill.SignType = SignEnum.CheckIn;

                    //显示签到时间
                    this.Bill.SigninDateTimeEnable = true;
                }
                //已经签退
                else if (result.SignTypeId == 2)
                {
                    this.SignInEnabled = true;
                    this.SignOutEnabled = false;
                    this.Bill.SignType = SignEnum.Signed;
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
    }
}

