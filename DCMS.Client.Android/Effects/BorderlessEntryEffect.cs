using DCMS.Client.Droid.Effects;
using DCMS.Client.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(BorderlessAndroidEntryEffect), nameof(BorderlessEntryEffect))]
namespace DCMS.Client.Droid.Effects
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