using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using DCMS.Client.Droid.KeepLive.utils;

namespace DCMS.Client.Droid.KeepLive.service
{

    /// <summary>
    /// API 18 ~ 25 以上的设备, 关闭通知到专用服务
    /// </summary>
    [Service(Name = "com.dcms.clientv3.CancelNotificationService")]
    public class CancelNotificationService : Service
    {
        public override void OnCreate()
        {
            base.OnCreate();
            StartForeground(NotificationUtil.NOTIFICATION_ID, new Notification());
            StopSelf();
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}