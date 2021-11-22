using Android.App;
using Android.Content;
using Android.OS;
using System;

namespace Wesley.Client.Droid
{

    [Activity(Theme = "@style/SplashScreen",
        MainLauncher = true,
        Icon = "@mipmap/app",
        NoHistory = true)]
    public class SplashActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            this.Finish();
        }
    }
}