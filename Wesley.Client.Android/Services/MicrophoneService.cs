using Android;
using Android.OS;
using AndroidX.Core.Content;
using Wesley.Client.Droid.Utils;
using Wesley.Client.Services;
using Google.Android.Material.Snackbar;
using System.Threading.Tasks;

using Wesley.Client.Droid.Services;
[assembly: Xamarin.Forms.Dependency(typeof(MicrophoneService))]
namespace Wesley.Client.Droid.Services
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
                var currentActivity = AppUtils.GetAppContext(); //MainActivity.Instance;
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
                    "App requires microphone permission.",
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