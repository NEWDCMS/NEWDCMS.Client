using Acr.UserDialogs;
using DCMS.Client.Models;
using DCMS.Client.Models.Census;
using DCMS.Client.Services;
using Prism.Navigation;
using ReactiveUI;
using DCMS.Client.Models.Terminals;
using System;
using Akavache;
using System.Reactive;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace DCMS.Client.ViewModels
{

    public class SecurityPageViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        public IReactiveCommand UpdatePasswordCommand => this.Navigate("ResetPasswordPage");
        public ReactiveCommand<string, Unit> SignOutCommand { get; }

        private readonly ILiteDbService<VisitStore> _conn1;
        private readonly ILiteDbService<TrackingModel> _conn2;
        private readonly ILiteDbService<NotificationEvent> _conn3;
        private readonly ILiteDbService<PushEvent> _conn4;
        private readonly ILiteDbService<MessageInfo> _conn5;
        private readonly ILiteDbService<CacheBillData> _conn6;
        private readonly ILiteDbService<CachePaymentMethod> _conn7;
        private readonly ILiteDbService<TerminalModel> _conn8;


        public SecurityPageViewModel(INavigationService navigationService,
            IAuthenticationService authenticationService,
            IDialogService dialogService,
            ILiteDbService<VisitStore> conn1,
            ILiteDbService<TrackingModel> conn2,
            ILiteDbService<NotificationEvent> conn3,
            ILiteDbService<PushEvent> conn4,
            ILiteDbService<MessageInfo> conn5,
            ILiteDbService<CacheBillData> conn6,
            ILiteDbService<CachePaymentMethod> conn7,
            ILiteDbService<TerminalModel> conn8) : base(navigationService, dialogService)
        {
            _authenticationService = authenticationService;

            _conn1 = conn1;
            _conn2 = conn2;
            _conn3 = conn3;
            _conn4 = conn4;
            _conn5 = conn5;
            _conn6 = conn6;
            _conn7 = conn7;
            _conn8 = conn8;

            Title = "账号设置";

            this.SignOutCommand = ReactiveCommand.CreateFromTask<string>(async e =>
            {
                var ok = await UserDialogs.Instance.ConfirmAsync("确定要注销账户码？", "", "确定", "取消");
                if (ok)
                {
                    using (UserDialogs.Instance.Loading("注销中..."))
                    {
                        try
                        {
                            Settings.AccessToken = "";
                            Settings.IsAuthenticated = false;
                            Settings.IsInitData = false;

                            Parallel.Invoke(async () =>
                            {
                                await _conn1.DeleteAllAsync();
                            },
                            async () =>
                            {
                                await _conn2.DeleteAllAsync();
                            },
                            async () =>
                            {
                                await _conn3.DeleteAllAsync();
                            },
                            async () =>
                            {
                                await _conn4.DeleteAllAsync();
                            },
                            async () =>
                            {
                                await _conn5.DeleteAllAsync();
                            },
                            async () =>
                            {
                                await _conn6.DeleteAllAsync();
                            },
                            async () =>
                            {
                                await _conn7.DeleteAllAsync();
                            },
                            async () =>
                            {
                                await _conn8.DeleteAllAsync();
                            },
                            () =>
                            {
                                try
                                {
                                    BlobCache.LocalMachine.InvalidateAll();
                                }
                                catch (Exception) { }

                            }, async () =>
                            {
                                await _authenticationService.LogOutAsync();
                            });
                        }
                        catch (Exception ex)
                        {
                            _dialogService.LongAlert(ex.Message);
                        }
                        finally 
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await this.NavigateAsync("../LoginPage");
                            });
                        }
                    };
                }
            });
        }
    }
}
