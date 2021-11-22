using Android.App;
using Android.Content;
using System;


namespace Wesley.Client.Droid.KeepLive.utils
{
    public class ServiceUtils
    {
        /// <summary>
        /// 判断服务是否在运行
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static bool isServiceRunning(Context ctx, String className)
        {
            bool isRunning = false;
            ActivityManager activityManager = (ActivityManager)ctx.GetSystemService(Context.ActivityService);
            var servicesList = activityManager.GetRunningServices(int.MaxValue);
            if (servicesList != null)
            {
                foreach(var si in servicesList)
                {
                    if (className.Equals(si.Service.ClassName))
                    {
                        isRunning = true;
                        break;
                    }
                }
            }
            return isRunning;
        }

        /// <summary>
        /// 判断任务是否在运行
        /// </summary>
        /// <param name="context"></param>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static bool isRunningTaskExist(Context context, String processName)
        {
            ActivityManager am = (ActivityManager)context.GetSystemService(Context.ActivityService);
            var processList = am.RunningAppProcesses;
            if (processList != null)
            {
                foreach (var info in processList)
                {
                    if (info.ProcessName.Equals(processName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}