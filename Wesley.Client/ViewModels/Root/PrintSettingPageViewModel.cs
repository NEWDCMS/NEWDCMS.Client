using Acr.UserDialogs;
using Wesley.Client.Models;
using Wesley.Client.Models.Global;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{

    public class PrintSettingPageViewModel : ViewModelBase
    {
        private readonly IBlueToothService _blueToothService;
        [Reactive] public double DrivesHeight { get; set; }
        [Reactive] public bool EnableBluetooth { get; set; }
        [Reactive] public bool EnableSplitPrint { get; set; }

        [Reactive] public int PrintStyleSelected { get; set; } = Settings.PrintStyleSelected;

        [Reactive] public ObservableRangeCollection<Printer> Drives { get; set; } = new ObservableRangeCollection<Printer>();
        [Reactive] public AbstractBill PrintData { get; set; }
        public int RepeatPrintNum { get; set; } = 1;
        public ReactiveCommand<Printer, Unit> AdaptationCmd { get; }

        public PrintSettingPageViewModel(INavigationService navigationService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            _blueToothService = DependencyService.Get<IBlueToothService>();

            Title = "蓝牙打印机";
            this.PrintData = null;
            this.EnableBluetooth = Settings.EnableBluetooth;

            this.WhenAnyValue(x => x.PrintStyleSelected).Subscribe(x => { Settings.PrintStyleSelected = x; })
              .DisposeWith(this.DeactivateWith);

            //启用蓝牙
            this.WhenAnyValue(x => x.EnableBluetooth)
              .Subscribe(async x =>
             {
                 try
                 {
                     if (x)
                     {
                         this.Drives.Clear();
                         Settings.EnableBluetooth = true;
                         var micAccessGranted = await _blueToothService.GetPermissionsAsync();
                         if (!micAccessGranted)
                         {
                             this.EnableBluetooth = false;
                             this.Alert("请打先启用手机蓝牙功能");
                             return;
                         }

                         Device.BeginInvokeOnMainThread(async () =>
                         {
                             using (var dig = UserDialogs.Instance.Loading("扫描中..."))
                             {
                                 long timeOutTime = 8000;
                                 long curRunTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                                 long tempTime = curRunTime;
                                 while (this.Drives.Count == 0 && (curRunTime - tempTime) <= timeOutTime)
                                 {
                                     curRunTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                                     var results = _blueToothService.PairedDevices();
                                     if (results != null && results.Any())
                                     {
                                         this.DrivesHeight = results.Count() * 40.7;
                                         this.Drives = new ObservableRangeCollection<Printer>(results);
                                     }
                                     await Task.Delay(1000);
                                 }
                             }

                             if (this.Drives.Count == 0)
                             {
                                 _dialogService.LongAlert("没有扫描蓝牙到设备！");
                                 this.EnableBluetooth = false;
                                 Settings.EnableBluetooth = false;
                             }
                         });
                     }
                     else
                     {
                         Settings.EnableBluetooth = false;
                         this.EnableBluetooth = false;
                         this.Drives.Clear();
                     }
                 }
                 catch (NullReferenceException)
                 {
                     this.EnableBluetooth = false;
                     Settings.EnableBluetooth = false;
                     _dialogService.LongAlert("没有找到蓝牙设备！");
                 }
             })
              .DisposeWith(this.DeactivateWith);


            //适配
            this.AdaptationCmd = ReactiveCommand.CreateFromTask<Printer>(async (item) =>
            {
                if (!item.Status && Drives != null && Drives.Where(s => s.Status).Count() > 0)
                {
                    _dialogService.LongAlert("请先断开当前适配");
                    return;
                }

                using (var dig = UserDialogs.Instance.Loading("请稍等..."))
                {
                    try
                    {
                        await Task.Delay(500);
                        await Task.Run(() =>
                       {
                           Device.BeginInvokeOnMainThread(async () =>
                           {
                               if (!item.Status)
                               {
                                   var connected = await _blueToothService.ConnectDevice(item);
                                   if (connected)
                                   {
                                       item.Status = true;
                                   }
                               }
                               else
                               {
                                   _blueToothService.PrintStop();
                                   App.BtAddress = "";
                                   item.Status = false;
                               }
                           });
                       });
                    }
                    catch (Exception)
                    {
                        _dialogService.LongAlert("适配不到设备，请确保打印机开启");
                    }
                }
            });

            //打印测试
            this.PrintCommand = ReactiveCommand.Create(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(App.BtAddress))
                    {
                        _blueToothService.Print(this.PrintData, Settings.PrintStyleSelected, this.RepeatPrintNum);
                    }
                    else
                    {
                        _dialogService.LongAlert("请选择匹配设备！");
                    }
                }
                catch (Exception ex)
                {
                    _dialogService.LongAlert(ex.Message);
                }
            });
        }



        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                if (parameters.ContainsKey("Bill"))
                {
                    parameters.TryGetValue("Bill", out AbstractBill bill);
                    if (bill != null)
                    {
                        this.PrintData = bill;
                    }
                }

                if (parameters.ContainsKey("PrintNum"))
                {
                    parameters.TryGetValue("PrintNum", out int printNum);
                    this.RepeatPrintNum = printNum;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }

    }
}
