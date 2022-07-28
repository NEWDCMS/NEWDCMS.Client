using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using System;


namespace DCMS.Client.Droid
{
    [BroadcastReceiver(Name = "com.dcms.clientv3.KeepEventBroadcastReceiver")]
    public class KeepEventBroadcastReceiver : BroadcastReceiver
    {
        private Context context;

        public override void OnReceive(Context context, Intent intent)
        {
            this.context = context;
            string action = intent.Action;

            if ("android.intent.action.USER_PRESENT".Equals(action) || "android.intent.action.ACTION_POWER_CONNECTED".Equals(action) || "android.intent.action.ACTION_POWER_DISCONNECTED".Equals(action))
            {
                this.StartPushService();
            }

            ///切换到后台 弹通知栏
            if (MainApplication.GetInstance().IsAppInBackground())
            {
                ShowMessageNotification("DCMS正在后台运行....");
            }
        }

        private void ShowMessageNotification(string message)
        {
            NotificationManagerCompat notificationManager = NotificationManagerCompat.From(MainApplication.GetInstance());

            var builder = new NotificationCompat.Builder(MainApplication.GetInstance(), MainApplication.NOTIFICATION_CHANNEL_ID);

            Intent intent = new Intent();
            intent.SetClass(MainApplication.GetInstance(), typeof(MainActivity));
            PendingIntent pendingIntent = PendingIntent.GetActivity(MainApplication.GetInstance(), 1, intent, PendingIntentFlags.UpdateCurrent);

            var currenttimemillis = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

            builder.SetAutoCancel(true);
            builder.SetContentIntent(pendingIntent);
            builder.SetWhen(currenttimemillis);
            builder.SetVisibility(NotificationCompat.VisibilityPublic);
            builder.SetDefaults(NotificationCompat.DefaultLights);

            builder.SetContentTitle(MainApplication.NOTIFICATION_CHANNEL_NAME);
            builder.SetContentText(message);

            Notification notification = builder.Build();
            notificationManager.Notify(0, notification);
        }

        private void StartPushService()
        {
            Intent intent = new Intent(this.context, typeof(KeepAppLifeService));
            intent.SetAction(KeepAppLifeManager.ACTION_ACTIVATE_PUSH_SERVICE);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                this.context.StartForegroundService(intent);
            }
            else
            {
                this.context.StartService(intent);
            }
        }
    }
}