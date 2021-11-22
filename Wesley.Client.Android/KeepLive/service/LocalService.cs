using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Runtime;
using DCMS.Client.Droid.KeepLive.config;
using DCMS.Client.Droid.KeepLive.receiver;
using DCMS.Client.Droid.KeepLive.utils;
using Java.Lang;
using System.Linq;
using System.Collections.Generic;


namespace DCMS.Client.Droid.KeepLive.service
{
    [Service(Name = "com.dcms.clientv3.LocalService")]
    public class LocalService : Service
    {
        private OnepxReceiver mOnepxReceiver;
        private ScreenStateReceiver screenStateReceiver;
        //控制暂停
        private static bool isPause = true;
        private static MediaPlayer mediaPlayer;
        private static Handler handler;

        private static MyServiceConnection connection;

        public override void OnCreate()
        {
            base.OnCreate();

            PowerManager pm = (PowerManager)GetSystemService(Context.PowerService);
            isPause = pm.IsScreenOn;
            if (handler == null)
                handler = new Handler();

            if (connection == null)
                connection = new MyServiceConnection(ApplicationContext, this);

        }

        public override IBinder OnBind(Intent intent)
        {
            return new MyBilder();
        }

        public class OnCompletionListener : Java.Lang.Object, MediaPlayer.IOnCompletionListener
        {
            public void OnCompletion(MediaPlayer mediaPlayer)
            {
                if (!isPause)
                {
                    if (KeepLive.runMode == KeepLive.RunMode.ROGUE)
                    {
                        Play();
                    }
                    else
                    {
                        if (handler != null)
                        {
                            handler.PostDelayed(() =>
                            {
                                new Thread(() =>
                                {
                                    Play();

                                }).Start();

                            }, 5000);
                        }
                    }
                }
            }
        }

        public class OnErrorListener : Java.Lang.Object, MediaPlayer.IOnErrorListener
        {
            public bool OnError(MediaPlayer mp, [GeneratedEnum] MediaError what, int extra)
            {
                return false;
            }
        }


        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (KeepLive.useSilenceMusice)
            {
                //播放无声音乐
                if (mediaPlayer == null)
                {
                    mediaPlayer = MediaPlayer.Create(this, Resource.Raw.cactus);
                    if (mediaPlayer != null)
                    {
                        mediaPlayer.SetVolume(0f, 0f);
                        mediaPlayer.SetOnCompletionListener(new OnCompletionListener());
                        mediaPlayer.SetOnErrorListener(new OnErrorListener());
                        Play();
                    }
                } 
            }

            //注册广播,像素保活
            if (mOnepxReceiver == null)
            {
                mOnepxReceiver = new OnepxReceiver();
            }
            IntentFilter intentFilter = new IntentFilter();
            intentFilter.AddAction("android.intent.action.SCREEN_OFF");//熄屏
            intentFilter.AddAction("android.intent.action.SCREEN_ON");//点亮
            RegisterReceiver(mOnepxReceiver, intentFilter);


            //注册广播,屏幕点亮状态监听，用于单独控制音乐播放
            if (screenStateReceiver == null)
            {
                screenStateReceiver = new ScreenStateReceiver();
            }
            IntentFilter intentFilter2 = new IntentFilter();
            intentFilter2.AddAction("_ACTION_SCREEN_OFF"); //熄屏
            intentFilter2.AddAction("_ACTION_SCREEN_ON");//点亮
            intentFilter2.AddAction("_ACTION_BACKGROUND");//后台
            intentFilter2.AddAction("_ACTION_FOREGROUND");//前台
             RegisterReceiver(screenStateReceiver, intentFilter2);


            //启用前台服务，提升优先级
            if (KeepLive.foregroundNotification != null)
            {
                TryStartService();
            }


            //隐藏服务通知
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.NMr1)
                {
                    StartService(new Intent(this, typeof(HideForegroundService)));
                }
            }
            catch (Java.Lang.Exception e)
            {
            }

            if (KeepLive.keepLiveService != null)
            {
                KeepLive.keepLiveService.onWorking();
            }

            return StartCommandResult.Sticky;
        }


        private void TryStartService()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O && KeepLive.foregroundNotification != null)
            {
               var intent = new Intent(ApplicationContext, typeof(NotificationClickReceiver));
                intent.SetAction("CLICK_NOTIFICATION");
                Notification notification = NotificationUtils.CreateNotification(this,
                    KeepLive.foregroundNotification.getTitle(),
                    KeepLive.foregroundNotification.getDescription(),
                    KeepLive.foregroundNotification.getIconRes(),
                    intent);

                // 开启前台进程 , API 26 以上无法关闭通知栏
                this.StartForeground(NotificationUtil.NOTIFICATION_ID, notification);
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr2 && KeepLive.foregroundNotification != null)
            {
                // API 18 ~ 25 以上的设备 , 启动相同 id 的前台服务 , 并关闭 , 可以关闭通知
                this.StartForeground(NotificationUtil.NOTIFICATION_ID, new Notification());
                StartService(new Intent(this, typeof(CancelNotificationService)));
            }
            else if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBeanMr2 && KeepLive.foregroundNotification != null)
            {
                // 将该服务转为前台服务
                // 需要设置 ID 和 通知
                // 设置 ID 为 0 , 就不显示已通知了 , 但是 oom_adj 值会变成后台进程 11
                // 设置 ID 为 1 , 会在通知栏显示该前台服务
                // 8.0 以上该用法报错
                StartForeground(NotificationUtil.NOTIFICATION_ID, new Notification());
            }
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

        private static void Play()
        {
            if (KeepLive.useSilenceMusice)
            {
                if (mediaPlayer != null && !mediaPlayer.IsPlaying)
                {
                    mediaPlayer.Start();
                }
            }
        }

        private static void Pause()
        {
            if (KeepLive.useSilenceMusice)
            {
                if (mediaPlayer != null && mediaPlayer.IsPlaying)
                {
                    mediaPlayer.Pause();
                }
            }
        }

        ///// <summary>
        ///// 打开一像素
        ///// </summary>
        //private static void OpenOnePix(Context? context)
        //{
        //    if (KeepLive.onePixEnabled)
        //    {
        //        if (handler != null)
        //        {
        //            handler.PostDelayed(() =>
        //            {
        //                new Thread(() =>
        //                {
        //                    KeepLive.StartOnePixActivity(context);

        //                }).Start();

        //            }, 1000);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 关闭一像素
        ///// </summary>
        //private static void CloseOnePix(Context? context)
        //{
        //    if (KeepLive.onePixEnabled)
        //    {
        //        KeepLive.BackBackground(context);
        //        KeepLive.FinishOnePix();
        //    }
        //}


        /// <summary>
        /// 屏幕息屏亮屏与前后台切换广播
        /// </summary>
        protected class ScreenStateReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context? context, Intent intent)
            {
                if (intent.Action.Equals("_ACTION_SCREEN_OFF"))
                {
                    // 熄屏
                    isPause = false;
                    //打开1像素Activity
                    //OpenOnePix(context);
                    //播放
                    Play();
                }
                else if (intent.Action.Equals("_ACTION_SCREEN_ON"))
                {
                    //亮屏
                    isPause = true;
                    //关闭1像素Activity
                    //CloseOnePix(context);
                    //暂停
                    Pause();

                }
                else if (intent.Action.Equals("_ACTION_BACKGROUND"))
                {
                    //后台
                    isPause = false;
                    //播放
                    Play();
                    //后台回调
                    OnBackground(true);
                }
                else if (intent.Action.Equals("_ACTION_FOREGROUND"))
                {
                    //前台
                    isPause = true;
                    //暂停
                    Pause();
                    //前台回调
                    OnBackground(false);
                }
            }
        }

        protected class MyBilder : IGuardAidlStub
        {
            public override void WakeUp(string title, string discription, int iconRes)
            {
               
            }
        }


        /// <summary>
        /// 是否是在后台
        /// </summary>
        public static void OnBackground(bool background)
        {
            if (KeepLive.BACKGROUND_CALLBACKS.Any())
            {
                foreach (var it in KeepLive.BACKGROUND_CALLBACKS)
                {
                    it.onBackground(background);
                }
            }
        }

        public class MyServiceConnection : Java.Lang.Object, IServiceConnection
        {
            LocalService _localService;
            Context _context;
            public MyServiceConnection(Context context, LocalService localService)
            {
                _localService = localService;
                _context = context;
            }

            /// <summary>
            /// 服务已断开时
            /// </summary>
            /// <param name="name"></param>
            public void OnServiceDisconnected(ComponentName name)
            {

                //屏幕锁屏广播
                PowerManager pm = (PowerManager)_context.GetSystemService(Context.PowerService);
                bool isScreenOn = pm.IsScreenOn;
                if (isScreenOn)
                {
                    _localService.SendBroadcast(new Intent("_ACTION_SCREEN_ON"));
                }
                else
                {
                    _localService.SendBroadcast(new Intent("_ACTION_SCREEN_OFF"));
                }
            }

            /// <summary>
            /// 服务已连接时
            /// </summary>
            /// <param name="name"></param>
            /// <param name="service"></param>
            public void OnServiceConnected(ComponentName name, IBinder service)
            {
                try
                {
                    if (service != null && KeepLive.foregroundNotification != null)
                    {
                        var guardAidl = new MyBilder();

                        //唤醒通知
                        guardAidl.WakeUp(KeepLive.foregroundNotification.getTitle(), 
                            KeepLive.foregroundNotification.getDescription(),
                            KeepLive.foregroundNotification.getIconRes());
                    }
                }
                catch (RemoteException e)
                {
                    e.PrintStackTrace();
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
                    if (mIsBoundRemoteService)
                    {
                        UnbindService(connection);
                    }
                }
                catch (Java.Lang.Exception e) { }
            }

            try
            {
                //卸载广播
                UnregisterReceiver(mOnepxReceiver);
                UnregisterReceiver(screenStateReceiver);
            }
            catch (Java.Lang.Exception e) { }

            if (KeepLive.keepLiveService != null)
            {
                KeepLive.keepLiveService.onStop();
            }
        }
    }
}