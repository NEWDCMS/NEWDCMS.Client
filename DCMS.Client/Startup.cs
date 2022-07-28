using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prism.DryIoc;
using Prism.Ioc;

using System;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;


namespace DCMS.Client
{
    /// <summary>
    /// 启动项 ShinyStartup
    /// </summary>
    public class Startup 
    {
        /// <summary>
        /// 配置日志级别
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="platform"></param>
        public override void ConfigureLogging(ILoggingBuilder builder, IPlatform platform)
        {
            builder.AddConsole(opts =>
                opts.LogToStandardErrorThreshold = LogLevel.Debug
            );

            //Sqlite存储日志
            //builder.AddSqliteLogging(LogLevel.Warning);


            //启用AppCente
            //"android=03804b2b-3759-4bab-9286-ae78dcd60abe;"
            //builder.AddAppCenter(AppCenterKey, LogLevel.Warning);
        }

        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="platform"></param>
        public override void ConfigureServices(IServiceCollection services, IPlatform platform)
        {
            //使用Sqlite 存储
            services.UseSqliteStore();

            //应用通知
            //services.AddSingleton<AppNotifications>();
            services.AddSingleton<IAppInfo, AppInfoImplementation>();
       

            //数据存储
            services.AddSingleton<LocalDatabase>();
            //services.AddSingleton<CoreDelegateServices>();
            //services.AddSingleton<IAppSettings, AppSettings>();

            //全局异常处理
            //services.AddSingleton<GlobalExceptionHandler>();

            //启动工作日志任务
            //services.AddSingleton<JobLoggerTask>();

            /*
            //使用通知:这里创建了 Notice 和  Message 2种 Channel
            services.UseNotifications<NotificationDelegate>(new AndroidOptions
            {
                ShowWhen = true,
                OnGoing = true,
                LaunchActivityFlags = AndroidActivityFlags.NewTask | AndroidActivityFlags.ResetTaskIfNeeded
            },
            new[]
            {
                Channel.Create(
                    "Notice",
                    ChannelAction.Create("查看", ChannelActionType.Destructive)
                ),
                Channel.Create(
                    "Message",
                    ChannelAction.Create("查看", ChannelActionType.Destructive)
                )
            });
            */
        }

        /// <summary>
        /// 使用Prism注册并初始化容器，需要确保Shiny和Prism使用相同的容器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            ContainerLocator.SetContainerExtension(() => new DryIocContainerExtension());
            var container = ContainerLocator.Container.GetContainer();
            DryIocAdapter.Populate(container, services);
            return container.GetServiceProvider();
        }
    }
}
