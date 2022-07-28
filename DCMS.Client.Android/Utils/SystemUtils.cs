using Android.App;
using Android.Content;

namespace DCMS.Client.Droid.Utils
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


        public static bool IsServiceExisted(Context context, string className)
        {
            try
            {
                ActivityManager am = (ActivityManager)context.GetSystemService(Context.ActivityService);
                var runningApps = am.RunningAppProcesses;
                if (!(runningApps.Count > 0))
                {
                    return false;
                }

                foreach (ActivityManager.RunningAppProcessInfo procInfo in runningApps)
                {
                    if (procInfo.ProcessName.ToLower().Equals(className.ToLower()))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Java.Lang.Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 获取当前进程名
        /// </summary>
        /// <param name="cxt"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static string GetProcessName(Context cxt, int pid)
        {
            try
            {
                ActivityManager am = (ActivityManager)cxt.GetSystemService(Context.ActivityService);
                var runningApps = am.RunningAppProcesses;
                if (runningApps == null)
                {
                    return null;
                }
                foreach (ActivityManager.RunningAppProcessInfo procInfo in runningApps)
                {
                    if (procInfo.Pid == pid)
                    {
                        return procInfo.ProcessName;
                    }
                }
                return null;
            }
            catch (Java.Lang.Exception)
            {
                return null;
            }
        }
    }
}