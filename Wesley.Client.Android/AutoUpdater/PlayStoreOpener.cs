using Android.Content;
using Android.Content.PM;
using DCMS.Client.AutoUpdater;
using DCMS.Client.AutoUpdater.Services;
using System;
using System.Collections.Generic;

//[assembly: Dependency(typeof(PlayStoreOpener))]
namespace DCMS.Client.Droid.AutoUpdater
{
    public class PlayStoreOpener : IStoreOpener
    {
        public void OpenStore()
        {
            string appID =
#if DEBUG
                UpdateManager.AppIDDummy;
#else
                AutoUpdate.Context.PackageName;
#endif

            Intent storeIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse($"market://details?id={appID}"));
            bool foundApp = false;

            IList<ResolveInfo> otherApps = AutoUpdate.Context.PackageManager.QueryIntentActivities(storeIntent, PackageInfoFlags.Activities);
            foreach (ResolveInfo info in otherApps)
            {
                if (info.ActivityInfo.ApplicationInfo.PackageName == "com.dcms.clientv3")
                {
                    ActivityInfo storeActivityInfo = info.ActivityInfo;
                    ComponentName componentName = new ComponentName(storeActivityInfo.ApplicationInfo.PackageName, storeActivityInfo.Name);

                    storeIntent.AddFlags(ActivityFlags.NewTask);
                    storeIntent.AddFlags(ActivityFlags.ResetTaskIfNeeded);
                    storeIntent.AddFlags(ActivityFlags.ClearTop);

                    storeIntent.SetComponent(componentName);
                    AutoUpdate.Context.StartActivity(storeIntent);

                    foundApp = true;
                    break;
                }
            }

            if (!foundApp)
                throw new Exception("Could not find google play store app.");
        }
    }
}
