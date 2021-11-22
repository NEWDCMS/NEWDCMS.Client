using Android.App;
using Android.Content;
using Android.OS;
using System.Collections.Generic;


namespace DCMS.Client.Droid
{

    public class AppMonitor
    {
        /// <summary>
        /// 注册了的监听器
        /// </summary>
        private List<ICallback> mCallbacks;

        /// <summary>
        /// 是否初始化了
        /// </summary>
        private bool isInited;

        /// <summary>
        /// 活跃Activity的数量
        /// </summary>
        public int mActiveActivityCount { get; set; } = 0;

        /// <summary>
        /// 存活的Activity数量
        /// </summary>
        public int mAliveActivityCount { get; set; } = 0;

        public static AppMonitor Get()
        {
            return SingleHolder.INSTANCE;
        }

        private static class SingleHolder
        {
            public static readonly AppMonitor INSTANCE = new AppMonitor();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(Context context)
        {
            if (isInited)
            {
                return;
            }
            mCallbacks = new List<ICallback>();
            RegisterLifecycle(context);
            isInited = true;
        }

        /// <summary>
        /// 注册生命周期
        /// </summary>
        /// <param name="context"></param>
        private void RegisterLifecycle(Context context)
        {
            Application application = (Application)context.ApplicationContext;
            application.RegisterActivityLifecycleCallbacks(new ActivityLifecycleCallbacks(Get()));
        }


        /// <summary>
        /// 判断App是否活着
        /// </summary>
        /// <returns></returns>
        public bool IsAppAlive()
        {
            return mAliveActivityCount > 0;
        }

        /// <summary>
        /// 判断App是否在前台
        /// </summary>
        /// <returns></returns>
        public bool IsAppForeground()
        {
            return mActiveActivityCount > 0;
        }

        /// <summary>
        /// 判断App是否在后台
        /// </summary>
        /// <returns></returns>
        public bool IsAppBackground()
        {
            return mActiveActivityCount <= 0;
        }

        /// <summary>
        /// 通知监听者
        /// </summary>
        public void NotifyChange()
        {
            if (mActiveActivityCount > 0)
            {
                foreach (var callback in mCallbacks)
                {
                    callback.OnAppForeground();
                }
            }
            else
            {
                foreach (var callback in mCallbacks)
                {
                    callback.OnAppBackground();
                }
            }
        }

        /// <summary>
        /// 通知监听者界面销毁
        /// </summary>
        public void NotifyAppAliveChange()
        {
            if (mAliveActivityCount == 0)
            {
                foreach (var callback in mCallbacks)
                {
                    callback.OnAppUIDestroyed();
                }
            }
        }

        public interface ICallback
        {
            /**
             * 当App切换到前台时回调
             */
            void OnAppForeground();

            /**
             * App切换到后台时回调
             */
            void OnAppBackground();

            /**
             * App所有界面都销毁了
             */
            void OnAppUIDestroyed();
        }


        public class CallbackAdapter : ICallback
        {
            public void OnAppForeground()
            {
                //App切换到前台
            }
            public void OnAppBackground()
            {
                //App切换到后台
            }

            public void OnAppUIDestroyed()
            {
                //App被杀死了
            }
        }

        /// <summary>
        /// 注册回调
        /// </summary>
        /// <param name="callback"></param>
        public void Register(ICallback callback)
        {
            if (mCallbacks.Contains(callback))
            {
                return;
            }
            mCallbacks.Add(callback);
        }

        /// <summary>
        /// 注销回调
        /// </summary>
        /// <param name="callback"></param>
        public void UnRegister(ICallback callback)
        {
            if (!mCallbacks.Contains(callback))
            {
                return;
            }
            mCallbacks.Remove(callback);
        }
    }


    public class ActivityLifecycleCallbacks : Java.Lang.Object, Application.IActivityLifecycleCallbacks
    {
        AppMonitor appMonitor;
        public ActivityLifecycleCallbacks(AppMonitor app)
        {
            appMonitor = app;
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            appMonitor.mAliveActivityCount++;
        }

        public void OnActivityDestroyed(Activity activity)
        {
            appMonitor.mAliveActivityCount--;
            appMonitor.NotifyAppAliveChange();
        }

        public void OnActivityPaused(Activity activity)
        {
        
        }

        public void OnActivityResumed(Activity activity)
        {
 
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
 
        }

        public void OnActivityStarted(Activity activity)
        {
            appMonitor.mActiveActivityCount++;
            appMonitor.NotifyChange();
        }

        public void OnActivityStopped(Activity activity)
        {
            appMonitor.mActiveActivityCount--;
            appMonitor.NotifyChange();
        }
    }
}
