using Android.Widget;
using Wesley.Effects.Droid;
using Wesley.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(AndroidListViewStyleEffect), nameof(ListViewStyleEffect))]

namespace Wesley.Effects.Droid
{
    [Preserve]
    public class AndroidListViewStyleEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var listView = (Android.Widget.ListView)Control;

            if (ListViewEffect.GetDisableSelection(Element))
            {
                listView.ChoiceMode = ChoiceMode.None;
            }
        }

        protected override void OnDetached()
        {
        }
    }
}