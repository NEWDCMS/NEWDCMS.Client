using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.Core.App;


namespace Wesley.Client.Droid.KeepLive.utils
{
    public class NotificationUtil
    {
        /// <summary>
        /// 通知ID
        /// </summary>
        public static readonly int NOTIFICATION_ID = int.MaxValue;
        private readonly Activity ctx = null;

        /// <summary>
        /// 通知管理器
        /// </summary>
        private readonly NotificationManager notificationManager = null;
        /// <summary>
        /// 通知
        /// </summary>
        private readonly Notification notification = null;

        public Notification GetNotification()
        {
            return notification;
        }

        /// <summary>
        /// 通知的事件消息
        /// </summary>
        private readonly PendingIntent pendingIntent = null;

        public NotificationUtil(Activity context, int icon, string title, string message, string channelId, string channelName)
        {
            this.ctx = context;

            // 创建一个NotificationManager的引用
            notificationManager = (NotificationManager)ctx.GetSystemService(Context.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                // importance: 通知重要性
                // IMPORTANCE_HIGH：紧急级别（发出通知声音并显示为提示通知）
                // IMPORTANCE_DEFAULT：高级别（发出通知声音并且通知栏有通知）
                // IMPORTANCE_LOW：中等级别（没有通知声音但通知栏有通知）
                // IMPORTANCE_MIN：低级别（没有通知声音也不会出现在状态栏）
                var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Low);

                //是否绕过请勿打扰模式
                channel.CanBypassDnd();

                //设置可绕过请勿打扰模式
                channel.SetBypassDnd(true);

                //锁屏显示通知
                channel.LockscreenVisibility = NotificationVisibility.Secret;

                //桌面launcher的消息角标
                channel.CanShowBadge();

                notificationManager.CreateNotificationChannel(channel);
            }


            // 定义Notification的各种属性
            var builder = new NotificationCompat.Builder(context, channelId)
                     .SetContentTitle(title)
                     .SetContentText(message)
                     .SetSmallIcon(icon)
                     .SetAutoCancel(true)//用户触摸时，自动关闭
                     .SetOngoing(true);//设置处于运行状态

            //设置通知的事件消息
            Intent notificationIntent = new Intent(ctx, ctx.Class);
            notificationIntent.SetAction(Intent.ActionMain);
            notificationIntent.AddCategory(Intent.CategoryLauncher);

            pendingIntent = PendingIntent.GetActivity(ctx, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);

            builder.SetContentIntent(pendingIntent);

            //Android 10
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                //全屏通知
                builder.SetFullScreenIntent(pendingIntent, true);
            }

            notification = builder.Build();
        }

        /// <summary>
        /// 显示Notification
        /// </summary>
        public void ShowNotification()
        {
            notificationManager.Notify(NOTIFICATION_ID, notification);
        }

        /// <summary>
        /// 取消通知
        /// </summary>
        public void CancelNotification()
        {
            notificationManager.Cancel(NOTIFICATION_ID);
        }
    }
}