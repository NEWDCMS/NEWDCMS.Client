using Wesley.Effects;
using Wesley.Effects.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(BorderlessAndroidEntryEffect), nameof(BorderlessEntryEffect))]
namespace Wesley.Effects.Droid
{
    public class BorderlessAndroidEntryEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Control.Background = null;
        }
        protected override void OnDetached()
        {
        }
    }
}