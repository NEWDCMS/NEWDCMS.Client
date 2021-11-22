using Android.Widget;
using DCMS.Client.Droid.Effects;
using DCMS.Client.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(AndroidListViewStyleEffect), nameof(ListViewStyleEffect))]

namespace DCMS.Client.Droid.Effects
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