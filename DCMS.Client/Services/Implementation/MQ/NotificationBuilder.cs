//using Shiny.Notifications;

namespace Wesley.Client.Services
{
    /*
    public class NotificationBuilder
    {
        public string SelectedSoundType { get; set; } = "None";
        public int Identifier { get; set; }
        public int BadgeCount { get; set; }
        public string Payload { get; set; }
        public string AndroidChannel { get; set; }
        public bool UseAndroidVibrate { get; set; } = true;
        public bool UseAndroidBigTextStyle { get; set; } = true;
        public bool UseAndroidHighPriority { get; set; } = true;

        /// <summary>
        /// 构建并发送通知
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="notificationId"></param>
        /// <param name="title">标题</param>
        /// <param name="message">消息体</param>
        /// <param name="scheduleDate">计划通知日期-留空以立即通知</param>
        /// <returns></returns>
        public async Task BuildAndSend(string channel, int notificationId, string title, string message, DateTime? scheduleDate = null)
        {
            try
            {
                var _notificationManager = ShinyHost.Resolve<INotificationManager>();

                if (_notificationManager == null)
                    this.Reset();

                //await _notificationManager.RemoveChannel("Notice");
                //await _notificationManager.RemoveChannel("Message");

                var notification = new Notification
                {
                    Title = title,
                    Message = message,
                    BadgeCount = this.BadgeCount,
                    ScheduleDate = scheduleDate,
                    //Notice 和  Message 
                    Channel = channel
                };


                notification.Id = notificationId;

                //承载消息体
                if (!this.Payload.IsEmpty())
                {
                    notification.Payload = new Dictionary<string, string> {
                    { nameof(this.Payload), this.Payload }
                };
                }
                notification.Android.UseBigTextStyle = this.UseAndroidBigTextStyle;

                //发送通知
                await _notificationManager.Send(notification);

                //提示语音
                var payload = this.Payload;
                var messageInfo = JsonConvert.DeserializeObject<MessageInfo>(payload);
                if (messageInfo != null)
                {
                    var _conn = ShinyHost.Resolve<LocalDatabase>();
                    //存储本地 MessageInfo
                    if (messageInfo != null)
                        await _conn.InsertAsync(messageInfo);

                    //播放提示语音
                    Play(messageInfo.MType);
                }
            }
            catch (Exception)
            {
            }

            //重置
            this.Reset();
        }


        private void Play(MTypeEnum soundType)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var path = "Wesley.Client.Resources.raw";
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

                //Dispose
                audioStream.Dispose();
            }
        }

        private void Reset()
        {
            this.Payload = string.Empty;
        }
    }
    */
}