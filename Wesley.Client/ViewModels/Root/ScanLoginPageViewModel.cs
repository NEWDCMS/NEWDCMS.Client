using Newtonsoft.Json;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using Acr.UserDialogs;


namespace Wesley.Client.ViewModels
{
    public class ScanLoginPageViewModel : ViewModelBase
    {
        public ScanData ScanData { get; set; } = new ScanData();
        [Reactive] public string AccountName { get; set; } = Settings.UserMobile;

        public IReactiveCommand LoginCommand { get; }

        private readonly IAuthenticationService _authenticationService;

        public ScanLoginPageViewModel(INavigationService navigationService,
            IAuthenticationService authenticationService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "扫码登录";

            _authenticationService = authenticationService;

            this.LoginCommand = ReactiveCommand.Create(async () =>
            {
                try
                {
                    //ScanData.UUID = "dd2d977b-1341-4957-9e7e-2659b2b28b93";

                    if (ScanData == null || string.IsNullOrEmpty(ScanData.UUID))
                    {
                        _dialogService.ShortAlert("无效操作！");
                        return;
                    }

                    using (UserDialogs.Instance.Loading("授权中..."))
                    {
                        var result = await _authenticationService.QRLoginAsync(ScanData.UUID);
                        if (result)
                        {
                            await this.NavigateAsync("../MainLayoutPage");
                        }
                        else
                        {
                            _dialogService.ShortAlert("会话已过期，请刷新重新尝试！");
                            await _navigationService.GoBackAsync();
                            return;
                        }
                    };
                }
                catch (Exception)
                {
                }
            });
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                //选择客户
                if (parameters.ContainsKey("ScanData"))
                {
                    parameters.TryGetValue("ScanData", out ScanData scanData);
                    if (scanData != null)
                        this.ScanData = scanData;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
