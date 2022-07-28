//using Shiny.Notifications;

namespace Wesley.Client
{
    /*
    public class NotificationDelegate : INotificationDelegate
    {
        private readonly LocalDatabase _conn;
        private readonly Shiny.IMessageBus _messageBus;
        private readonly INotificationManager _notifications;


        /// <summary>
        /// 通知发送，参见 Wesley.Client.Services.NotificationBuilder
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="notifications"></param>
        /// <param name="messageBus"></param>
        public NotificationDelegate(LocalDatabase conn,
                                    INotificationManager notifications,
                                    Shiny.IMessageBus messageBus)
        {
            this._conn = conn;
            this._notifications = notifications;
            this._messageBus = messageBus;
        }

        /// <summary>
        /// 点击通知响应
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task OnEntry(NotificationResponse response)
        {
            var @event = new NotificationEvent
            {
                NotificationId = response.Notification.Id,
                NotificationTitle = response.Notification.Title ?? response.Notification.Message,
                Action = response.ActionIdentifier,
                ReplyText = response.Text,
                Timestamp = DateTime.Now
            };

            this._messageBus.Publish(@event);

            await _notifications?.Cancel(response.Notification.Id);
            await this._conn.InsertAsync(@event);

            await this.Handle(response);
        }

        private async Task Handle(NotificationResponse response)
        {
            try
            {
                //var cat = response.Notification.Channel;
                var payload = response.Notification.Payload["Payload"];
                var messageInfo = JsonConvert.DeserializeObject<MessageInfo>(payload);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var nav = App.Resolve<INavigationService>();
                    if (new[] { 0, 1, 2, 3 }.Contains(messageInfo.MTypeId))
                    {
                        await nav.NavigateAsync("MainLayoutPage/MessagesPage");

                    }
                    else if (new[] { 4, 5, 6, 7, 8, 9, 10, 11, 12 }.Contains(messageInfo.MTypeId))
                    {
                        await nav.NavigateAsync("MainLayoutPage/NotificationsPage");
                    }
                });
                await Task.FromResult(true);
            }
            catch (Exception)
            {
            }
        }
    }
    */
}
