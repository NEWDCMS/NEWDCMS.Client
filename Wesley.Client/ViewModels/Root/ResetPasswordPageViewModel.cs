using Acr.UserDialogs;
using Wesley.Client.Services;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;

namespace Wesley.Client.ViewModels
{

    public class ResetPasswordPageViewModel : ViewModelBase
    {

        #region 属性

        [Reactive]
        public string OldPassword
        { get; internal set; }

        [Reactive]
        public string NewPassword
        { get; internal set; }

        [Reactive]
        public string ComfirmPassword
        { get; internal set; }


        #endregion


        #region Commands


        /// <summary>
        /// 保存
        /// </summary>
        private DelegateCommand<object> _saveCommand;
        public new DelegateCommand<object> SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new DelegateCommand<object>(async (e) =>
                    {

                        if (string.IsNullOrEmpty(OldPassword))
                        {
                            await _dialogService.ShowAlertAsync("请输入旧密码...", "提示", "取消");
                            return;
                        }

                        if (string.IsNullOrEmpty(NewPassword))
                        {
                            await _dialogService.ShowAlertAsync("请输入新密码...", "提示", "取消");
                            return;
                        }

                        if (string.IsNullOrEmpty(ComfirmPassword))
                        {
                            await _dialogService.ShowAlertAsync("请输入确认密码...", "提示", "取消");
                            return;
                        }

                        if (!NewPassword.Equals(ComfirmPassword))
                        {
                            await _dialogService.ShowAlertAsync("新密码和确认密码不相符", "提示", "取消");
                            return;
                        }

                        using (UserDialogs.Instance.Loading("Loading..."))
                        {
                            this.Alert("修改成功！");
                        }

                        await _navigationService.GoBackAsync();
                    });
                }
                return _saveCommand;
            }
        }

        #endregion


        public ResetPasswordPageViewModel(INavigationService navigationService,
              IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = "重置密码";
        }

    }
}
