using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Shiny;
using Shiny.Notifications;

namespace Wesley.Client
{
    /// <summary>
    /// 表示委托核心服务
    /// </summary>
    public class CoreDelegateServices
    {
        public LocalDatabase Connection { get; }
        public AppNotifications Notifications { get; }
        public IAppSettings AppSettings { get; }

        public CoreDelegateServices(AppNotifications notifications,
                                    IAppSettings appSettings)
        {
            this.Notifications = notifications;
            this.AppSettings = appSettings;
        }

        public async Task SendNotification(string title, string message, Expression<Func<IAppSettings, bool>>? expression = null)
        {
            await this.Notifications.Send(
                this.GetType(),
                true,
                title,
                message
            );
        }


    }
}
