using Wesley.Effects.Droid.Renderers;
using Android.App;
using Android.Runtime;


namespace Wesley.Effects.Droid
{
    public static class Effects
    {
        internal static Activity Activity;

        public static void Init(Activity activity)
        {
            Activity = activity;

            TouchEffectPlatform.Init();
            CommandsPlatform.Init();
            BorderViewRenderer.Init();

        
            //Activity.Window.DecorView.ViewTreeObserver.AddOnGlobalLayoutListener(new SoftKeyboardService());
        }
    }
}
