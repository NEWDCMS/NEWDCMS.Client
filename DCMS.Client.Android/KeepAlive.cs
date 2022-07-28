using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Views;
using AndroidX.Core.App;
using AndroidX.Work;
using System;


using DCMS.Client.BaiduMaps;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace DCMS.Client.Droid
{
    #region workmanager

    ///// <summary>
    ///// 利用jetpack中的workManager启动WorkJobService进行保活
    ///// </summary>
    //public class KeepLiveWork : Worker
    //{
    //    private Context _context;
    //    public KeepLiveWork(Context context, WorkerParameters workerParams) : base(context, workerParams)
    //    {
    //        _context = context;
    //    }
    //    public override Result DoWork()
    //    {
    //        try
    //        {
    //            //启动job服务
    //            WorkJobService.StartJob(_context);
    //            return new Result.Success();
    //        }
    //        catch (Exception)
    //        {
    //            return new Result.Failure();
    //        }
    //    }
    //}

    [Service(Name = "com.dcms.clientv3.WorkJobService", Permission = "android.permission.BIND_JOB_SERVICE")]
    public class WorkJobService : JobService
    {
        private static readonly string TAG = "WorkJobService";
        public static void StartJob(Context context)
        {
            try
            {
                int jobId = 8;
                Log.Error(TAG, "startJob");

                JobScheduler jobScheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);

                var componentName = new ComponentName(context, Java.Lang.Class.FromType(typeof(WorkJobService)));

                var builder = new JobInfo.Builder(jobId, componentName).SetPersisted(true);

                // 小于7.0
                if (Build.VERSION.SdkInt < BuildVersionCodes.N)
                {
                    Log.Error(TAG, "每隔 1s 执行一次 job");
                    builder.SetPeriodic(1000);
                }
                else
                { 
                    // 延迟执行任务
                    builder.SetMinimumLatency(1000);
                }

                int resultCode = jobScheduler.Schedule(builder.Build());
            }
            catch (Exception ex)
            {
                Log.Error(TAG, ex.Message);
            }
        }
        public override bool OnStartJob(JobParameters jobParameters)
        {
          
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                Log.Error(TAG, "onStartJob");
                StartJob(this);
            }

            return false;
        }
        public override bool OnStopJob(JobParameters jobParameters)
        {
            return false;
        }

    }

    #endregion

    #region jobscheduler

    //[Service(Name = "com.dcms.clientv3.AliveJobService", Permission = "android.permission.BIND_JOB_SERVICE")]
    //public class AliveJobService : JobService
    //{
    //    private static readonly string TAG = "AliveJobService";
    //    public static void StartJob(Context context)
    //    {
    //        try
    //        {
    //            int jobId = 8;

    //            JobScheduler jobScheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);

    //            //setPersisted 在设备重启依然执行
    //            var componentName = new ComponentName(context, Java.Lang.Class.FromType(typeof(AliveJobService)));
    //            var builder = new JobInfo.Builder(jobId, componentName).SetPersisted(true);
    //            // 小于7.0
    //            if (Build.VERSION.SdkInt < BuildVersionCodes.N)
    //            {
    //                // 每隔 1s 执行一次 job
    //                // 版本23 开始 进行了改进，最小周期为 5s
    //                Log.Error(TAG, "每隔 1s 执行一次 job");
    //                builder.SetPeriodic(1000);
    //            }
    //            else
    //            {
    //                // 延迟执行任务
    //                builder.SetMinimumLatency(1000);
    //            }

    //            int resultCode = jobScheduler.Schedule(builder.Build());
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error(TAG, ex.Message);
    //        }
    //    }
    //    public override bool OnStartJob(JobParameters jobParameters)
    //    {
    //        // 如果7.0以上 轮询
    //        if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
    //        {
    //            Log.Error(TAG, "onStartJob");
    //            StartJob(this);
    //        }

    //        return false;
    //    }
    //    public override bool OnStopJob(JobParameters jobParameters)
    //    {
    //        return false;
    //    }
    //}

    #endregion

    #region service

    ///// <summary>
    ///// 使用粘性服务进行保活
    ///// </summary>
    //[Service(Name = "com.dcms.clientv3.StickyService")]
    //public class StickyService : Service
    //{
    //    CancellationTokenSource _cts;
    //    public override IBinder OnBind(Intent intent)
    //    {
    //        return null;
    //    }

    //    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    //    {
    //        _cts = new CancellationTokenSource();

    //        Task.Run(async () =>
    //        {
    //            Location locShared = null;
    //            try
    //            {
    //                locShared = new Location();
    //                await locShared.Run(_cts.Token);
    //            }
    //            catch (Android.OS.OperationCanceledException)
    //            {
    //            }
    //            finally
    //            {
    //                if (_cts.IsCancellationRequested)
    //                {
    //                   //发送通知停止服务
    //                }
    //            }
    //        }, _cts.Token);

    //        return StartCommandResult.Sticky;
    //    }

    //    public override void OnDestroy()
    //    {
    //        if (_cts != null)
    //        {
    //            _cts.Token.ThrowIfCancellationRequested();
    //            _cts.Cancel();
    //        }
    //        base.OnDestroy();
    //    }


    //    public class Location
    //    {
    //        readonly bool stopping = false;
    //        public Location()
    //        {
    //            try
    //            {
    //                //
    //            }
    //            catch (Java.Lang.Exception ex)
    //            {
    //                Android.Util.Log.Error("StickyService", ex.Message);
    //            }
    //        }

    //        public async Task Run(CancellationToken token)
    //        {
    //            await Task.Run(async () =>
    //            {
    //                while (!stopping)
    //                {
    //                    token.ThrowIfCancellationRequested();
    //                    try
    //                    {
    //                        await Task.Delay(2000);
    //                        System.Diagnostics.Debug.Print($"间隔8秒GPS定位一次.....");
    //                        var request = new GeolocationRequest(GeolocationAccuracy.Best);
    //                        var location = await Geolocation.GetLocationAsync(request);
    //                        if (location != null)
    //                        {
    //                            GlobalSettings.Latitude = location.Latitude;
    //                            GlobalSettings.Longitude = location.Longitude;
    //                            //System.Diagnostics.Debug.Print($"{location.Latitude} {location.Longitude}");
    //                        }
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        Android.Util.Log.Error("StickyService", ex.Message);
    //                    }
    //                }
    //            }, token);
    //        }
    //    }
    //}


    ///// <summary>
    ///// 不同版本有差异，需分开处理 此种方法适用于音乐播放器保活，8.0以后会在通知栏显示
    ///// </summary>
    //[Service(Name = "com.dcms.clientv3.ForegroundService")]
    //public class ForegroundService : Service
    //{
    //    private static readonly string TAG = "ForegroundService";
    //    private static readonly int SERVICE_ID = 1;

    //    public override IBinder OnBind(Intent intent)
    //    {
    //        return null;
    //    }

    //    public override void OnCreate()
    //    {
    //        base.OnCreate();

    //        Log.Error(TAG, "ForegroundService 服务创建了");

    //        if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBeanMr2)
    //        {
    //            //4.3以下
    //            //将service设置成前台服务，并且不显示通知栏消息
    //            StartForeground(SERVICE_ID, new Notification());
    //        }
    //        else if (Build.VERSION.SdkInt < BuildVersionCodes.O)
    //        {
    //            //Android4.3-->Android7.0
    //            //将service设置成前台服务
    //            StartForeground(SERVICE_ID, new Notification());
    //            //删除通知栏消息
    //            StartService(new Intent(this, typeof(InnerService)));
    //        }
    //        else
    //        {
    //            // 8.0 及以上
    //            //通知栏消息需要设置channel
    //            NotificationManager manager = (NotificationManager)GetSystemService(NotificationService);
    //            //NotificationManager.IMPORTANCE_MIN 通知栏消息的重要级别  最低，不让弹出
    //            //IMPORTANCE_MIN 前台时，在阴影区能看到，后台时 阴影区不消失，增加显示 IMPORTANCE_NONE时 一样的提示
    //            //IMPORTANCE_NONE app在前台没有通知显示，后台时有
    //            NotificationChannel channel = new NotificationChannel("channel", "DCMS位置服务", NotificationImportance.None);
    //            if (manager != null)
    //            {
    //                manager.CreateNotificationChannel(channel);
    //                Notification notification = new NotificationCompat.Builder(this, "channel").Build();
    //                //将service设置成前台服务，8.x退到后台会显示通知栏消息，9.0会立刻显示通知栏消息
    //                StartForeground(SERVICE_ID, notification);
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 内联服务
    //    /// </summary>
    //    [Service(Name = "com.dcms.clientv3.ForegroundService$InnerService")]
    //    public class InnerService : Service
    //    {
    //        public override void OnCreate()
    //        {
    //            base.OnCreate();

    //            Log.Error(TAG, "InnerService 服务创建了");
    //            // 让服务变成前台服务
    //            StartForeground(SERVICE_ID, new Notification());
    //            // 关闭自己
    //            StopSelf();
    //        }

    //        public override IBinder OnBind(Intent intent)
    //        {
    //            return null;
    //        }

    //        public override void OnDestroy()
    //        {
    //            base.OnDestroy();
    //        }
    //    }

    //    public override void OnDestroy()
    //    {
    //        base.OnDestroy();
    //    }
    //}


    #endregion

    #region activity
    /*
    /// <summary>
    /// 用于保活的1像素activity
    /// </summary>
    [Activity(Name = "com.dcms.clientv3.AliveActivity",
        Theme = "@style/KeepAliveTheme",
        ExcludeFromRecents = true,
        TaskAffinity = "com.dcms.clientv3")]
    public class AliveActivity : Activity
    {
        private static readonly string TAG = "AliveActivity";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Log.Debug(TAG, "AliveActivity启动");

            Window window = this.Window;
            window.SetGravity(GravityFlags.Start | GravityFlags.Top);
            WindowManagerLayoutParams @params = window.Attributes;

            //宽高
            @params.Width = 1;
            @params.Height = 1;
            //设置位置
            @params.X = 0;
            @params.Y = 0;
            window.Attributes = @params;

            KeepManager.GetInstance().SetKeep(this);
        }


        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(0, 0);
        }

        protected override void OnDestroy()
        {
            Log.Debug(TAG, "AliveActivity关闭");

            base.OnDestroy();
        }
    }

    /// <summary>
    /// 息屏广播监听
    /// </summary>
    public class KeepAliveReceiver : BroadcastReceiver
    {
        private static readonly string TAG = "KeepAliveReceiver";
        public override void OnReceive(Context context, Intent intent)
        {
            string action = intent.Action;
            Log.Debug(TAG, "onReceive:" + action);
            if (TextUtils.Equals(action, Intent.ActionScreenOff))
            {
                Log.Debug(TAG, "息屏 开启");
                KeepManager.GetInstance().StartKeep(context);
            }
            else if (TextUtils.Equals(action, Intent.ActionScreenOn))
            {
                Log.Debug(TAG, "开屏 关闭");
                KeepManager.GetInstance().FinishKeep();
            }
        }
    }

    /// <summary>
    ///  1像素activity保活管理类
    /// </summary>
    public class KeepManager
    {
        private static readonly KeepManager mInstance = new KeepManager();
        private KeepAliveReceiver mKeepAliveReceiver;

        // 使用弱引用，防止内存泄漏
        private WeakReference<Activity> mKeepActivity;

        public static KeepManager GetInstance()
        {
            return mInstance;
        }

        /// <summary>
        /// 注册 开屏 关屏 广播
        /// </summary>
        /// <param name="context"></param>
        public void RegisterKeep(Context context)
        {
            try
            {
                IntentFilter filter = new IntentFilter();

                filter.AddAction(Intent.ActionScreenOn);
                filter.AddAction(Intent.ActionScreenOff);

                mKeepAliveReceiver = new KeepAliveReceiver();
                context.RegisterReceiver(mKeepAliveReceiver, filter);
            }
            catch (Java.Lang.IllegalArgumentException ex)
            {
                Log.Error("RegisterKeep", ex.Message);
            }
        }

        /// <summary>
        /// 注销 广播接收者
        /// </summary>
        /// <param name="context"></param>
        public void UnregisterKeep(Context context)
        {
            try
            {
                if (mKeepAliveReceiver != null)
                {
                    context.UnregisterReceiver(mKeepAliveReceiver);
                }
            }
            catch (Java.Lang.IllegalArgumentException ex)
            {
                Log.Error("UnregisterKeep", ex.Message);
            }
        }

        /// <summary>
        /// 开启1像素Activity
        /// </summary>
        /// <param name="context"></param>
        public void StartKeep(Context context)
        {
            Intent intent = new Intent(context, typeof(AliveActivity));
            // 结合 taskAffinity 一起使用 在指定栈中创建这个activity
            intent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
        }

        /// <summary>
        /// 关闭1像素Activity
        /// </summary>
        public void FinishKeep()
        {
            if (mKeepActivity != null)
            {
                mKeepActivity.TryGetTarget(out Activity activity);
                if (activity != null)
                {
                    activity.Finish();
                }
                mKeepActivity = null;
            }
        }

        /// <summary>
        /// 设置弱引用
        /// </summary>
        /// <param name="keep"></param>
        public void SetKeep(AliveActivity keep)
        {
            mKeepActivity = new WeakReference<Activity>(keep);
        }
    }
    */
    #endregion
}