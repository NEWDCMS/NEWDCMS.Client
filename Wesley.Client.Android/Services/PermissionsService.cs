using Android;
using Android.Annotation;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Annotations;
using AndroidX.Core.Content;
using Wesley.Client.Droid.Utils;
using Wesley.Client.Services;
using Google.Android.Material.Snackbar;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;


using Wesley.Client.Droid.Services;
[assembly: Xamarin.Forms.Dependency(typeof(PermissionsService))]
namespace Wesley.Client.Droid.Services
{
    public class PermissionsService : IPermissionsService
    {
        private readonly string[] permissions = { Manifest.Permission.RequestIgnoreBatteryOptimizations };
        private TaskCompletionSource<bool> tcsPermissions;

        /// <summary>
        /// 后台运行设置
        /// </summary>
        public void BackgroundOperationSetting()
        {
            if (IsHuawei())
            {
                GoHuaweiSetting();
            }
            else if (IsXiaomi())
            {
                GoXiaomiSetting();
            }
            else if (IsOPPO())
            {
                GoOPPOSetting();
            }
            else if (IsVIVO())
            {
                GoVIVOSetting();
            }
            else if (IsMeizu())
            {
                GoMeizuSetting();
            }
            else if (IsSamsung())
            {
                GoSamsungSetting();
            }
            else 
            {
                ToastUtils.ShowSingleToast("没有检测到机型！");
            }
        }

        /// <summary>
        /// 电池优化设置
        /// </summary>
        public void BatteryOptimizationSetting()
        {
            var isIgnoring = IsIgnoringBatteryOptimizations();
            if (!isIgnoring)
            {
                EnableBackgroundServicesDialogue();
            }
            else 
            {
                ToastUtils.ShowSingleToast("已经加入到白名单！");
            }
        }
        public Task<bool> GetPermissionsAsync()
        {
            tcsPermissions = new TaskCompletionSource<bool>();

            if ((int)Build.VERSION.SdkInt < 23)
            {
                tcsPermissions.TrySetResult(true);
            }
            else
            {
                if (ContextCompat.CheckSelfPermission(MainActivity.Instance, Manifest.Permission.RequestIgnoreBatteryOptimizations) != (int)Permission.Granted)
                {
                    RequestMicPermission();
                }
                else
                {
                    tcsPermissions.TrySetResult(true);
                }
            }

            return tcsPermissions.Task;
        }
        private void RequestMicPermission()
        {
            var currentActivity = MainActivity.Instance;
            if (currentActivity.ShouldShowRequestPermissionRationale(Manifest.Permission.RequestIgnoreBatteryOptimizations))
            {
                Snackbar.Make(currentActivity.FindViewById(Android.Resource.Id.Content),
                    "App requires IgnoreBattery permission.",
                    Snackbar.LengthIndefinite).SetAction("Ok",
                    v =>
                    {
                        currentActivity.RequestPermissions(permissions, 1);
                    }).Show();
            }
            else
            {
                currentActivity.RequestPermissions(permissions, 1);
            }
        }
        public void OnRequestPermissionsResult(bool isGranted)
        {
            tcsPermissions.TrySetResult(isGranted);
        }


        /// <summary>
        /// 判断应用是否在白名单中
        /// </summary>
        /// <returns></returns>
        [TargetApi(Value = 23)]
        private bool IsIgnoringBatteryOptimizations()
        {
            bool isIgnoring = false;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var powerManager = (PowerManager)MainApplication.Context.GetSystemService(Context.PowerService);
                if (powerManager != null)
                {
                    var packageName = SystemUtils.GetAppProcessName(MainApplication.Context);
                    isIgnoring = powerManager.IsIgnoringBatteryOptimizations(packageName);
                }
            }
            return isIgnoring;
        }


        /// <summary>
        /// 申请加入白名单
        /// </summary>
        [TargetApi(Value = 23)]
        private void RequestIgnoreBatteryOptimizations()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    var packageName = SystemUtils.GetAppProcessName(MainApplication.Context);
                    Intent intent = new Intent();
                    intent.SetAction(Android.Provider.Settings.ActionIgnoreBatteryOptimizationSettings);
                    intent.SetData(Android.Net.Uri.Parse("package:" + packageName));
                    intent.SetFlags(ActivityFlags.NewTask);
                    MainApplication.Context.StartActivity(intent);
                }
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
        }

        /// <summary>
        /// 申请加入白名单
        /// </summary>
        private void EnableBackgroundServicesDialogue()
        {
            try
            {
                var packageName = SystemUtils.GetAppProcessName(MainApplication.Context);
                var myIntent = PrepareIntentForWhiteListingOfBatteryOptimization(packageName);
                if (myIntent != null)
                {
                    myIntent.SetFlags(ActivityFlags.NewTask);
                    MainApplication.Context.StartActivity(myIntent);
                }
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
        }

        public enum WIBO
        {
            WHITE_LISTED, NOT_WHITE_LISTED, ERROR_GETTING_STATE, IRRELEVANT_OLD_ANDROID_API
        }

        public WIBO GetIfAppIsWhiteListedFromBatteryOptimizations(string packageName)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
            {
                return WIBO.IRRELEVANT_OLD_ANDROID_API;
            }

            var powerManager = (PowerManager)MainApplication.Context.GetSystemService(Context.PowerService);
            if (powerManager == null)
            {
                return WIBO.ERROR_GETTING_STATE;
            }
            else
            {
                if (powerManager.IsIgnoringBatteryOptimizations(packageName))
                {
                    return WIBO.WHITE_LISTED;
                }
                else
                {
                    return WIBO.NOT_WHITE_LISTED;
                }
            }
        }

        [TargetApi(Value = 23)]
        [SuppressLint(Value = new string[] { "BatteryLife", "InlinedApi" })]
        [RequiresPermission(Value = Manifest.Permission.RequestIgnoreBatteryOptimizations)]
        public Intent PrepareIntentForWhiteListingOfBatteryOptimization(string packageName, bool alsoWhenWhiteListed = false)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
                return null;

            if (ContextCompat.CheckSelfPermission(MainApplication.Context, Manifest.Permission.RequestIgnoreBatteryOptimizations) == Permission.Denied)
                return null;

            var appIsWhiteListedFromPowerSave = GetIfAppIsWhiteListedFromBatteryOptimizations(packageName);

            Intent intent = null;

            switch (appIsWhiteListedFromPowerSave)
            {
                case WIBO.WHITE_LISTED:
                    if (alsoWhenWhiteListed)
                    {
                        intent = new Intent(Android.Provider.Settings.ActionIgnoreBatteryOptimizationSettings);
                    }
                    break;
                case WIBO.NOT_WHITE_LISTED:
                    {
                        intent = new Intent(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
                        intent.SetData(Android.Net.Uri.Parse("package:" + packageName));
                    }
                    break;
                case WIBO.ERROR_GETTING_STATE:
                case WIBO.IRRELEVANT_OLD_ANDROID_API:
                    break;
                default:
                    break;
            }

            return intent;

        }


        /// <summary>
        /// 跳转到指定应用的首页
        /// </summary>
        /// <param name="string"></param>
        /// <param name=""></param>
        private void ShowActivity(string packageName)
        {
            //var packageName = SystemUtils.GetAppProcessName(MainApplication.Context);
            Intent intent = MainApplication.Context.PackageManager.GetLaunchIntentForPackage(packageName);
            MainApplication.Context.StartActivity(intent);
        }

        /// <summary>
        /// 跳转到指定应用的指定页面
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="activityDir"></param>
        private void ShowActivity(string packageName, string activityDir)
        {
            Intent intent = new Intent();
            intent.SetComponent(new ComponentName(packageName, activityDir));
            intent.AddFlags(ActivityFlags.NewTask);
            MainApplication.Context.StartActivity(intent);
        }

        /// <summary>
        /// 华为
        /// </summary>
        /// <returns></returns>
        public bool IsHuawei()
        {
            if (Build.Brand == null)
            {
                return false;
            }
            else
            {
                return Build.Brand.ToLower().Equals("huawei") || Build.Brand.ToLower().Equals("honor");
            }
        }
        private void GoHuaweiSetting()
        {
            try
            {
                ShowActivity("com.huawei.systemmanager", "com.huawei.systemmanager.startupmgr.ui.StartupNormalAppListActivity");
            }
            catch (Exception)
            {
                ShowActivity("com.huawei.systemmanager", "com.huawei.systemmanager.optimize.bootstart.BootStartActivity");
            }
        }


        /// <summary>
        /// 小米
        /// </summary>
        /// <returns></returns>
        public static bool IsXiaomi()
        {
            return Build.Brand != null && Build.Brand.ToLower().Equals("xiaomi");
        }
        private void GoXiaomiSetting()
        {
            try
            {
                ShowActivity("com.miui.securitycenter", "com.miui.permcenter.autostart.AutoStartManagementActivity");
            }
            catch (Exception)
            {
            }
        }


        /// <summary>
        /// OPPO
        /// </summary>
        /// <returns></returns>
        public static bool IsOPPO()
        {
            return Build.Brand != null && Build.Brand.ToLower().Equals("oppo");
        }
        private void GoOPPOSetting()
        {
            try
            {
                ShowActivity("com.coloros.phonemanager");
            }
            catch (Exception)
            {
                try
                {
                    ShowActivity("com.oppo.safe");
                }
                catch (Exception)
                {
                    try
                    {
                        ShowActivity("com.coloros.oppoguardelf");
                    }
                    catch (Exception)
                    {
                        ShowActivity("com.coloros.safecenter");
                    }
                }
            }
        }

        /// <summary>
        /// VIVO
        /// </summary>
        /// <returns></returns>
        public static bool IsVIVO()
        {
            return Build.Brand != null && Build.Brand.ToLower().Equals("vivo");

        }
        private void GoVIVOSetting()
        {
            try
            {
                ShowActivity("com.iqoo.secure");
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Meizu
        /// </summary>
        /// <returns></returns>
        public static bool IsMeizu()
        {
            return Build.Brand != null && Build.Brand.ToLower().Equals("meizu");
        }
        private void GoMeizuSetting()
        {
            try
            {
                ShowActivity("com.meizu.safe");
            }
            catch (Exception)
            {
            }
        }


        /// <summary>
        /// Samsung
        /// </summary>
        /// <returns></returns>
        public static bool IsSamsung()
        {
            return Build.Brand != null && Build.Brand.ToLower().Equals("samsung");
        }
        private void GoSamsungSetting()
        {
            try
            {
                ShowActivity("com.samsung.android.sm_cn");
            }
            catch (Exception)
            {
                ShowActivity("com.samsung.android.sm");
            }
        }

        public void RequestLocationAndCameraPermission()
        {
            try
            {
                var currentActivity = MainActivity.Instance;
                var isSpecialDevice = Build.VERSION.SdkInt < BuildVersionCodes.M || Build.Brand.ToLower().Equals("smartisan") || Build.Brand.ToLower().Equals("xiaomi") || Build.Brand.ToLower().Equals("oppo") || Build.Brand.ToLower().Equals("vivo") || Build.Brand.ToLower().Equals("lenovo") || Build.Brand.ToLower().Equals("meizu");
                if (ContextCompat.CheckSelfPermission(currentActivity, Manifest.Permission.AccessFineLocation) != (int)Permission.Granted)
                {
                    if (isSpecialDevice)
                    {
                        currentActivity.RequestPermissions(new string[] {
                            Manifest.Permission.Camera,
                            Manifest.Permission.AccessFineLocation,
                            Manifest.Permission.AccessCoarseLocation
                        }, 0);
                    }
                    else
                    {
                        currentActivity.RequestPermissions(new string[]
                        {
                            Manifest.Permission.AccessFineLocation,
                            Manifest.Permission.AccessCoarseLocation
                        }, 0);
                    }
                }

            }
            catch (Java.Lang.Exception ex)
            {
                Android.Util.Log.Error("OnRequestPermissionsResult", ex.Message);
            }
        }

        public async Task<PermissionStatus> GetLocationConsent()
        {
            return await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
    }
}