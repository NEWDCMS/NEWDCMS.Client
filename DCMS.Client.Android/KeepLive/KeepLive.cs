using Android.App;
using Android.Content;
using Android.OS;
using DCMS.Client.Droid.KeepLive.config;
using DCMS.Client.Droid.KeepLive.service;
using System;
using System.Collections.Generic;
using DCMS.Client.Droid.KeepLive.receiver;
using DCMS.Client.Droid.KeepLive.utils;

namespace DCMS.Client.Droid.KeepLive
{
    public class KeepLive
    {
        /// <summary>
        /// 运行模式
        /// </summary>
        public enum RunMode
        {
            /// <summary>
            /// 省电模式, 省电一些，但保活效果会差一点
            /// </summary>
            ENERGY,
            /// <summary>
            /// 流氓模式 相对耗电，但可造就不死之身
            /// </summary>
            ROGUE
        }

        /// <summary>
        /// 前台通知
        /// </summary>
        public static ForegroundNotification foregroundNotification = null;
        public static IKeepLiveService keepLiveService = null;

        /// <summary>
        /// 运行模式
        /// </summary>
        public static RunMode runMode;

        /// <summary>
        /// 启用背景音乐
        /// </summary>
        public static bool useSilenceMusice = true;

        /// <summary>
        /// 是否启用1像素
        /// </summary>
        public static bool onePixEnabled = true;

        /// <summary>
        /// 用来表示是前台还是后台
        /// </summary>
        public static bool mIsForeground = false;

        /// <summary>
        /// 用以保存一像素Activity
        /// </summary>
        //public static WeakReference<OnePixelActivity>? mWeakReference = null;

        /// <summary>
        /// 前后台回调集合
        /// </summary>
        public static List<ICactusBackgroundCallback> BACKGROUND_CALLBACKS = new List<ICactusBackgroundCallback>();


        /// <summary>
        /// 启动保活
        /// </summary>
        /// <param name="application">APP</param>
        /// <param name="runMode"></param>
        /// <param name="foregroundNotification">前台服务 必须要，安卓8.0后必须有前台通知才能正常启动Service</param>
        /// <param name="keepLiveService">保活业务</param>
        /// <param name="cactusBackgroundCallback">回调</param>
        public static void StartWork(Application? application, RunMode runMode, ForegroundNotification? foregroundNotification, IKeepLiveService? keepLiveService, ICactusBackgroundCallback cactusBackgroundCallback)
        {
            //是否主进程
            if (IsMain(application))
            {
                KeepLive.foregroundNotification = foregroundNotification;
                KeepLive.keepLiveService = keepLiveService;
                KeepLive.runMode = runMode;
                KeepLive.BACKGROUND_CALLBACKS.Add(cactusBackgroundCallback);

                //Api大于21 JobScheduler 拉活
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    //启动定时器，在定时器中启动本地服务和守护进程
                    Intent intent = new Intent(application, typeof(JobHandlerService));

                    //Api大于29
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                    {
                        //intent = new Intent(this, typeof(KeepAppLifeService));
                        //intent.PutExtra(KeepAppLifeService.EXTRA_NOTIFICATION_CONTENT, "DCMS正在后台运行....");
                        application.StartForegroundService(intent);
                    }
                    //Api大于26
                    else if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {
                        //注意26以下不支持
                        application.StartForegroundService(intent);
                    }
                    else
                    {
                        application.StartService(intent);
                    }

                    //Android.Util.AndroidRuntimeException: 'Context.startForegroundService() did not then call Service.startForeground(): ServiceRecord{ba40c5d u0 com.dcms.clientv3/.JobHandlerService}'

                }
                else
                {
                    // 通过前台 Service 提升应用权限
                    // 启动普通 Service , 但是在该 Service 的 onCreate 方法中执行了 startForeground
                    // 变成了前台 Service 服务

                    //启动本地服务
                    Intent localIntent = new Intent(application, typeof(LocalService));
                    application.StartService(localIntent);

                    //启动守护进程
                    //Intent guardIntent = new Intent(application, typeof(RemoteService));
                    //application.StartService(guardIntent);
                }
            }
        }

        /// <summary>
        /// 是否启用无声音乐 如不设置，则默认启用
        /// </summary>
        /// <param name="enable"></param>
        public static void UseSilenceMusice(bool enable)
        {
            KeepLive.useSilenceMusice = enable;
        }

        /// <summary>
        /// 前后台切换回调，用于处理app前后台切换，非必传
        /// </summary>
        /// <param name="cactusBackgroundCallback"></param>
        public void AddBackgroundCallback(ICactusBackgroundCallback cactusBackgroundCallback)
        {
            BACKGROUND_CALLBACKS.Add(cactusBackgroundCallback);
        }


        /// <summary>
        /// 判断是否主进程
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        private static bool IsMain(Application application)
        {
            int pid = Android.OS.Process.MyPid();
            string processName = "";
            ActivityManager mActivityManager = (ActivityManager)application.GetSystemService(Context.ActivityService);
            var runningAppProcessInfos = mActivityManager.RunningAppProcesses;
            if (runningAppProcessInfos != null)
            {
                foreach (var appProcess in runningAppProcessInfos)
                {
                    if (appProcess.Pid == pid)
                    {
                        processName = appProcess.ProcessName;
                        break;
                    }
                }
                string packageName = application.PackageName;
                if (processName.Equals(packageName))
                {
                    return true;
                }
            }
            return false;
        }

        //public static void FinishOnePix()
        //{
        //    if (mWeakReference != null)
        //    {
        //        mWeakReference.TryGetTarget(out OnePixelActivity activity);
        //        activity?.Finish();
        //    }
        //}

        ///// <summary>
        ///// 保存一像素，方便销毁
        ///// </summary>
        ///// <param name="activity"></param>
        //public static void SetOnePix(OnePixelActivity activity)
        //{
        //    if (mWeakReference == null)
        //    {
        //        mWeakReference = new WeakReference<OnePixelActivity>(activity);
        //    }
        //}


        /// <summary>
        /// 屏幕是否亮屏
        /// </summary>
        public static bool IsScreenOn(Context context)
        {
            try
            {
                var pm = context.GetSystemService(Context.PowerService) as PowerManager;
                return pm.IsScreenOn;
            }
            catch (Java.Lang.Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// 退到后台
        /// </summary>
        public static void BackBackground(Context context)
        {
            if (IsScreenOn(context))
            {
                var home = new Intent(Intent.ActionMain);
                home.AddFlags(ActivityFlags.NewTask);
                home.AddCategory(Intent.CategoryHome);
                context.StartActivity(home);
            }
        }


        ///// <summary>
        ///// 开启一像素界面
        ///// </summary>
        //public static void StartOnePixActivity(Context context)
        //{
        //    if (!IsScreenOn(context) && Build.VERSION.SdkInt < BuildVersionCodes.P)
        //    {
        //        //mIsForeground = isForeground;
        //        var onePixIntent = new Intent(context, typeof(OnePixelActivity));
        //        onePixIntent.AddFlags(ActivityFlags.SingleTop);
        //        onePixIntent.AddFlags(ActivityFlags.NewTask);
        //        var pendingIntent = PendingIntent.GetActivity(context, 0, onePixIntent, 0);

        //        try
        //        {
        //            pendingIntent.Send();
        //        }
        //        catch (Java.Lang.Exception ex)
        //        {
        //            ex.PrintStackTrace();
        //        }
        //    }
        //}

    }

    /// <summary>
    ///  Context 扩展
    /// </summary>
    public static class ContextExt
    {
        /// <summary>
        /// 注册 StopReceiver
        /// </summary>
        /// <param name="context"></param>
        /// <param name="block"></param>
        public static void RegisterStopReceiver(this Context context, Action block)
        {
            var st = new StopReceiver(context);
            st.Register(block);
        }

        public static bool IsCactusRunning(this Context context)
        {
            var l = ServiceUtils.isServiceRunning(context, typeof(LocalService).FullName);
            var r = ServiceUtils.isServiceRunning(context, typeof(RemoteService).FullName);
            return l && r;
        }
    }

    public class ForegroundNotificationClickListener : IForegroundNotificationClickListener
    {
        public void foregroundNotificationClick(Context context, Intent intent)
        {
            //System.Diagnostics.Debug.Print("通知点击了！");
        }
    }

    public class CactusBackgroundCallback : ICactusBackgroundCallback
    {
        public void onBackground(bool background)
        {
            Utils.ToastUtils.ShowSingleToast(background ? "转到到后台运行" : "恢复前台运行");
        }
    }

    public class KeepLiveService : IKeepLiveService
    {
        /// <summary>
        /// 服务终止 由于服务可能会被多次终止，该方法可能重复调用，需同onWorking配套使用，如注册和注销broadcast
        /// </summary>
        public void onStop()
        {
            System.Diagnostics.Debug.Print("服务停止！");
        }

        /// <summary>
        ///  运行中 由于服务可能会多次自动启动，该方法可能重复调用 
        /// </summary>
        public void onWorking()
        {
            System.Diagnostics.Debug.Print("服务开启！");
        }
    }
}