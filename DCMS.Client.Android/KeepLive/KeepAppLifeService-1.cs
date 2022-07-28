using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Microsoft.AppCenter.Crashes;
using Plugin.SimpleAudioPlayer;
using System;
using System.Reflection;

namespace DCMS.Client.Droid
{

    [Service(Name = "com.dcms.clientv3.KeepAppLifeService")]
    public class KeepAppLifeService : Service
    {
        public static readonly string EXTRA_NOTIFICATION_CONTENT = "notification_content";
        private static readonly string CHANNEL_ID = "com.dcms.clientv3.keep";
        private static readonly string CHANNEL_NAME = "dcms_default_channel";
        private PowerManager.WakeLock _wakeLock;
        private NotificationUtil notificationUtil;
        private ISimpleAudioPlayer _mediaPlayer = null;
        private bool IsRun { get; set; } = false;


        public override void OnCreate()
        {
            base.OnCreate();

            IsRun = true;

            //获取电源锁
            AcquireWakeLock();

            if (_mediaPlayer == null)
                _mediaPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();

            TryStartForeground();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent == null)
            {
                return StartCommandResult.NotSticky;
            }

            try
            {
                //要求service 一旦startForegroundService() 启动，必须要在service 中startForeground()，
                //如果在这之前stop 或stopSelf，那就会用crash 来代替ANR
                TryStartForeground(intent);

                //播放MP3
                if (_mediaPlayer != null && !_mediaPlayer.IsPlaying)
                {
                    var assembly = typeof(App).GetTypeInfo().Assembly;
                    var path = "DCMS.Client.Resources.raw";
                    using (var _audioStream = assembly?.GetManifestResourceStream($"{path}.cactus.mp3"))
                    {
                        if (_audioStream != null)
                        {
                            _mediaPlayer.Load(_audioStream);
                            _mediaPlayer.Loop = true;
                            _mediaPlayer.Play();
                            IsRun = true;
                        }
                    }
                }
            }
            catch (System.InvalidOperationException) { }
            catch (System.Exception) { }

            return StartCommandResult.Sticky;
        }


        private void TryStartForeground(Intent intent = null)
        {
            try
            {

                string content = "DCMS正在后台运行....";

                if (intent != null)
                    content = intent.GetStringExtra(EXTRA_NOTIFICATION_CONTENT);

                //创建Notification
                notificationUtil = new NotificationUtil(MainActivity.Instance,
                    Resource.Mipmap.ic_launcher,
                    "DCMS",
                    content,
                    CHANNEL_ID,
                    CHANNEL_NAME);

                //将服务放置前台
                //startForeground() 中的id 和notification 不能为0 和 null
                StartForeground(NotificationUtil.NOTIFICATION_ID, notificationUtil.GetNotification());
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// 销毁时
        /// </summary>
        public override void OnDestroy()
        {
            try
            {
                IsRun = false;

                //取消通知
                if (notificationUtil != null)
                {
                    notificationUtil.CancelNotification();
                    notificationUtil = null;
                }

                //退出服务
                StopForeground(true);

                //停止播放
                if (_mediaPlayer != null)
                {
                    _mediaPlayer.Stop();
                }

                //停止服务
                this.StopSelf();

                //释放电源锁
                ReleaseWakeLock();

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                base.OnDestroy();
            }
        }


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }


        /// <summary>
        /// 获取电源锁
        /// </summary>
        private void AcquireWakeLock()
        {
            if (null == _wakeLock)
            {
                using (PowerManager pm = (PowerManager)GetSystemService(Context.PowerService))
                {
                    _wakeLock = pm.NewWakeLock(WakeLockFlags.Partial | WakeLockFlags.AcquireCausesWakeup, "MyWakelockTag");
                    if (_wakeLock != null)
                    {
                        _wakeLock.Acquire();

                        //服务没运行时
                        if (!IsRun)
                        {
                            if (_mediaPlayer != null && !_mediaPlayer.IsPlaying)
                            {
                                var assembly = typeof(App).GetTypeInfo().Assembly;
                                var path = "DCMS.Client.Resources.raw";
                                using (var _audioStream = assembly?.GetManifestResourceStream($"{path}.cactus.mp3"))
                                {
                                    if (_audioStream != null)
                                    {
                                        _mediaPlayer.Load(_audioStream);
                                        _mediaPlayer.Loop = true;
                                        _mediaPlayer.Play();
                                        IsRun = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 释放电源锁
        /// </summary>
        private void ReleaseWakeLock()
        {
            if (null != _wakeLock || _wakeLock.IsHeld)
            {
                if (!IsRun)
                {
                    //停止播放
                    if (_mediaPlayer != null)
                    {
                        _mediaPlayer.Stop();
                    }
                }

                _wakeLock?.Release();
                _wakeLock = null;
            }
        }

    }

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

        ///// <summary>
        ///// 显示Notification
        ///// </summary>
        //public void ShowNotification()
        //{
        //    notificationManager.Notify(NOTIFICATION_ID, notification);
        //}

        /// <summary>
        /// 取消通知
        /// </summary>
        public void CancelNotification()
        {
            notificationManager.Cancel(NOTIFICATION_ID);
        }
    }
}