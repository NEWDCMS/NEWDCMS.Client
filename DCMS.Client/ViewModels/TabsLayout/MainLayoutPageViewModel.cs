using DCMS.Client.AutoUpdater.Services;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using System.Reactive.Disposables;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DCMS.Client.ViewModels
{
    public class MainLayoutPageViewModel : ViewModelBaseCutom
    {
        public static TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(30);
        public MainLayoutPageViewModel(INavigationService navigationService,
            IProductService productService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,
            IDialogService dialogService) : base(navigationService, productService, terminalService,
                userService, wareHousesService, accountingService, dialogService)
        { }

        public override void OnAppearing()
        {
            base.OnAppearing();

            if (!Settings.IsInitData)
            {
                //延迟3秒
                Observable.Timer(TimeSpan.FromSeconds(3))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(a =>
                {
                    try
                    {
                        //启动更新检查
                        Task.Run(async () =>
                        {
                            //检查更新
                            if (!Settings.IsNextTimeUpdate)
                            {
                                var version = await App.Resolve<IUpdateService>()?.GetCurrentVersionAsync();
                                if (version != null)
                                {
                                    App.Resolve<IOperatingSystemVersionProvider>()?.CheckUpdate(version);
                                }
                            }
                        }).ConfigureAwait(false);

                        Settings.IsInitData = true;
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                }, (ex) => { Crashes.TrackError(ex); })
                .DisposeWith(DeactivateWith);
            }
        }
    }
}

