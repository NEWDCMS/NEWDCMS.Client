using Android.Content;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using System.Collections.Generic;
using static Java.Lang.Thread;
using Process = Android.OS.Process;


namespace DCMS.Client.Droid
{

    /// <summary>
    /// 用于自定义崩溃处理（实验性：待提交到DCMS.logger里程碑)）
    /// </summary>
    public class CrashHandler : Java.Lang.Object, IUncaughtExceptionHandler
    {
        /// <summary>
        /// /系统默认的UncaughtException处理类
        /// </summary>
        private IUncaughtExceptionHandler mDefaultHandler;

        /// <summary>
        /// 表示一个CrashHandler实例
        /// </summary>
        private static CrashHandler Instance;

        /// <summary>
        /// App上下文
        /// </summary>
        //private Context mContext;

        /// <summary>
        /// 用来存储设备信息和异常信息
        /// </summary>
        //private readonly Dictionary<string, string> infos = new Dictionary<string, string>();

        private CrashHandler() { }

        /// <summary>
        /// 获取CrashHandler实例 ,单例模式
        /// </summary>
        /// <returns></returns>
        public static CrashHandler GetInstance()
        {
            if (Instance == null)
                Instance = new CrashHandler();
            return Instance;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        public void Init(Context context)
        {
            //mContext = context;
            //获取系统默认的UncaughtException处理器
            mDefaultHandler = Thread.DefaultUncaughtExceptionHandler;
            //设置该CrashHandler为程序的默认处理器
            Thread.DefaultUncaughtExceptionHandler = this;
        }

        /// <summary>
        /// 当UncaughtException发生时会转入该函数来处理
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="ex"></param>
        public void UncaughtException(Thread thread, Throwable ex)
        {
            if (!HandleException(ex) && mDefaultHandler != null)
            {
                //如果用户没有处理则让系统默认的异常处理器来处理
                mDefaultHandler.UncaughtException(thread, ex);
            }
            else
            {
                //new Thread(() =>
                //{
                //    Looper.Prepare();
                //    ToastUtils.ShowSingleToast($"很抱歉,程序出现异常,即将重启:{ex.Message}");
                //    Looper.Loop();
                //}).Start();

                ////展示的时间
                //Thread.Sleep(5000);

                //退出程序
                Process.KillProcess(Process.MyPid());
                JavaSystem.Exit(1);
            }
        }


        /// <summary>
        /// 自定义错误处理,收集错误信息 发送错误报告等操作均在此完成.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool HandleException(Throwable ex)
        {
            if (ex == null)
            {
                return false;
            }
            //收集设备参数信息
            //CollectDeviceInfo(mContext);
            //保存日志文件
            //SaveCrashInfo2File(ex);
            Crashes.TrackError(ex, new Dictionary<string, string> { { "Exception:", "CrashHandler" } });
            return true;
        }

        /*
        /// <summary>
        /// 收集设备参数信息
        /// </summary>
        /// <param name="ctx"></param>
        public void CollectDeviceInfo(Context ctx)
        {
            try
            {
                PackageManager pm = ctx.PackageManager;
                PackageInfo pi = pm.GetPackageInfo(ctx.PackageName, PackageInfoFlags.Activities);
                if (pi != null)
                {
                    string versionName = pi.VersionName ?? "null";
                    string versionCode = Android.Resource.Attribute.VersionCode + "";
                    infos.Add("versionName", versionName);
                    infos.Add("versionCode", versionCode);
                }
            }
            catch (NameNotFoundException)
            {

            }
            Field[] fields = Class.FromType(typeof(Build)).GetDeclaredFields();
            foreach (Field field in fields)
            {
                try
                {
                    field.Accessible = true;
                    infos.Add(field.Name, field.Get(null).ToString());
                }
                catch (Java.Lang.Exception)
                {
    
                }
            }
        }

        /// <summary>
        /// 保存错误信息到文件中
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private string SaveCrashInfo2File(Throwable ex)
        {

            StringBuffer sb = new StringBuffer();
            sb.Append("---------------------Begin--------------------------");
            foreach (var entry in infos)
            {
                string key = entry.Key;
                string value = entry.Value;
                sb.Append(key + "=" + value + "\n");
            }

            Writer writer = new StringWriter();
            PrintWriter printWriter = new PrintWriter(writer);
            ex.PrintStackTrace(printWriter);
            Throwable cause = ex.Cause;
            while (cause != null)
            {
                cause.PrintStackTrace(printWriter);
                cause = cause.Cause;
            }
            printWriter.Close();
            string result = writer.ToString();
            sb.Append(result);
            sb.Append("--------------------End---------------------------");

            SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd");
            string fileName = format.Format(new Date()) + ".log";
            File file = new File(Utils.FileUtils.CreateRootPath(mContext) + "/log/dcms_" + fileName);
            Utils.FileUtils.CreateFile(file);
            Utils.FileUtils.WriteFile(file.AbsolutePath, sb.ToString());

            return null;
        }

        */
    }
}