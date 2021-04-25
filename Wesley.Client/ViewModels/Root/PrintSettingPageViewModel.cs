using Acr.UserDialogs;
using Wesley.Client.Models;
using Wesley.Client.Models.Global;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{

    public class PrintSettingPageViewModel : ViewModelBase
    {
        private readonly IBlueToothService _blueToothService;
        [Reactive] public double DrivesHeight { get; set; }
        [Reactive] public bool EnableBluetooth { get; set; }
        [Reactive] public bool EnableSplitPrint { get; set; }

        [Reactive] public ObservableRangeCollection<Printer> Drives { get; set; } = new ObservableRangeCollection<Printer>();
        [Reactive] public Printer Selecter { get; set; }
        [Reactive] public AbstractBill PrintData { get; set; }

        public PrintSettingPageViewModel(INavigationService navigationService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            _blueToothService = DependencyService.Get<IBlueToothService>();

            Title = "蓝牙打印机";
            this.PrintData = null;
            this.EnableBluetooth = Settings.EnableBluetooth;

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
                             this.Alert("请打先启用手机蓝牙功能");
                             return;
                         }

                         using (UserDialogs.Instance.Loading("搜索中..."))
                         {
                             var results = _blueToothService.PairedDevices();
                             if (results != null && results.Any())
                             {
                                 this.DrivesHeight = results.Count() * 40.7;
                                 this.Drives = results;
                             }
                         }
                     }
                     else
                     {
                         Settings.EnableBluetooth = false;
                         this.Drives.Clear();
                     }
                 }
                 catch (NullReferenceException)
                 {
                     _dialogService.LongAlert("没有找到蓝牙设备！");
                 }
             })
              .DisposeWith(this.DeactivateWith);


            //选择设备
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
            .Skip(1)
            .Where(x => x != null)
            .SubOnMainThread(item =>
            {
                if (item != null)
                {
                    item.Selected = !item.Selected;
                    Settings.SelectedDeviceName = item.Name;
                    _dialogService.ShortAlert(item.Name);
                }
                Selecter = null;
            }, ex => _dialogService.ShortAlert(ex.ToString()));

            //打印测试
            this.PrintCommand = ReactiveCommand.Create(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.SelectedDeviceName))
                    {
                        if (this.PrintData == null)
                        {
                            _dialogService.LongAlert("数据未准备就绪！");
                            return;
                        }

                        _blueToothService.SendData(PrintData);
                        _blueToothService.Connect(Settings.SelectedDeviceName);
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
