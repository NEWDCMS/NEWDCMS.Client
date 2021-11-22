using Android.Widget;
using Wesley.Client.Droid.Effects;
using Wesley.Client.Effects;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(NoKeyboardEffect_Droid), nameof(NoKeyboardEffect))]

namespace Wesley.Client.Droid.Effects
{
    public class NoKeyboardEffect_Droid : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                if (Control is EditText editText)
                {
                    // cursor shown, but keyboard will not be displayed
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
                    {
                        editText.ShowSoftInputOnFocus = false;
                    }
                    else
                    {
                        editText.SetTextIsSelectable(true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(NoKeyboardEffect_Droid)} failed to attached: {ex.Message}");
            }
        }

        protected override void OnDetached()
        {
        }
    }
}