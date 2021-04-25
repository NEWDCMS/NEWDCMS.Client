using Wesley.Client.AutoUpdater.Services;
using Wesley.Client.BaiduMaps;
using Wesley.Client.Droid.Services;
using Wesley.Client.Services;
using Prism;
using Prism.Ioc;

namespace Wesley.Client.Droid
{
    /// <summary>
    /// 注册用于任何特定于平台的实现 Prism.Ioc.IPlatformInitializer
    /// </summary>
    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {
            container.RegisterSingleton<ICacheHandler, AkavacheCacheHandler>();
            container.RegisterSingleton<IConnectivityHandler, ShinyConnectivityHandler>();
            container.RegisterSingleton<ICacheManager, CacheManager>();
            container.RegisterSingleton<IPermissionsService, PermissionsService>();
            container.RegisterSingleton<IDialogService, DialogService>();
            container.RegisterSingleton<IBaiduLocationService, BaiduLocationServiceImpl>();
            container.RegisterSingleton<IBaiduNavigationService, BaiduNavigationService>();
            container.RegisterSingleton<ICalculateUtils, CalculateUtilsImpl>();
            container.RegisterSingleton<IMapManager, MapManagerImpl>();
            container.RegisterSingleton<IOfflineMap, OfflineMapImpl>();
            container.RegisterSingleton<IProjection, ProjectionImpl>();
            //container.RegisterSingleton<GlobalExceptionHandler>();
            // Media
            container.RegisterSingleton<IMediaPickerService, MediaPickerService>();
            //Microphone
            container.RegisterSingleton<IMicrophoneService, MicrophoneService>();
            //perating
            container.RegisterSingleton<IOperatingSystemVersionProvider, OperatingSystemVersionProvider>();
            //Keyboard
            container.RegisterSingleton<ISoftwareKeyboardService, SoftwareKeyboardService>();
        }
    }
}