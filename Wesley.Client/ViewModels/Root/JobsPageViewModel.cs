using Acr.UserDialogs;
using Wesley.Client.AutoUpdater.Services;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class JobsPageViewModel : ViewModelBase
    {
        private readonly IOperatingSystemVersionProvider _operatingSystemVersionProvider;
        private readonly IUpdateService _updateService;
        private readonly ICacheManager _cacheManager;
        [Reactive] public string CurrentVersion { get; internal set; }
        [Reactive] public string LatestVersion { get; internal set; }

        public JobsPageViewModel(INavigationService navigationService,
            IOperatingSystemVersionProvider operatingSystemVersionProvider,
            IUpdateService updateService,
            ICacheManager cacheManager,
              IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            _operatingSystemVersionProvider = operatingSystemVersionProvider;
            _updateService = updateService;
            _cacheManager = cacheManager;

            Title = "关于我们";

            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                var version = await _updateService.GetCurrentVersionAsync();
                if (version != null && version != null)
                {
                    var curVersion = _operatingSystemVersionProvider.GetVersion();
                    CurrentVersion = curVersion;
                    LatestVersion = version.Version;
                }
            }));

            BindBusyCommand(Load);

        }

        private DelegateCommand<string> _invokeCommand;
        public DelegateCommand<string> InvokeCommand
        {
            get
            {
                if (_invokeCommand == null)
                {
                    _invokeCommand = new DelegateCommand<string>(async (r) =>
                    {
                        try
                        {
                            switch (r)
                            {
                                case "AgreementPage":
                                    {
                                        //协议
                                        await this.NavigateAsync(r.ToString(), null);
                                        break;
                                    }
                                case "UpdatePage":
                                    {
                                        //更新
                                        await this.NavigateAsync(r.ToString(), null);
                                        break;
                                    }
                                case "ClearCache":
                                    {

                                        var ok = await UserDialogs.Instance.ConfirmAsync("你确定要删除缓存，这将会导致数据丢失？", "清理缓存", "确定", "取消");
                                        if (ok)
                                        {
                                            //清理缓存
                                            using (UserDialogs.Instance.Loading("清理中..."))
                                            {
                                                try
                                                {

                                                    _cacheManager?.ClearCache(true, true);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Crashes.TrackError(ex);
                                                }
                                            }
                                        }

                                        break;
                                    }
                                case "TrackLocationPage":
                                    {
                                        //定位
                                        await this.NavigateAsync("TrackLocationPage", null);
                                        break;
                                    }
                                case "IssuesPost":
                                    {
                                        await this.NavigateAsync("ConversationsPage", null);
                                        break;
                                    }
                                case "QAHelper":
                                    {
                                        await this.NavigateAsync("ConversationsPage", null);
                                        break;
                                    }
                                case "CopyrightPage":
                                    {
                                        await this.NavigateAsync("CopyrightPage", null);
                                        break;
                                    }
                                case "FeedbackPage":
                                    {
                                        await this.NavigateAsync("FeedbackPage", null);
                                        break;
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            //Debug(ex.Message);
                            Crashes.TrackError(ex);
                        }
                    });
                }
                return _invokeCommand;
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
