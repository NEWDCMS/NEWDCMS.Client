using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;
using System.Threading.Tasks;
using Android.Views;

namespace Wesley.Client.Droid
{
    [Activity(Label = "@string/ApplicationName", 
        MainLauncher = false,
        Theme = "@style/SplashScreen",
        NoHistory = true,
        Icon = "@mipmap/ic_launcher",
        LaunchMode = LaunchMode.SingleTask,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait | ScreenOrientation.Landscape)]
    public class SplashActivity : AppCompatActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);

            //Ìí¼ÓActivity
            ActivityCollector.AddActivity(this);

            base.Window.AddFlags(WindowManagerFlags.Fullscreen);
            base.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            base.Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                base.Window.DecorView.SetFitsSystemWindows(true);
                base.Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            //ÒÆ³ýActivity
            ActivityCollector.RemoveActivity(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() =>
            {
                RunOnUiThread(() =>
                {
                    StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    //Finish();
                    OverridePendingTransition(0, 0);
                });
            });
            startupWork.Start();
        }

        protected override void OnNewIntent(Intent intent)
        {
            Finish();
        }
    }
}