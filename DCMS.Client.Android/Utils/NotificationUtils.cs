using Android.App;
using Android.Content;
using Android.Graphics;
using AndroidX.Annotations;
using System;

namespace DCMS.Client.Droid.Utils
{
    public class NotificationUtils : ContextWrapper
    {
        private NotificationManager mManager;
        public static readonly string ANDROID_CHANNEL_ID = "com.baidu.baidulocationdemo";
        public static readonly string ANDROID_CHANNEL_NAME = "ANDROID CHANNEL";

        [RequiresApi(Api = 26)]
        public NotificationUtils(Context context) : base(context)
        {
            CreateChannels();
        }

        [RequiresApi(Api = 26)]
        public void CreateChannels()
        {
            // create android channel
            NotificationChannel androidChannel = new NotificationChannel(ANDROID_CHANNEL_ID,
                    ANDROID_CHANNEL_NAME, NotificationImportance.Default);
            // Sets whether notifications posted to this channel should display notification lights
            androidChannel.EnableLights(true);
            // Sets whether notification posted to this channel should vibrate.
            androidChannel.EnableVibration(true);
            // Sets the notification light color for notifications posted to this channel
            androidChannel.LightColor = Color.Green;
            // Sets whether notifications posted to this channel appear on the lockscreen or not
            androidChannel.LockscreenVisibility = NotificationVisibility.Private;
            GetManager().CreateNotificationChannel(androidChannel);
        }

        private NotificationManager GetManager()
        {
            if (mManager == null)
            {
                mManager = (NotificationManager)GetSystemService(Context.NotificationService);
            }
            return mManager;
        }

        [RequiresApi(Api = 26)]
        public Notification.Builder GetAndroidChannelNotification(string title, string body)
        {
            return new Notification.Builder(ApplicationContext, ANDROID_CHANNEL_ID)
                    .SetContentTitle(title)
                    .SetContentText(body)
                    .SetSmallIcon(Android.Resource.Drawable.StatNotifyMore)
                    .SetAutoCancel(true);
        }
    }
}