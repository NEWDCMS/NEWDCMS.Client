using Android.Annotation;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using System;
using AndroidX.Annotations;
using Android.Runtime;

namespace DCMS.Client.Droid.KeepLive.config
{
    public class NotificationUtils : ContextWrapper
    {
        private NotificationManager manager;
        private readonly String id;
        private readonly String name;
        private readonly Context _context;
        private NotificationChannel channel;

        private NotificationUtils(Context context) : base(context)
        {
            this._context = context;
            id = context.PackageName;
            name = context.PackageName;
        }


        [RequiresApi(Value = 26)]
        public void CreateNotificationChannel()
        {
            if (channel == null)
            {
                channel = new NotificationChannel(id, name, NotificationImportance.High);
                channel.EnableVibration(false);
                channel.EnableLights(false);
                channel.EnableVibration(false);
                channel.SetVibrationPattern(new long[] { 0 });
                channel.SetSound(null, null);
                GetManager().CreateNotificationChannel(channel);
            }
        }

        private NotificationManager GetManager()
        {
            if (manager == null)
            {
                manager = (NotificationManager)GetSystemService(Context.NotificationService);
            }
            return manager;
        }

        [RequiresApi(Value = 26)]
        public Notification.Builder GetChannelNotification(String title, String content, int icon, Intent intent)
        {
            //PendingIntent.FLAG_UPDATE_CURRENT 这个类型才能传值
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(_context, 0, intent, PendingIntentFlags.UpdateCurrent);
            return new Notification.Builder(_context, id)
                    .SetContentTitle(title)
                    .SetContentText(content)
                    .SetSmallIcon(icon)
                    .SetAutoCancel(true)
                    .SetContentIntent(pendingIntent);
        }

        [RequiresApi(Value = 25)]
        public NotificationCompat.Builder GetNotification_25(String title, String content, int icon, Intent intent)
        {
            //PendingIntent.FLAG_UPDATE_CURRENT 这个类型才能传值
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(_context, 0, intent, PendingIntentFlags.UpdateCurrent);
            return new NotificationCompat.Builder(_context, id)
                    .SetContentTitle(title)
                    .SetContentText(content)
                    .SetSmallIcon(icon)
                    .SetAutoCancel(true)
                    .SetVibrate(new long[] { 0 })
                    .SetContentIntent(pendingIntent);
        }

        public static void SendNotification( Context? context, String? title,  String? content,  int? icon,  Intent? intent)
        {
            NotificationUtils notificationUtils = new NotificationUtils(context);
            Notification notification;
            //26
            if (Build.VERSION.SdkInt >=  BuildVersionCodes.O)
            {
                notificationUtils.CreateNotificationChannel();
                notification = notificationUtils.GetChannelNotification(title, content, icon ?? 0, intent).Build();
            }
            else
            {
                notification = notificationUtils.GetNotification_25(title, content, icon ?? 0, intent).Build();
            }
            notificationUtils.GetManager()
                .Notify(new Java.Util.Random()
                .NextInt(10000), notification);
        }

        public static Notification CreateNotification(Context? context,  String? title,  String? content,  int? icon,  Intent? intent)
        {
            NotificationUtils notificationUtils = new NotificationUtils(context);
            Notification notification;
            //26
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                notificationUtils.CreateNotificationChannel();
                notification = notificationUtils.GetChannelNotification(title, content, icon??0, intent).Build();
            }
            //else if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            //{
            //    // importance: 通知重要性
            //    // IMPORTANCE_HIGH：紧急级别（发出通知声音并显示为提示通知）
            //    // IMPORTANCE_DEFAULT：高级别（发出通知声音并且通知栏有通知）
            //    // IMPORTANCE_LOW：中等级别（没有通知声音但通知栏有通知）
            //    // IMPORTANCE_MIN：低级别（没有通知声音也不会出现在状态栏）
            //    var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Low);

            //    //是否绕过请勿打扰模式
            //    channel.CanBypassDnd();

            //    //设置可绕过请勿打扰模式
            //    channel.SetBypassDnd(true);

            //    //锁屏显示通知
            //    channel.LockscreenVisibility = NotificationVisibility.Secret;

            //    //桌面launcher的消息角标
            //    channel.CanShowBadge();

            //    notificationManager.CreateNotificationChannel(channel);
            //}
            else
            {
                notification = notificationUtils.GetNotification_25(title, content, icon ?? 0, intent).Build();
            }
            return notification;
        }
    }

}