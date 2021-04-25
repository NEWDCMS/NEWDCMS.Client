using Acr.UserDialogs;
using Wesley.Client.Services;

using Prism.Navigation;
using ReactiveUI;

using System;
using System.Reactive;
using System.Reactive.Linq;
namespace Wesley.Client.ViewModels
{

    public class SecurityPageViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        public IReactiveCommand UpdatePasswordCommand => this.Navigate("ResetPasswordPage");
        public ReactiveCommand<string, Unit> SignOutCommand { get; }


        public SecurityPageViewModel(INavigationService navigationService,
            IAuthenticationService authenticationService,


            IDialogService dialogService) : base(navigationService, dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _authenticationService = authenticationService;

            Title = "账号设置";

            this.SignOutCommand = ReactiveCommand.CreateFromTask<string>(async e =>
            {
                var ok = await UserDialogs.Instance.ConfirmAsync("确定要注销账户码？", "", "确定", "取消");
                if (ok)
                {
                    using (UserDialogs.Instance.Loading("注销中..."))
                    {
                        await _authenticationService.LogOutAsync();
                        await this.NavigateAsync("../LoginPage");
                    };
                }
            });

            this.UpdatePasswordCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.SignOutCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.ExceptionsSubscribe();
        }
    }
}
