using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;

namespace DCMS.Client.Droid
{
    public class TranslucentStatubar
    {
        public static void Immersive(Window window)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var newUiOptions = (int)SystemUiFlags.LayoutStable;
                newUiOptions |= (int)SystemUiFlags.LayoutFullscreen;
                newUiOptions |= (int)SystemUiFlags.HideNavigation;
                newUiOptions |= (int)SystemUiFlags.ImmersiveSticky;
                window.DecorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;
            }
        }

        public static int MixtureColor(int color, float alpha)
        {
            int a = (color & 0x000000) == 0 ? 0xff : color >> 24;
            return (color & 0xffffff) | (((int)(a * alpha)) << 24);
        }

        public static int GetStatusBarHeight(Context context)
        {
            int result = 24;
            int resId = context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resId > 0)
            {
                result = context.Resources.GetDimensionPixelSize(resId);
            }
            else
            {
                result = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip,
                        result, Android.Content.Res.Resources.System.DisplayMetrics);
            }
            return result;
        }
    }
}