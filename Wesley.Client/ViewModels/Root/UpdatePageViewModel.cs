using Wesley.Client.AutoUpdater.Services;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{

    public class UpdatePageViewModel : ViewModelBase
    {

        #region 属性

        private readonly IOperatingSystemVersionProvider _operatingSystemVersionProvider;
        private readonly IUpdateService _updateService;

        [Reactive] public UpdateInfo CurrentUpdateInfo { get; set; } = new UpdateInfo();
        [Reactive] public string CurrentVersion { get; set; } = "0.0.0.0";
        [Reactive] public string LatestVersion { get; set; } = "0.0.0.0";
        [Reactive] public string DownloadInfoText { get; set; } = "已经是最新版了";
        [Reactive] public bool IsUpdating { get; set; }
        [Reactive] public bool IsEnabled { get; set; }
        public IReactiveCommand UpdateCommand { get; }

        #endregion


        public UpdatePageViewModel(INavigationService navigationService,
            IOperatingSystemVersionProvider operatingSystemVersionProvider,
            IUpdateService updateService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {

            _operatingSystemVersionProvider = operatingSystemVersionProvider;
            _updateService = updateService;

            Title = "版本更新";

            this.WhenAnyValue(x => x.IsUpdating)
                .Select(x => x ? "下载更新" : "已经是最新版了")
                .Subscribe(x => 
                {
                    this.DownloadInfoText = x;
                }).DisposeWith(DeactivateWith);

            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run( async () =>
            {
                try
                {
                    var curVersion = _operatingSystemVersionProvider.GetVersion();

                    if (!string.IsNullOrEmpty(curVersion))
                        this.CurrentVersion = curVersion;

                    var version = await _updateService.GetCurrentVersionAsync();

                    if (version != null)
                    {
                        var checkNew = _operatingSystemVersionProvider.CheckNewVersion(version);
                        this.CurrentUpdateInfo = version;
                        this.LatestVersion = version.Version;
                        this.IsEnabled = version.Enable;
                        this.IsUpdating = checkNew;
                    }
                }
                catch (Exception) { }
            }));

            //检查更新
            this.UpdateCommand = ReactiveCommand.Create(() =>
            {
                try
                {
                    if (CurrentUpdateInfo == null)
                    {
                        _dialogService.ShortAlert("请确保MainActivity已经初始化配置AutoUpdate.");
                        return;
                    }
                    _operatingSystemVersionProvider.Check(CurrentUpdateInfo);
                }
                catch (Exception ex)
                {
                    IsActive = true;
                    Crashes.TrackError(ex);
                }
            });

            this.BindBusyCommand(Load);

        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }

    }
}
