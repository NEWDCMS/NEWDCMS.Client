using Android.App;
using Android.Content;

namespace Wesley.Client.Droid.Utils
{
    public class SystemUtils
    {
        /// <summary>
        /// 判断本应用是否存活
        /// 如果需要判断本应用是否在后台还是前台用getRunningTask
        /// </summary>
        /// <param name="mContext"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public static bool IsAPPALive(Context mContext, string packageName)
        {
            bool isAPPRunning = false;
            // 获取activity管理对象
            ActivityManager activityManager = (ActivityManager)mContext.GetSystemService(Context.ActivityService);
            // 获取所有正在运行的app
            var appProcessInfoList = activityManager.RunningAppProcesses;
            if (appProcessInfoList != null)
            {
                // 遍历，进程名即包名
                foreach (ActivityManager.RunningAppProcessInfo appInfo in appProcessInfoList)
                {
                    if (packageName.Equals(appInfo.ProcessName))
                    {
                        isAPPRunning = true;
                        break;
                    }
                }
            }
            return isAPPRunning;
        }


        public static bool IsServiceRunning(Context context, string serviceName)
        {
            bool isAPPRunning = false;
            try
            {
                if (string.IsNullOrEmpty(serviceName))
                {
                    return isAPPRunning;
                }
                var myManager = (ActivityManager)context.GetSystemService(Context.ActivityService);
#pragma warning disable CS0618 // 类型或成员已过时
                var runningService = myManager.GetRunningServices(50);
#pragma warning restore CS0618 // 类型或成员已过时
                foreach (ActivityManager.RunningServiceInfo sInfo in runningService)
                {
                    if (sInfo.Service.ClassName.IndexOf(serviceName) >= 0)
                    {
                        isAPPRunning = true;
                        break;
                    }
                }
            }
            catch (Java.Lang.Exception) { }

            return isAPPRunning;
        }

        public static string GetAppProcessName(Context context)
        {
            //当前应用pid
            int pid = Android.OS.Process.MyPid();
            //任务管理类
            ActivityManager activityManager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            //遍历所有应用
            var appProcessInfoList = activityManager.RunningAppProcesses;
            if (appProcessInfoList != null)
            {
                foreach (ActivityManager.RunningAppProcessInfo appInfo in appProcessInfoList)
                {
                    if (appInfo.Pid == pid)//得到当前应用
                        return appInfo.ProcessName;//返回包名
                }
            }
            return "";
        }

        public static ActivityManager.RunningAppProcessInfo GetRunningAppProcessInfo(Context context)
        {
            //当前应用pid
            int pid = Android.OS.Process.MyPid();
            //任务管理类
            ActivityManager activityManager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            //遍历所有应用
            var appProcessInfoList = activityManager.RunningAppProcesses;
            if (appProcessInfoList != null)
            {
                foreach (ActivityManager.RunningAppProcessInfo appInfo in appProcessInfoList)
                {
                    if (appInfo.Pid == pid)//得到当前应用
                        return appInfo;
                }
            }
            return null;
        }


        public static string GetProcessName(Context context)
        {
            int pid = Android.OS.Process.MyPid();
            var mActivityManager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            var processes = mActivityManager.RunningAppProcesses;
            if (processes != null)
            {
                foreach (ActivityManager.RunningAppProcessInfo appProcess in processes)
                {
                    if (appProcess.Pid == pid)
                    {
                        return appProcess.ProcessName;
                    }
                }
            }
            return null;
        }

        //public static string GetProtectPackageName()
        //{
        //    return SharePrefs.Get().GetString(Contants.KEY_PACKAGE_NAME);
        //}

    }
}