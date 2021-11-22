using Android.Content;
using Android.Content.Res;
using Android.OS;
using Java.Lang;


namespace DCMS.Client.Droid.Utils
{
    /// <summary>
    /// （实验性：待提交到DCMS.logger里程碑)）
    /// </summary>
    public class AppUtils
    {
        private static Context mContext;
        private static Thread mUiThread;
        private static readonly Handler sHandler = new Handler(Looper.MainLooper);

        public static void Init(Context context)
        {
            mContext = context;
            mUiThread = Thread.CurrentThread();
        }

        public static Context GetAppContext()
        {
            return mContext;
        }

        public static AssetManager GetAssets()
        {
            return mContext.Assets;
        }

        public static Android.Content.Res.Resources GetResource()
        {
            return mContext.Resources;
        }

        public static bool IsUIThread()
        {
            return Thread.CurrentThread() == mUiThread;
        }

        public static void RunOnUI(Runnable r)
        {
            sHandler.Post(r);
        }

        public static void RunOnUIDelayed(Runnable r, long delayMills)
        {
            sHandler.PostDelayed(r, delayMills);
        }

        public static void RemoveRunnable(Runnable r)
        {
            if (r == null)
            {
                sHandler.RemoveCallbacksAndMessages(null);
            }
            else
            {
                sHandler.RemoveCallbacks(r);
            }
        }
    }
}