using Android;
using Android.OS;
using AndroidX.Core.Content;
using DCMS.Client.Services;
using Google.Android.Material.Snackbar;
using System.Threading.Tasks;


namespace DCMS.Client.Droid.Services
{
    internal class MicrophoneService : IMicrophoneService
    {
        public const int REQUEST_MIC = 1;
        private readonly string[] permissions = { Manifest.Permission.RecordAudio };
        private TaskCompletionSource<bool> tcsPermissions;

        public Task<bool> GetPermissionsAsync()
        {
            tcsPermissions = new TaskCompletionSource<bool>();

            // Permissions are required only for Marshmallow and up
            if ((int)Build.VERSION.SdkInt < 23)
            {
                tcsPermissions.TrySetResult(true);
            }
            else
            {
                var currentActivity = Android.App.Application.Context;
                if (ContextCompat.CheckSelfPermission(currentActivity, Manifest.Permission.RecordAudio) != (int)Android.Content.PM.Permission.Granted)
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
            if (currentActivity.ShouldShowRequestPermissionRationale(Manifest.Permission.RecordAudio))
            {
                Snackbar.Make(currentActivity.FindViewById(Android.Resource.Id.Content),
                    "应用程序需要麦克风权限.",
                    Snackbar.LengthIndefinite).SetAction("Ok",
                    v =>
                    {
                        currentActivity.RequestPermissions(permissions, REQUEST_MIC);
                    }).Show();
            }
            else
            {
                currentActivity.RequestPermissions(permissions, REQUEST_MIC);
            }
        }

        public void OnRequestPermissionsResult(bool isGranted)
        {
            tcsPermissions.TrySetResult(isGranted);
        }
    }
}