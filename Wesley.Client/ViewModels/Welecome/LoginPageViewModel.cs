using Acr.UserDialogs;
using Wesley.Client.AutoUpdater.Services;
using Wesley.Client.CustomViews;
using Wesley.Client.Pages;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using System;
using System.Net;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IOperatingSystemVersionProvider _operatingSystemVersionProvider;
        [Reactive] public string Password { get; set; }
        [Reactive] public bool IsEnabled { get; set; }
        [Reactive] public bool Agreemented { get; set; }
        public IReactiveCommand PerformLogin { get; }
        public IReactiveCommand AgreementedCmd { get; }

        private readonly ValidationHelper NameRule;
        private readonly ValidationHelper PasswordRule;
        private readonly ValidationHelper AgreementedRule;

        public LoginPageViewModel(INavigationService navigationService,
            IAuthenticationService authenticationService,
            IOperatingSystemVersionProvider operatingSystemVersionProvider,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "登录";

            _authenticationService = authenticationService;
            _operatingSystemVersionProvider = operatingSystemVersionProvider;

            this.WhenAnyValue(
                x => x.UserName,
                x => x.Password,
                (u, p) => !string.IsNullOrEmpty(u) && !string.IsNullOrEmpty(p))
                .Select(x => x)
                .Subscribe(x => this.IsEnabled = x).DisposeWith(DestroyWith);

#if DEBUG
            UserName = "13883655199";
            Password = "dcms.1";
#endif
            var name = this
                .WhenAnyValue(x => x.UserName)
                .Select(x => x == null || CommonHelper.IsPhoneNo(x, true));

            var password = this
               .WhenAnyValue(x => x.Password)
               .Select(x => x == null || x?.Length >= 6);

            var agreemented = this
                .WhenAnyValue(x => x.Agreemented)
                .Select(x => x);

            this.AgreementedRule = this.ValidationRule(vm => vm.Agreemented,
                agreemented, "是否同意并接受云销管理服务协议条款?");

            this.NameRule = this.ValidationRule(vm => vm.UserName,
                name, "用户名不能为空，且必须为手机号.");

            this.PasswordRule = this.ValidationRule(vm => vm.Password,
                password, "必须指定一个长度超过6个符号的有效密码.");

            //检查版本
            this.Load = ReactiveCommand.Create(() =>
            {
                try
                {
                    CurrentAppVersion = _operatingSystemVersionProvider.GetVersion();
                }
                catch (Exception)
                { }
            });

            //登录
            this.PerformLogin = ReactiveCommand.CreateFromTask<string>(async arg =>
            {
                if (!AgreementedRule.IsValid) { _dialogService.ShortAlert(AgreementedRule.Message[0]); return; }
                if (!NameRule.IsValid) { _dialogService.ShortAlert(NameRule.Message[0]); return; }
                if (!PasswordRule.IsValid) { _dialogService.ShortAlert(PasswordRule.Message[0]); return; }

                bool isAuthenticated = false;
                using (var dig = UserDialogs.Instance.Loading("登录中..."))
                {
                    try
                    {
                        Settings.IsInitData = false;
                        var rcode = await _authenticationService.LoginAsync(UserName, Password);
                        if (rcode == 1)
                        {
                            isAuthenticated = true;
                        }
                    }
                    catch (Exception ex) when (ex is WebException || ex is HttpRequestException)
                    {
                        dig.Hide();
                        await ShowAlert(false, "认证失败,网络异常！");
                        return;
                    }
                    catch (Exception)
                    {
                        dig.Hide();
                        await ShowAlert(false, "认证失败,网络异常！");
                        return;
                    }
                    //认证成功
                    if (isAuthenticated)
                    {
                        await this.NavigateAsync($"{nameof(MainLayoutPage)}");
                    }
                    else
                    {
                        dig.Hide();
                        await ShowAlert(false, "认证失败,无效账户或错误凭证！");
                        return;
                    }
                }
            });

            //免责
            this.AgreementedCmd = ReactiveCommand.Create(async () =>
            {
                await CrossDiaglogKit.Current.PopViewAsync("免责申明", GlobalSettings.AgreementText);
            });

            this.PerformLogin.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.BindBusyCommand(Load);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
