using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xcrash;
using Log = System.Diagnostics;
using Process = Android.OS.Process;
using Microsoft.AppCenter.Crashes;

namespace DCMS.Client.Droid
{
    /// <summary>
    /// 前后台切换回调
    /// </summary>
    public interface ICactusBackgroundCallback
    {
        void onBackground(bool background);
    }

    /// <summary>
    /// 允许申请最大内存空间
    /// </summary>
    [Application(LargeHeap = true)]
    public class MainApplication : Android.App.Application
    {
        public static bool IsFinishAll { get; set; }
        public static bool Initialized;
        private static MainApplication _context;
        /// <summary>
        /// 前后台回调集合
        /// </summary>
        public static List<ICactusBackgroundCallback> BACKGROUND_CALLBACKS = new List<ICactusBackgroundCallback>();


        /// <summary>
        /// 前后台切换监听
        /// </summary>
        private AppBackgroundCallback? mAppBackgroundCallback = null;

        public static MainApplication GetInstance()
        {
            return _context;
        }

        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialized = true;
            _context = this;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            try
            {
                var access = e.NetworkAccess;
                switch (access)
                {
                    case NetworkAccess.Internet:
                        Utils.ToastUtils.ShowSingleToast("网络已连接");
                        break;
                    case NetworkAccess.ConstrainedInternet:
                        Utils.ToastUtils.ShowSingleToast("限制访问互联网");
                        break;
                    case NetworkAccess.Local:
                        Utils.ToastUtils.ShowSingleToast("仅限本地网络访问");
                        break;
                    case NetworkAccess.None:
                        Utils.ToastUtils.ShowSingleToast("无可用连接");
                        break;
                    case NetworkAccess.Unknown:
                        Utils.ToastUtils.ShowSingleToast("无法确定互联网连接");
                        break;
                }
            }
            catch (System.Exception) { }
        }

        public override void OnCreate()
        {
            //OnCreate前不能有调用
            base.OnCreate();

            //Essentials
            Platform.Init(this);

            //初始 Akavache 数据库
            Akavache.Registrations.Start(this.PackageName);

            //百度地图初始
            FormsBaiduMaps.Init(this);

            #region KeepLive

            //后台回调
            BACKGROUND_CALLBACKS.Add(new CactusBackgroundCallback());

            //注册生命周期回调
            if (this is Android.App.Application && mAppBackgroundCallback == null)
            {
                var mAppBackgroundCallback = new AppBackgroundCallback(this);
                RegisterActivityLifecycleCallbacks(mAppBackgroundCallback);
            }

            #endregion
        }


        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Log.Debug.Print(e.Exception.Message);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Exception ex = (System.Exception)e.ExceptionObject;
            Log.Debug.Print(ex.Message);
        }

        private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            UnhandledExceptionHandler(e.Exception, e);
        }

        /// <summary>
        /// 处理未处理异常
        /// </summary>
        /// <param name="e"></param>
        private void UnhandledExceptionHandler(System.Exception ex, RaiseThrowableEventArgs e)
        {
            Crashes.TrackError(ex);
            e.Handled = true;
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            //卸载生命周期回调
            UnregisterActivityLifecycleCallbacks(mAppBackgroundCallback);
        }
    }


    public class CactusBackgroundCallback : ICactusBackgroundCallback
    {
        public void onBackground(bool background)
        {
            Utils.ToastUtils.ShowSingleToast(background ? "转到到后台运行" : "恢复前台运行");
        }
    }


    /// <summary>
    /// 生命周期回调
    /// </summary>
    public class AppBackgroundCallback : Java.Lang.Object, Android.App.Application.IActivityLifecycleCallbacks
    {
        private Handler mHander;
        /// <summary>
        /// 前台Activity数量
        /// </summary>
        private int mFrontActivityCount = 0;
        /// <summary>
        ///  当Activity数量大于0的时候，标识是否已经发出前后台广播
        /// </summary>
        private bool mIsSend = false;

        private static WeakReference<Context>? mContext;

        private const long FIRST_TIME = 1000L;

        public AppBackgroundCallback(Context? context)
        {
            //mContext = context;
            mHander = new Handler(Looper.MainLooper);
            Init();
        }

        private void Init()
        {
            mHander.PostDelayed(() =>
            {
                if (mFrontActivityCount == 0)
                {
                    Post();
                }

            }, FIRST_TIME);
        }


        #region 生命周期


        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            mContext = new WeakReference<Context>(activity);
        }
        public void OnActivityDestroyed(Activity activity) { }
        public void OnActivityPaused(Activity activity) { }
        public void OnActivityResumed(Activity activity) { }
        public void OnActivitySaveInstanceState(Activity activity, Bundle outState) { }
        public void OnActivityStarted(Activity activity)
        {
            mFrontActivityCount++;
            Post();
        }
        public void OnActivityStopped(Activity activity)
        {
            mFrontActivityCount--;
            Post();
        }

        /// <summary>
        /// 前后台切换处理广播
        /// </summary>
        private void Post()
        {
            if (mContext == null) return;

            mContext.TryGetTarget(out Context context);
            if (context != null)
            {
                if (mFrontActivityCount == 0)
                {
                    mIsSend = false;
                    mHander.PostDelayed(() =>
                    {
                        var intent = new Intent();
                        intent.SetAction("_ACTION_BACKGROUND");
                        context.SendBroadcast(intent);
                    }, 0);
                }
                else
                {
                    if (!mIsSend)
                    {
                        mIsSend = true;
                        mHander.PostDelayed(() =>
                        {
                            var intent = new Intent();
                            intent.SetAction("_ACTION_FOREGROUND");
                            context.SendBroadcast(intent);
                        }, 0);
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Activity管理类 
    /// </summary>
    public class AppManager
    {
        private static AppManager _appManager = null;
        static AppManager()
        {
            _appManager = new AppManager();
        }
        public static AppManager Instance()
        {
            return _appManager;
        }

        private List<Activity> _activities = new List<Activity>();

        /// <summary>
        /// 增加Activity
        /// </summary>
        /// <param name="activity"></param>
        public void AddActivity(Activity activity)
        {
            _activities.Add(activity);
        }

        public void RemoveActivity(Activity activity)
        {
            activity.FinishAffinity();
            _activities.Remove(activity);
        }

        public void RemoveAllActivity()
        {
            foreach (var ac in _activities)
            {
                ac.FinishAffinity();
            }
            _activities.Clear();
        }

        /// <summary>
        /// 获得最顶部的Activity
        /// </summary>
        /// <returns></returns>
        public Activity GetTopActivity()
        {
            return _activities.FirstOrDefault();
        }

        /// <summary>
        /// 是否有某个Activity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HasActivity<T>(T t) where T : Activity
        {
            foreach (var it in _activities)
            {
                if (t.LocalClassName == it.LocalClassName)
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr2)
                    {
                        return !(it.IsDestroyed || it.IsFinishing);
                    }
                    else
                    {
                        return !it.IsFinishing;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 退出APP
        /// </summary>
        public void ExitApp()
        {
            RemoveAllActivity();
            Android.OS.Process.KillProcess(Process.MyPid());
            Xamarin.Forms.Application.Current.Quit();
        }
    }

    /// <summary>
    /// Activity基类
    /// </summary>
    public class BaseActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void AttachBaseContext(Context @base)
        {
            base.AttachBaseContext(@base);

            //xCrash拦截崩溃
            XCrash.Init(@base);
        }

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

                AppManager.Instance().AddActivity(this);
            }
            catch (Java.Lang.Exception ex)
            { }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var newExc = new System.Exception("TaskSchedulerOnUnobservedTaskException", e.Exception);
            LogUnhandledException(newExc);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var newExc = new System.Exception("CurrentDomainOnUnhandledException", e.ExceptionObject as System.Exception);
            LogUnhandledException(newExc);
        }

        internal static void LogUnhandledException(System.Exception exception)
        {
            try
            {
                Android.Util.Log.Error("Crash Report", exception.ToString());
            }
            catch
            {
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            try
            {
                AppManager.Instance().RemoveActivity(this);
            }
            catch (Java.Lang.Exception ex)
            { }
        }
    }
}