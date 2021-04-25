using Wesley.Client.Models;
using Newtonsoft.Json;
using Plugin.SimpleAudioPlayer;
using ReactiveUI;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

using Shiny.Notifications;

namespace Wesley.Client
{
    public class NotificationDelegate : INotificationDelegate
    {
        private readonly LocalDatabase _conn;
        readonly Shiny.IMessageBus _messageBus;
        private readonly INotificationManager _notifications;

        public NotificationDelegate(LocalDatabase conn,
           INotificationManager notifications,
             Shiny.IMessageBus messageBus)
        {
            this._conn = conn;
            this._notifications = notifications;
            this._messageBus = messageBus;
        }


        //public async Task OnEntry(NotificationResponse response)
        //{
        //    await this.Store(new NotificationEvent
        //    {
        //        NotificationId = response.Notification.Id,
        //        NotificationTitle = response.Notification.Title ?? response.Notification.Message,
        //        Action = response.ActionIdentifier,
        //        ReplyText = response.Text,
        //        IsEntry = true,
        //        Timestamp = DateTime.Now
        //    });
        //    await this.DoChat(response);
        //}


        public async Task OnEntry(NotificationResponse response)
        {
            try
            {
                var payload = response.Notification.Payload["Payload"];
                var messageInfo = JsonConvert.DeserializeObject<MessageInfo>(payload);
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (new[] { 0, 1, 2, 3 }.Contains(messageInfo.MTypeId))
                    {
                        MessageBus.Current.SendMessage(messageInfo, Constants.MESSAGE_PUSHERRECEIVED_KEY);
                    }
                    else if (new[] { 4, 5, 6, 7, 8, 9, 10, 11, 12 }.Contains(messageInfo.MTypeId))
                    {
                        MessageBus.Current.SendMessage(messageInfo, Constants.NOTIFI_PUSHERRECEIVED_KEY);
                    }
                });
                await Task.FromResult(true);
            }
            catch (Exception)
            { }
        }

        //public Task OnReceived(Notification notification) => this.Store(new NotificationEvent
        //{
        //    NotificationId = notification.Id,
        //    NotificationTitle = notification.Title ?? notification.Message,
        //    IsEntry = false,
        //    Timestamp = DateTime.Now
        //});

        public async Task OnReceived(Notification notification)
        {
            try
            {
                var payload = notification.Payload["Payload"];
                var messageInfo = JsonConvert.DeserializeObject<MessageInfo>(payload);
                //存储
                await this.Store(messageInfo);
                //播放提示语音
                Play(messageInfo.MType);
            }
            catch (Exception)
            { }
        }

        private void Play(MTypeEnum soundType)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var path = "Wesley.Client.Resources.raw";
            // 
            Stream audioStream = soundType switch
            {
                MTypeEnum.All => assembly.GetManifestResourceStream($"{path}.S6238.mp3"),
                MTypeEnum.Message => assembly.GetManifestResourceStream($"{path}.S4130.mp3"),
                MTypeEnum.Receipt => assembly.GetManifestResourceStream($"{path}.S3247.mp3"),
                MTypeEnum.Hold => assembly.GetManifestResourceStream($"{path}.S7020.mp3"),
                MTypeEnum.Push => assembly.GetManifestResourceStream($"{path}.S6241.mp3"),
                MTypeEnum.Audited => assembly.GetManifestResourceStream($"{path}.S7073.mp3"),
                MTypeEnum.Scheduled => assembly.GetManifestResourceStream($"{path}.S8961.mp3"),
                MTypeEnum.InventoryCompleted => assembly.GetManifestResourceStream($"{path}.S7919.mp3"),
                MTypeEnum.TransferCompleted => assembly.GetManifestResourceStream($"{path}.S4776.mp3"),
                MTypeEnum.InventoryWarning => assembly.GetManifestResourceStream($"{path}.S5130.mp3"),
                MTypeEnum.CheckException => assembly.GetManifestResourceStream($"{path}.S4655.mp3"),
                MTypeEnum.LostWarning => assembly.GetManifestResourceStream($"{path}.S78.mp3"),
                MTypeEnum.LedgerWarning => assembly.GetManifestResourceStream($"{path}.S1553.mp3"),
                MTypeEnum.Paymented => assembly.GetManifestResourceStream($"{path}.S2873.mp3"),
                _ => assembly.GetManifestResourceStream($"{path}.S6238.mp3"),
            };
            if (audioStream != null)
            {
                var audio = CrossSimpleAudioPlayer.Current;
                audio.Load(audioStream);
                audio.Play();
            }
        }

        private async Task Store(MessageInfo msg)
        {
            //存储本地 MessageInfo
            await this._conn.InsertAsync(msg);
            Device.BeginInvokeOnMainThread(() =>
            {
                var @event = new NotificationEvent();
                MessageBus.Current.SendMessage(@event, Constants.UPDATEUI_KEY);
            });

            //this._messageBus.Publish(msg);
        }


        //async Task Store(NotificationEvent @event)
        //{
        //    //await this.conn.InsertAsync(@event);
        //    this._messageBus.Publish(@event);
        //}
    }
}
