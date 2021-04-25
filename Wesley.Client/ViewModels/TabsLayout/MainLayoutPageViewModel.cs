using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;
using Shiny.Jobs;
using System;
using Xamarin.Forms;
using System.Diagnostics;
using Wesley.Client.AutoUpdater.Services;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Wesley.Client.Jobs;
using Wesley.Client.Services;

namespace Wesley.Client.ViewModels
{
    public class MainLayoutPageViewModel : ViewModelBaseCutom
    {

        public ICommand CreateJob { get; }
        public ICommand RunAsTask { get; }
        public ICommand ChangeRequiredInternetAccess { get; }
        [Reactive] public string AccessStatus { get; private set; }
        [Reactive] public string JobName { get; set; } = "DataSyncJob";

        [Reactive] public int SecondsToRun { get; set; } = 10;
        [Reactive] public string RequiredInternetAccess { get; set; } = InternetAccess.None.ToString();
        [Reactive] public bool BatteryNotLow { get; set; }
        [Reactive] public bool DeviceCharging { get; set; }

        /// <summary>
        /// 是否重复
        /// </summary>
        [Reactive] public bool Repeat { get; set; } = true;

        /// <summary>
        /// 是否前台运行
        /// </summary>
        [Reactive] public bool RunOnForeground { get; set; }


        private readonly IConnectivityHandler _connectivityHandler;
        private readonly IJobManager _jobManager;


        public MainLayoutPageViewModel(INavigationService navigationService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IConnectivityHandler connectivityHandler,
            IJobManager jobManager,
            IDialogService dialogService) : base(navigationService,
                productService,
                terminalService,
                userService,
                wareHousesService,
                accountingService,
                dialogService)
        {
            _connectivityHandler = connectivityHandler;
            _jobManager = jobManager;

            //订阅
            SubsribeAsync();

            var valObs = this.WhenAny(
                  x => x.JobName,
                  x => x.SecondsToRun,
                  (name, seconds) =>
                      !name.GetValue().IsEmpty() &&
                      seconds.GetValue() >= 10
            );

            this.CreateJob = ReactiveCommand.CreateFromTask(
             async _ =>
             {
                 var job = new JobInfo(typeof(JobWorker), this.JobName.Trim())
                 {
                     Repeat = true,
                     BatteryNotLow = this.BatteryNotLow,
                     DeviceCharging = this.DeviceCharging,
                     RunOnForeground = true,
                     RequiredInternetAccess = InternetAccess.Any
                 };
                 job.SetParameter("SecondsToRun", this.SecondsToRun);
                 await this._jobManager.Register(job);
             },
             valObs
         );

            this.ExceptionsSubscribe();
        }

        private void SubsribeAsync()
        {
            Task.Run(() =>
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                try
                {
                    if (!string.IsNullOrEmpty(Settings.UserMobile))
                    {
                        //开始订阅消息接收 / 通知接收
                        if (!string.IsNullOrEmpty(Settings.PusherMQEndpoint))
                        {
                            MQConsumer.Subsribe(Settings.UserMobile, token);
                        }
                    }
                }
                catch (Exception)
                {
                    tokenSource.Cancel();
                }

            }).ConfigureAwait(false);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();


            if (!Settings.IsInitData)
            {
                Observable.Timer(TimeSpan.FromSeconds(3))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async a =>
                {
                    try
                    {
                        Settings.IsInitData = true;
                        if (_connectivityHandler.IsConnected())
                        {
                            Device.BeginInvokeOnMainThread( () =>
                            {
                                ((ICommand)CreateJob)?.Execute(null);
                            });
                  
                            //var _globalService = App.Resolve<IGlobalService>();
                            //await _globalService?.GetAPPFeatures();
#if RELEASE
                            var _osp = App.Resolve<IOperatingSystemVersionProvider>();
                            var _updateService = App.Resolve<IUpdateService>();
                            //检查更新
                            if (!Settings.IsNextTimeUpdate)
                            {
                                var version = await _updateService?.GetCurrentVersionAsync();
                                if (version != null)
                                {
                                    _osp?.CheckUpdate(version);
                                }
                            }
#endif

                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                }, (ex) => { Crashes.TrackError(ex); });


                //Observable
                // .Interval(TimeSpan.FromSeconds(30))
                // .SubOnMainThread(async _ =>
                // {
                //     var tokenSource = new CancellationTokenSource();
                //     var token = tokenSource.Token;
                //     try
                //     {
                //         Debug.Print($"MainLayoutPageViewModel:30秒同步一次数据.....");
                //         if (_connectivityHandler.IsConnected())
                //         {
                //             var _globalService = App.Resolve<IGlobalService>();
                //             var _settingService = App.Resolve<ISettingService>();
                //             var taskList = new List<Task>
                //             {
                //                _globalService?.GetAPPFeatures(calToken: token),
                //                _globalService?.SynchronizationPermission(token),
                //                _settingService?.GetCompanySettingAsync(token)
                //             };
                //             await Task.WhenAll(taskList.ToArray()).ConfigureAwait(false);
                //         }
                //     }
                //     catch (Exception ex)
                //     {
                //         tokenSource.Cancel();
                //         Crashes.TrackError(ex);
                //     }
                // })
                // .DisposeWith(this.DeactivateWith);

            }
        }
    }
}

