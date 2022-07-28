using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Annotations;
using DCMS.Client.Droid.KeepLive.config;
using DCMS.Client.Droid.KeepLive.receiver;
using DCMS.Client.Droid.KeepLive.utils;
//using Java.Lang;
//using System;

namespace DCMS.Client.Droid.KeepLive.service
{
    [RequiresApi(Value = 21)]
    [Service(Name = "com.dcms.clientv3.JobHandlerService")]
    public class JobHandlerService : JobService
    {
        private JobScheduler mJobScheduler;
        private int jobId = 100;

        public static readonly string EXTRA_NOTIFICATION_CONTENT = "notification_content";

        public override void OnCreate()
        {
            base.OnCreate();
        }


        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent == null)
                return StartCommandResult.NotSticky;

            this.TryStartService(this);

            //注册Job
            StartJob(this, intent);

            return StartCommandResult.Sticky;
        }


        public override void OnDestroy()
        {
            StopForeground(true);
            mJobScheduler.Cancel(this.jobId);
            this.jobId = -1;
            base.OnDestroy();
        }

        private void StartJob(Context context, Intent implicitIntent)
        {
            try
            {
                this.mJobScheduler = (JobScheduler)this.GetSystemService(Context.JobSchedulerService);
                this.mJobScheduler.Cancel(this.jobId);

                if (this.jobId != -1)
                {
                    this.mJobScheduler.Cancel(this.jobId);
                }

                jobId = 100;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    ComponentName component = null;

                    PackageManager pm = context.PackageManager;
                    //查出所有的能匹配这个隐式意图的服务列表
                    var resolveInfo = pm.QueryIntentServices(implicitIntent, 0);
                    if (resolveInfo != null || resolveInfo.Count == 1)
                    {
                        ResolveInfo serviceInfo = resolveInfo[0];
                        //取出包名
                        string packageName = serviceInfo.ServiceInfo.PackageName;
                        //取出服务名
                        string className = serviceInfo.ServiceInfo.Name;
                        //用包名和服务名来创建一个ComponentName对象
                        component = new ComponentName(packageName, className);
                    }

                    if (component != null)
                    {
                        var builder = new JobInfo.Builder(this.jobId, component);

                        //24 
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                        {
                            //7.0以上延迟3s执行
                            builder.SetMinimumLatency(30000L);////执行的最小延迟时间
                            builder.SetOverrideDeadline(30000L);////执行的最长延时时间
                            builder.SetMinimumLatency(30000L);
                            builder.SetBackoffCriteria(30000L, 0);//线性重试方案
                        }
                        else
                        {
                            //每隔3s执行一次job
                            builder.SetPeriodic(30000L);
                        }

                        builder.SetRequiredNetworkType(NetworkType.Any);

                        // 当插入充电器，执行该任务
                        builder.SetRequiresCharging(true);
                        builder.SetPersisted(true);

                        this.mJobScheduler.Schedule(builder.Build());
                    }
                }
            }
            catch (Java.Lang.Exception ex)
            {
                ex.PrintStackTrace();
            }
        }



        /// <summary>
        ///  //要求service 一旦startForegroundService() 启动，必须要在service 中startForeground()，
        ///  /如果在这之前stop 或stopSelf，那就会用crash 来代替ANR
        /// </summary>
        /// <param name="context"></param>
        private void TryStartService(Context context)
        {
            Intent localIntent;
            Intent RemoteIntent;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O && KeepLive.foregroundNotification != null)
            {
                localIntent = new Intent(ApplicationContext, typeof(NotificationClickReceiver));
                localIntent.SetAction("CLICK_NOTIFICATION");

                Notification notification = NotificationUtils.CreateNotification(this,
                    KeepLive.foregroundNotification.getTitle(),
                    KeepLive.foregroundNotification.getDescription(),
                    KeepLive.foregroundNotification.getIconRes(),
                    localIntent);

                // 开启前台进程 , API 26 以上无法关闭通知栏
                this.StartForeground(NotificationUtil.NOTIFICATION_ID, notification);
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr2 && KeepLive.foregroundNotification != null)
            {
                // API 18 ~ 25 以上的设备 , 启动相同 id 的前台服务 , 并关闭 , 可以关闭通知
                this.StartForeground(NotificationUtil.NOTIFICATION_ID, new Notification());
                StartService(new Intent(this,typeof(CancelNotificationService)));
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

            localIntent = new Intent(context, typeof(LocalService));
            RemoteIntent = new Intent(context, typeof(RemoteService));

            this.StartService(localIntent);
            this.StartService(RemoteIntent);
        }

        /// <summary>
        /// JOb 启动时
        /// </summary>
        /// <param name="jobParameters"></param>
        /// <returns></returns>
        public override bool OnStartJob(JobParameters jobParameters)
        {
            //判断服务是否在运行
            var isLocalServiceRun = ServiceUtils.isServiceRunning(ApplicationContext, "com.dcms.clientv3.LocalService");
            var isRemoteServiceRun = ServiceUtils.isRunningTaskExist(ApplicationContext, this.PackageName + ":dcRemoteService");
            if (!isLocalServiceRun || !isRemoteServiceRun)
            {
                this.TryStartService(this);
            }
            return false;
        }

        public override bool OnStopJob(JobParameters jobParameters)
        {
            //判断服务是否在运行
            var isLocalServiceRun = ServiceUtils.isServiceRunning(ApplicationContext, "com.dcms.clientv3.LocalService");
            var isRemoteServiceRun = ServiceUtils.isRunningTaskExist(ApplicationContext, this.PackageName + ":dcRemoteService");
            if (!isLocalServiceRun || !isRemoteServiceRun)
            {
                this.TryStartService(this);
            }
            return false;
        }

    }

}