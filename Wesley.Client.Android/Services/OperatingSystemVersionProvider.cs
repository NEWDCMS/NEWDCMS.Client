using Android.App;
using Android.OS;
using Wesley.Client.AutoUpdater.Services;
using Wesley.Client.CustomViews;
using Wesley.Client.Droid.AutoUpdater;
using Java.Lang;
using MySettings = Android.Provider.Settings;

namespace Wesley.Client.Droid.Services
{
    /// <summary>
    /// 版本
    /// </summary>
    public class OperatingSystemVersionProvider : IOperatingSystemVersionProvider
    {
        private readonly UpdateUtils updateUtils;
        public OperatingSystemVersionProvider()
        {
            updateUtils = new UpdateUtils();
        }

        public string GetOperatingSystemVersionString()
        {
            return Build.VERSION.Release;
        }

        public void Check(UpdateInfo updateInfo)
        {
            updateUtils.Update(AutoUpdate.Context, updateInfo);
        }

        public void StarInit()
        {
            updateUtils.StarInit();
        }


        public async void CheckUpdate(UpdateInfo updateInfo)
        {
            //版本检查
            var cc = updateUtils.CheckNewVersion(AutoUpdate.Context, updateInfo);
            if (cc)
            {
                var result = await CrossDiaglogKit.Current.GetUpgradeResultAsync("系统升级提示:", $"当前版本:{UpdateUtils.GetVersion(AutoUpdate.Context)}, 发现新版本:{updateInfo.Version}, 是否更新?");
                if (result)
                {
                    //更新
                    updateUtils.StarNewVersionUpdateV2(AutoUpdate.Context, updateInfo);
                    Settings.IsNextTimeUpdate = false;
                }
                else
                {
                    //退出Activity
                    //ActivityCollector.FinishAll();

                    MainActivity.Instance.FinishAffinity();
                    Xamarin.Forms.Application.Current.Quit();

                    ////强制退出应用程序
                    //Process.KillProcess(Process.MyPid());
                    ////正常退出
                    //JavaSystem.Exit(0);

                    Settings.IsNextTimeUpdate = true;
                }
            }
        }


        public bool CheckNewVersion(UpdateInfo updateInfo)
        {
            try
            {
                return updateUtils.CheckNewVersion(AutoUpdate.Context, updateInfo);
            }
            catch (Exception) { return false; }
        }

        public string GetVersion()
        {
            if (AutoUpdate.Context != null)
                return UpdateUtils.GetVersion(AutoUpdate.Context);
            else
                return "";
        }

        public string GetDeviceId()
        {
            return Id;
        }

        private string id = string.Empty;
        public string Id
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(id))
                    return id;

                if ((int)Build.VERSION.SdkInt <= 28)
                {
#pragma warning disable CS0618
                    id = Build.Serial;
#pragma warning restore CS0618 
                }
                else
                {
                    id = Build.GetSerial();
                }

                if (string.IsNullOrWhiteSpace(id) || id == Build.Unknown || id == "0")
                {
                    try
                    {
                        var context = Application.Context;
                        id = MySettings.Secure.GetString(context.ContentResolver, MySettings.Secure.AndroidId);
                    }
                    catch (Java.Lang.Exception ex)
                    {
                        Android.Util.Log.Warn("DeviceInfo", "Unable to get id: " + ex.ToString());
                    }
                }

                return id;
            }
        }

    }
}