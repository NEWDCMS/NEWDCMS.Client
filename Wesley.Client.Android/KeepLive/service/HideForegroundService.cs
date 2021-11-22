using Android.App;
using Android.Content;
using Android.OS;

using DCMS.Client.Droid.KeepLive.config;
using DCMS.Client.Droid.KeepLive.utils;
using DCMS.Client.Droid.KeepLive.receiver;
using Java.Lang;

namespace DCMS.Client.Droid.KeepLive.service
{
    /// <summary>
    /// 隐藏前台服务通知
    /// </summary>
    [Service(Name = "com.dcms.clientv3.HideForegroundService")]
    public class HideForegroundService : Service
    {
        private Handler handler;
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            TryStartForeground();

            if (handler == null)
            {
                handler = new Handler();
            }

            handler.PostDelayed(() =>
            {
                new Thread(() =>
                {
                    StopForeground(true);
                    StopSelf();

                }).Start();
            }, 2000);

            return StartCommandResult.Sticky;
        }

        private void TryStartForeground()
        {
            if (KeepLive.foregroundNotification != null)
            {
                Intent intent = new Intent(ApplicationContext, typeof(NotificationClickReceiver));
                intent.SetAction(NotificationClickReceiver.CLICK_NOTIFICATION);

                //26
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    Notification notification = NotificationUtils.CreateNotification(this,
                        KeepLive.foregroundNotification.getTitle(),
                        KeepLive.foregroundNotification.getDescription(),
                        KeepLive.foregroundNotification.getIconRes(),
                        intent);
                    StartForeground(NotificationUtil.NOTIFICATION_ID, notification);
                }
                else
                {
                    this.StartService(intent);
                }
            }
        }

    }
}