using Microsoft.Extensions.DependencyInjection;
using Shiny;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;

namespace Wesley.Client
{
    /// <summary>
    /// 启动项 ShinyStartup
    /// </summary>
    public class Startup : ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            //应用通知
            services.AddSingleton<AppNotifications>();
            services.AddSingleton<IAppInfo, AppInfoImplementation>();
            services.AddSingleton<LocalDatabase>();
            services.AddSingleton<CoreDelegateServices>();

            //注册单例配置
            services.AddSingleton<IAppSettings, AppSettings>();

            //使用通知
            services.UseNotifications();

            //services.UseNotifications<NotificationDelegate>(
            //    true,
            //    new NotificationCategory(
            //        "Test",
            //        new NotificationAction("Reply", "Reply", NotificationActionType.TextReply),
            //        new NotificationAction("Yes", "Yes", NotificationActionType.None),
            //        new NotificationAction("No", "No", NotificationActionType.Destructive)
            //    ),
            //    new NotificationCategory(
            //        "ChatName",
            //        new NotificationAction("Answer", "Answer", NotificationActionType.TextReply)
            //    ),
            //    new NotificationCategory(
            //        "ChatAnswer",
            //        new NotificationAction("yes", "Yes", NotificationActionType.None),
            //        new NotificationAction("no", "No", NotificationActionType.Destructive)
            //    )
            //);

            //////GPS 服务
            //services.UseGeofencing<GeofenceDelegate>();
            //services.UseGps<GpsDelegate>();

            //////GPS 同步
            //services.UseGeofencingSync<LocationSyncDelegates>();
            //services.UseGpsSync<LocationSyncDelegates>();

            ////轨迹跟踪器

        }
    }
}
