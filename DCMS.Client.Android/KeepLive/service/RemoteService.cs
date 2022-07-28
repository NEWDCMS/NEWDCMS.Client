using Android.App;
using Android.Content;
//using Android.Annotation;
//using AndroidX.Annotations;
using Android.Content.PM;
using Android.OS;
using DCMS.Client.Droid.KeepLive.config;
using DCMS.Client.Droid.KeepLive.receiver;
using DCMS.Client.Droid.KeepLive.utils;


namespace DCMS.Client.Droid.KeepLive.service
{
    /// <summary>
    /// 守护进程
    /// </summary>
    //[Service(Name = "com.dcms.clientv3.RemoteService")]
    public class RemoteService : Service
    {
        /// <summary>
        /// 是否绑定本地服务
        /// </summary>
        private static bool mIsBoundLocalService;
        private static MyServiceConnection connection;

        public override IBinder OnBind(Intent intent)
        {
            return new MyBilder(ApplicationContext, this);
        }


        public override void OnCreate()
        {
            base.OnCreate();

            if (connection == null)
                connection = new MyServiceConnection(ApplicationContext, this);
        }


        /// <summary>
        /// 在Android5.0及以上系统中需要显式声明Intent才能启动Service
        /// </summary>
        /// <param name="context"></param>
        /// <param name="implicitIntent"></param>
        /// <returns></returns>
        private Intent CreateExplicitFromImplicitIntent(Context context, Intent implicitIntent)
        {
            PackageManager pm = context.PackageManager;

            //查出所有的能匹配这个隐式意图的服务列表
            var resolveInfo = pm.QueryIntentServices(implicitIntent, 0);
            if (resolveInfo == null || resolveInfo.Count != 1)
            {
                return null;
            }

            ResolveInfo serviceInfo = resolveInfo[0];

            //取出包名
            string packageName = serviceInfo.ServiceInfo.PackageName;

            //取出服务名
            string className = serviceInfo.ServiceInfo.Name;

            //用包名和服务名来创建一个ComponentName对象
            ComponentName component = new ComponentName(packageName, className);

            //拿隐式意图对象implicitIntent作为构造参数，来创建一个新的显示的意图
            Intent explicitIntent = new Intent(implicitIntent);

            //设置显示意图的组件名对象
            explicitIntent.SetComponent(component);

            return explicitIntent;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                //mIsBoundLocalService = this.BindService(new Intent(this, typeof(LocalService)), connection, Bind.AboveClient);
                //Intent localIntent = new Intent("com.dcms.clientv3.LocalService");
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    Intent localIntent = new Intent(this, typeof(LocalService));

                    //（在Android5.0及以上系统中需要显式声明Intent才能启动Service）
                    var tmplicitIntent = CreateExplicitFromImplicitIntent(this, localIntent);

                    //绑定守护进程
                    mIsBoundLocalService = this.BindService(tmplicitIntent, connection, Bind.AboveClient);
                }
            }
            catch (Java.Lang.Exception e)
            {
            }
            return StartCommandResult.Sticky;
        }

        public class MyServiceConnection : Java.Lang.Object, IServiceConnection
        {
            RemoteService _remoteService;
            Context _context;
            public MyServiceConnection(Context context, RemoteService remoteService)
            {
                _remoteService = remoteService;
                _context = context;
            }

            /// <summary>
            /// 服务已断开时
            /// </summary>
            /// <param name="name"></param>
            public void OnServiceDisconnected(ComponentName name)
            {
                if (ServiceUtils.isRunningTaskExist(_context, _context.PackageName + ":dcRemoteService"))
                {
                    Intent localService = new Intent(_context, typeof(LocalService));
                    //启动本地服务
                    _remoteService.StartService(localService);

                    //绑定守护进程
                    mIsBoundLocalService = _remoteService.BindService(new Intent(_remoteService,
                            typeof(LocalService)), connection, Bind.AboveClient);
                }

                //屏幕锁屏广播
                PowerManager pm = (PowerManager)_context.GetSystemService(Context.PowerService);
                bool isScreenOn = pm.IsScreenOn;
                if (isScreenOn)
                {
                    _remoteService.SendBroadcast(new Intent("_ACTION_SCREEN_ON"));
                }
                else
                {
                    _remoteService.SendBroadcast(new Intent("_ACTION_SCREEN_OFF"));
                }
            }

            /// <summary>
            /// 服务已连接时
            /// </summary>
            /// <param name="name"></param>
            /// <param name="service"></param>
            public void OnServiceConnected(ComponentName name, IBinder service) { }
        }


        protected class MyBilder : IGuardAidlStub
        {
            readonly RemoteService _remoteService;
            readonly Context _context;
            public MyBilder(Context context, RemoteService remoteService)
            {
                _remoteService = remoteService;
                _context = context;
            }

            /// <summary>
            /// 唤醒
            /// </summary>
            /// <param name="title"></param>
            /// <param name="discription"></param>
            /// <param name="iconRes"></param>
            public override void WakeUp(string title, string discription, int iconRes)
            {
                //if (Build.VERSION.SdkInt < BuildVersionCodes.NMr1)
                //{
                //    Intent intent = new Intent(_context, typeof(NotificationClickReceiver));
                //    intent.SetAction(NotificationClickReceiver.CLICK_NOTIFICATION);
                //    Notification notification = NotificationUtils.createNotification(_context, title, discription, iconRes, intent);

                //    //开启前台进程
                //    _remoteService.StartForeground(NotificationUtil.NOTIFICATION_ID, notification);
                //}

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var intent = new Intent(_context, typeof(NotificationClickReceiver));
                    intent.SetAction("CLICK_NOTIFICATION");
                    Notification notification = NotificationUtils.CreateNotification(_remoteService,
                        KeepLive.foregroundNotification.getTitle(),
                        KeepLive.foregroundNotification.getDescription(),
                        KeepLive.foregroundNotification.getIconRes(),
                        intent);

                    // 开启前台进程 , API 26 以上无法关闭通知栏
                    _remoteService.StartForeground(NotificationUtil.NOTIFICATION_ID, notification);
                }
                else if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr2 )
                {
                    // API 18 ~ 25 以上的设备 , 启动相同 id 的前台服务 , 并关闭 , 可以关闭通知
                    _remoteService.StartForeground(NotificationUtil.NOTIFICATION_ID, new Notification());
                    _remoteService.StartService(new Intent(_remoteService, typeof(CancelNotificationService)));
                }
                else if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBeanMr2)
                {
                    // 将该服务转为前台服务
                    // 需要设置 ID 和 通知
                    // 设置 ID 为 0 , 就不显示已通知了 , 但是 oom_adj 值会变成后台进程 11
                    // 设置 ID 为 1 , 会在通知栏显示该前台服务
                    // 8.0 以上该用法报错
                    _remoteService.StartForeground(NotificationUtil.NOTIFICATION_ID, new Notification());
                }
            }
        }


        public override void OnDestroy()
        {
            base.OnDestroy();
            if (connection != null)
            {
                try
                {
                    if (mIsBoundLocalService)
                    {
                        UnbindService(connection);
                    }
                }
                catch (Java.Lang.Exception e) { }
            }
        }
    }
}