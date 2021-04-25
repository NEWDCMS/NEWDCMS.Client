using Wesley.Client.Services;
using Prism.Navigation;
using ReactiveUI;


namespace Wesley.Client.ViewModels
{

    public class SystemSettingPageViewModel : ViewModelBase
    {
        private readonly IPermissionsService _permissionsService;

        public IReactiveCommand BackgroundOperationCommand { get; }
        public IReactiveCommand BatteryOptimizationCommand { get; }

        public SystemSettingPageViewModel(INavigationService navigationService,
            IPermissionsService permissionsService,
              IDialogService dialogService) : base(navigationService, dialogService)
        {
            _permissionsService = permissionsService;

            Title = "权限设置";

            //后台运行设置
            this.BackgroundOperationCommand = ReactiveCommand.Create(() =>
            {
                _permissionsService.BackgroundOperationSetting();
            });

            //电池优化设置
            this.BatteryOptimizationCommand = ReactiveCommand.Create(() =>
            {
                _permissionsService.BatteryOptimizationSetting();
            });

            this.ExceptionsSubscribe();
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
