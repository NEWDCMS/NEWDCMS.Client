using Android.App;
using Android.Content;
using Android.OS;
using System.Collections.Generic;
using System.Linq;
using Uri = Android.Net.Uri;

namespace Wesley.Client.Droid
{
    public class IntentWrapper
    {
        //Android 7.0+ Doze 模式
        protected const int DOZE = 98;
        //华为 自启管理
        protected const int HUAWEI = 99;
        //华为 锁屏清理
        protected const int HUAWEI_GOD = 100;
        //小米 自启动管理
        protected const int XIAOMI = 101;
        //小米 神隐模式
        protected const int XIAOMI_GOD = 102;
        //三星 5.0/5.1 自启动应用程序管理
        protected const int SAMSUNG_L = 103;
        //魅族 自启动管理
        protected const int MEIZU = 104;
        //魅族 待机耗电管理
        protected const int MEIZU_GOD = 105;
        //Oppo 自启动管理
        protected const int OPPO = 106;
        //三星 6.0+ 未监视的应用程序管理
        protected const int SAMSUNG_M = 107;
        //Oppo 自启动管理(旧版本系统)
        protected const int OPPO_OLD = 108;
        //Vivo 后台高耗电
        protected const int VIVO_GOD = 109;
        //金立 应用自启
        protected const int GIONEE = 110;
        //乐视 自启动管理
        protected const int LETV = 111;
        //乐视 应用保护
        protected const int LETV_GOD = 112;
        //酷派 自启动管理
        protected const int COOLPAD = 113;
        //联想 后台管理
        protected const int LENOVO = 114;
        //联想 后台耗电优化
        protected const int LENOVO_GOD = 115;
        //中兴 自启管理
        protected const int ZTE = 116;
        //中兴 锁屏加速受保护应用
        protected const int ZTE_GOD = 117;

        protected static List<IntentWrapper> sIntentWrapperList;

        protected Intent intent;
        protected int type;

        protected IntentWrapper(Intent intent, int type)
        {
            this.intent = intent;
            this.type = type;
        }

        /// <summary>
        /// 防止华为机型未加入白名单时按返回键回到桌面再锁屏后几秒钟进程被杀
        /// </summary>
        /// <param name="a"></param>
        public static void OnBackPressed(Activity a)
        {
            Intent launcherIntent = new Intent(Intent.ActionMain);
            launcherIntent.AddCategory(Intent.CategoryHome);
            a.StartActivity(launcherIntent);
        }

        /// <summary>
        /// 获取Intent包装列表
        /// </summary>
        /// <returns></returns>
        public static List<IntentWrapper> GetIntentWrapperList()
        {
            if (sIntentWrapperList == null)
            {

                if (!MainApplication.Initialized)
                    return new List<IntentWrapper>();

                sIntentWrapperList = new List<IntentWrapper>();

                //Android 7.0+ Doze 模式
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    PowerManager pm = (PowerManager)MainActivity.Instance.GetSystemService(Context.PowerService);
                    bool ignoringBatteryOptimizations = pm.IsIgnoringBatteryOptimizations(MainActivity.Instance.PackageName);
                    if (!ignoringBatteryOptimizations)
                    {
                        Intent dozeIntent = new Intent(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
                        dozeIntent.SetData(Uri.Parse("package:" + MainActivity.Instance.PackageName));
                        sIntentWrapperList.Add(new IntentWrapper(dozeIntent, DOZE));
                    }
                }

                //华为 自启管理
                Intent huaweiIntent = new Intent();
                huaweiIntent.SetAction("huawei.intent.action.HSM_BOOTAPP_MANAGER");
                sIntentWrapperList.Add(new IntentWrapper(huaweiIntent, HUAWEI));

                //华为 锁屏清理
                Intent huaweiGodIntent = new Intent();
                huaweiGodIntent.SetComponent(new ComponentName("com.huawei.systemmanager", "com.huawei.systemmanager.optimize.process.ProtectActivity"));
                sIntentWrapperList.Add(new IntentWrapper(huaweiGodIntent, HUAWEI_GOD));

                //小米 自启动管理
                Intent xiaomiIntent = new Intent();
                xiaomiIntent.SetAction("miui.intent.action.OP_AUTO_START");
                xiaomiIntent.AddCategory(Intent.CategoryDefault);
                sIntentWrapperList.Add(new IntentWrapper(xiaomiIntent, XIAOMI));

                //小米 神隐模式
                Intent xiaomiGodIntent = new Intent();
                xiaomiGodIntent.SetComponent(new ComponentName("com.miui.powerkeeper", "com.miui.powerkeeper.ui.HiddenAppsConfigActivity"));
                xiaomiGodIntent.PutExtra("package_name", MainActivity.Instance.PackageName);
                xiaomiGodIntent.PutExtra("package_label", "经销商管家");
                sIntentWrapperList.Add(new IntentWrapper(xiaomiGodIntent, XIAOMI_GOD));

                //三星 5.0/5.1 自启动应用程序管理
                Intent samsungLIntent = MainActivity.Instance.PackageManager.GetLaunchIntentForPackage("com.samsung.android.sm");
                if (samsungLIntent != null) sIntentWrapperList.Add(new IntentWrapper(samsungLIntent, SAMSUNG_L));

                //三星 6.0+ 未监视的应用程序管理
                Intent samsungMIntent = new Intent();
                samsungMIntent.SetComponent(new ComponentName("com.samsung.android.sm_cn", "com.samsung.android.sm.ui.battery.BatteryActivity"));
                sIntentWrapperList.Add(new IntentWrapper(samsungMIntent, SAMSUNG_M));

                //魅族 自启动管理
                Intent meizuIntent = new Intent("com.meizu.safe.security.SHOW_APPSEC");
                meizuIntent.AddCategory(Intent.CategoryDefault);
                meizuIntent.PutExtra("packageName", MainActivity.Instance.PackageName);
                sIntentWrapperList.Add(new IntentWrapper(meizuIntent, MEIZU));

                //魅族 待机耗电管理
                Intent meizuGodIntent = new Intent();
                meizuGodIntent.SetComponent(new ComponentName("com.meizu.safe", "com.meizu.safe.powerui.PowerAppPermissionActivity"));
                sIntentWrapperList.Add(new IntentWrapper(meizuGodIntent, MEIZU_GOD));

                //Oppo 自启动管理
                Intent oppoIntent = new Intent();
                oppoIntent.SetComponent(new ComponentName("com.coloros.safecenter", "com.coloros.safecenter.permission.startup.StartupAppListActivity"));
                sIntentWrapperList.Add(new IntentWrapper(oppoIntent, OPPO));

                //Oppo 自启动管理(旧版本系统)
                Intent oppoOldIntent = new Intent();
                oppoOldIntent.SetComponent(new ComponentName("com.color.safecenter", "com.color.safecenter.permission.startup.StartupAppListActivity"));
                sIntentWrapperList.Add(new IntentWrapper(oppoOldIntent, OPPO_OLD));

                //Vivo 后台高耗电
                Intent vivoGodIntent = new Intent();
                vivoGodIntent.SetComponent(new ComponentName("com.vivo.abe", "com.vivo.applicationbehaviorengine.ui.ExcessivePowerManagerActivity"));
                sIntentWrapperList.Add(new IntentWrapper(vivoGodIntent, VIVO_GOD));

                //金立 应用自启
                Intent gioneeIntent = new Intent();
                gioneeIntent.SetComponent(new ComponentName("com.gionee.softmanager", "com.gionee.softmanager.MainActivity"));
                sIntentWrapperList.Add(new IntentWrapper(gioneeIntent, GIONEE));

                //乐视 自启动管理
                Intent letvIntent = new Intent();
                letvIntent.SetComponent(new ComponentName("com.letv.android.letvsafe", "com.letv.android.letvsafe.AutobootManageActivity"));
                sIntentWrapperList.Add(new IntentWrapper(letvIntent, LETV));

                //乐视 应用保护
                Intent letvGodIntent = new Intent();
                letvGodIntent.SetComponent(new ComponentName("com.letv.android.letvsafe", "com.letv.android.letvsafe.BackgroundAppManageActivity"));
                sIntentWrapperList.Add(new IntentWrapper(letvGodIntent, LETV_GOD));

                //酷派 自启动管理
                Intent coolpadIntent = new Intent();
                coolpadIntent.SetComponent(new ComponentName("com.yulong.android.security", "com.yulong.android.seccenter.tabbarmain"));
                sIntentWrapperList.Add(new IntentWrapper(coolpadIntent, COOLPAD));

                //联想 后台管理
                Intent lenovoIntent = new Intent();
                lenovoIntent.SetComponent(new ComponentName("com.lenovo.security", "com.lenovo.security.purebackground.PureBackgroundActivity"));
                sIntentWrapperList.Add(new IntentWrapper(lenovoIntent, LENOVO));

                //联想 后台耗电优化
                Intent lenovoGodIntent = new Intent();
                lenovoGodIntent.SetComponent(new ComponentName("com.lenovo.powersetting", "com.lenovo.powersetting.ui.Settings$HighPowerApplicationsActivity"));
                sIntentWrapperList.Add(new IntentWrapper(lenovoGodIntent, LENOVO_GOD));

                //中兴 自启管理
                Intent zteIntent = new Intent();
                zteIntent.SetComponent(new ComponentName("com.zte.heartyservice", "com.zte.heartyservice.autorun.AppAutoRunManager"));
                sIntentWrapperList.Add(new IntentWrapper(zteIntent, ZTE));

                //中兴 锁屏加速受保护应用
                Intent zteGodIntent = new Intent();
                zteGodIntent.SetComponent(new ComponentName("com.zte.heartyservice", "com.zte.heartyservice.setting.ClearAppSettingsActivity"));
                sIntentWrapperList.Add(new IntentWrapper(zteGodIntent, ZTE_GOD));
            }
            return sIntentWrapperList;
        }

        protected static string sApplicationName;
        public static string GetApplicationName()
        {
            if (sApplicationName == null)
            {
                if (!MainApplication.Initialized)
                    return "";

                var app = MainApplication.GetInstance();
                Android.Content.PM.PackageManager pm;
                Android.Content.PM.ApplicationInfo ai;

                try
                {
                    pm = app.PackageManager;
                    ai = pm.GetApplicationInfo(app.PackageName, 0);
                    sApplicationName = pm.GetApplicationLabel(ai).ToString();
                }
                catch (Android.Content.PM.PackageManager.NameNotFoundException e)
                {
                    e.PrintStackTrace();
                    sApplicationName = app.PackageName;
                }
            }
            return sApplicationName;
        }

        /// <summary>
        /// 判断本机上是否有能处理当前Intent的Activity
        /// </summary>
        /// <returns></returns>
        protected bool DoesActivityExists()
        {
            if (!MainApplication.Initialized)
                return false;

            var app = MainApplication.GetInstance();
            Android.Content.PM.PackageManager pm = app.PackageManager;
            var list = pm.QueryIntentActivities(intent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
            return list != null && list.Count() > 0;
        }

        /// <summary>
        /// 处理白名单
        /// </summary>
        /// <param name="Activity"></param>
        /// <param name=""></param>
        /// <param name="reason"></param>
        /// <returns>弹框IntentWrapper</returns>
        public static List<IntentWrapper> WhiteListMatters(Activity a, string reason)
        {
            List<IntentWrapper> showed = new List<IntentWrapper>();
            if (reason == null) reason = "核心服务的持续运行";
            List<IntentWrapper> intentWrapperList = GetIntentWrapperList();
            if (intentWrapperList != null && intentWrapperList.Any())
            {
                foreach (IntentWrapper iw in intentWrapperList)
                {
                    //如果本机上没有能处理这个Intent的Activity，说明不是对应的机型，直接忽略进入下一次循环。
                    if (!iw.DoesActivityExists()) continue;
                    switch (iw.type)
                    {
                        case DOZE:
                            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                            {
                                PowerManager pm = (PowerManager)a.GetSystemService(Context.PowerService);
                                if (pm.IsIgnoringBatteryOptimizations(a.PackageName)) break;
                                new AlertDialog.Builder(a)
                                        .SetCancelable(false)
                                        .SetTitle("需要忽略 " + GetApplicationName() + " 的电池优化")
                                        .SetMessage(reason + "需要 " + GetApplicationName() + " 加入到电池优化的忽略名单。\n\n" +
                                                "请点击『确定』，在弹出的『忽略电池优化』对话框中，选择『是』。")
                                        .SetPositiveButton("确定", (s, e) =>
                                        {
                                            iw.StartActivitySafely(a);
                                        }).Show();
                                showed.Add(iw);
                            }
                            break;
                        case HUAWEI:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle("需要允许 " + GetApplicationName() + " 自动启动")
                                    .SetMessage(reason + "需要允许 " + GetApplicationName() + " 的自动启动。\n\n" +
                                            "请点击『确定』，在弹出的『自启管理』中，将 " + GetApplicationName() + " 对应的开关打开。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                        case ZTE_GOD:
                        case HUAWEI_GOD:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle(GetApplicationName() + " 需要加入锁屏清理白名单")
                                    .SetMessage(reason + "需要 " + GetApplicationName() + " 加入到锁屏清理白名单。\n\n" +
                                            "请点击『确定』，在弹出的『锁屏清理』列表中，将 " + GetApplicationName() + " 对应的开关打开。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                        case XIAOMI_GOD:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle("需要关闭 " + GetApplicationName() + " 的神隐模式")
                                    .SetMessage(reason + "需要关闭 " + GetApplicationName() + " 的神隐模式。\n\n" +
                                            "请点击『确定』，在弹出的 " + GetApplicationName() + " 神隐模式设置中，选择『无限制』，然后选择『允许定位』。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                        case SAMSUNG_L:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle("需要允许 " + GetApplicationName() + " 的自启动")
                                    .SetMessage(reason + "需要 " + GetApplicationName() + " 在屏幕关闭时继续运行。\n\n" +
                                            "请点击『确定』，在弹出的『智能管理器』中，点击『内存』，选择『自启动应用程序』选项卡，将 " + GetApplicationName() + " 对应的开关打开。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                        case SAMSUNG_M:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle("需要允许 " + GetApplicationName() + " 的自启动")
                                    .SetMessage(reason + "需要 " + GetApplicationName() + " 在屏幕关闭时继续运行。\n\n" +
                                            "请点击『确定』，在弹出的『电池』页面中，点击『未监视的应用程序』->『添加应用程序』，勾选 " + GetApplicationName() + "，然后点击『完成』。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                        case MEIZU:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle("需要允许 " + GetApplicationName() + " 保持后台运行")
                                    .SetMessage(reason + "需要允许 " + GetApplicationName() + " 保持后台运行。\n\n" +
                                            "请点击『确定』，在弹出的应用信息界面中，将『后台管理』选项更改为『保持后台运行』。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                        case MEIZU_GOD:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle(GetApplicationName() + " 需要在待机时保持运行")
                                    .SetMessage(reason + "需要 " + GetApplicationName() + " 在待机时保持运行。\n\n" +
                                            "请点击『确定』，在弹出的『待机耗电管理』中，将 " + GetApplicationName() + " 对应的开关打开。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                        case ZTE:
                        case LETV:
                        case XIAOMI:
                        case OPPO:
                        case OPPO_OLD:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle("需要允许 " + GetApplicationName() + " 的自启动")
                                    .SetMessage(reason + "需要 " + GetApplicationName() + " 加入到自启动白名单。\n\n" +
                                            "请点击『确定』，在弹出的『自启动管理』中，将 " + GetApplicationName() + " 对应的开关打开。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                        case COOLPAD:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle("需要允许 " + GetApplicationName() + " 的自启动")
                                    .SetMessage(reason + "需要允许 " + GetApplicationName() + " 的自启动。\n\n" +
                                            "请点击『确定』，在弹出的『酷管家』中，找到『软件管理』->『自启动管理』，取消勾选 " + GetApplicationName() + "，将 " + GetApplicationName() + " 的状态改为『已允许』。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                        case VIVO_GOD:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle("需要允许 " + GetApplicationName() + " 的后台运行")
                                    .SetMessage(reason + "需要允许 " + GetApplicationName() + " 在后台高耗电时运行。\n\n" +
                                            "请点击『确定』，在弹出的『后台高耗电』中，将 " + GetApplicationName() + " 对应的开关打开。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                        case GIONEE:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle(GetApplicationName() + " 需要加入应用自启和绿色后台白名单")
                                    .SetMessage(reason + "需要允许 " + GetApplicationName() + " 的自启动和后台运行。\n\n" +
                                            "请点击『确定』，在弹出的『系统管家』中，分别找到『应用管理』->『应用自启』和『绿色后台』->『清理白名单』，将 " + GetApplicationName() + " 添加到白名单。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                        case LETV_GOD:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle("需要禁止 " + GetApplicationName() + " 被自动清理")
                                    .SetMessage(reason + "需要禁止 " + GetApplicationName() + " 被自动清理。\n\n" +
                                            "请点击『确定』，在弹出的『应用保护』中，将 " + GetApplicationName() + " 对应的开关关闭。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                        case LENOVO:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle("需要允许 " + GetApplicationName() + " 的后台运行")
                                    .SetMessage(reason + "需要允许 " + GetApplicationName() + " 的后台自启、后台 GPS 和后台运行。\n\n" +
                                            "请点击『确定』，在弹出的『后台管理』中，分别找到『后台自启』、『后台 GPS』和『后台运行』，将 " + GetApplicationName() + " 对应的开关打开。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                        case LENOVO_GOD:
                            new AlertDialog.Builder(a)
                                    .SetCancelable(false)
                                    .SetTitle("需要关闭 " + GetApplicationName() + " 的后台耗电优化")
                                    .SetMessage(reason + "需要关闭 " + GetApplicationName() + " 的后台耗电优化。\n\n" +
                                            "请点击『确定』，在弹出的『后台耗电优化』中，将 " + GetApplicationName() + " 对应的开关关闭。")
                                    .SetPositiveButton("确定", (s, e) =>
                                    {
                                        iw.StartActivitySafely(a);
                                    }).Show();
                            showed.Add(iw);
                            break;
                    }
                }
            }
            return showed;
        }

        /// <summary>
        /// 安全地启动一个Activity
        /// </summary>
        /// <param name="activityContext"></param>
        protected void StartActivitySafely(Activity activityContext)
        {
            try { activityContext.StartActivity(intent); } catch (Java.Lang.Exception e) { e.PrintStackTrace(); }
        }
    }
}