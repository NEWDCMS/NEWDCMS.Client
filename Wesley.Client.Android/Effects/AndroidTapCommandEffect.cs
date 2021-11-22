using Android.Widget;
using Wesley.Client.Droid.Effects;
using Wesley.Client.Effects;
using Wesley.Infrastructure.Tasks;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportEffect(typeof(AndroidTapCommandEffect), nameof(TapCommandEffect))]
namespace Wesley.Client.Droid.Effects
{
    [Preserve]
    public class AndroidTapCommandEffect : PlatformEffect
    {
        private FrameLayout _clickOverlay;

        protected override void OnAttached()
        {
            if (Container == null)
            {
                return;
            }

            _clickOverlay = ViewOverlayCollector.Add(Container, this);
            _clickOverlay.Click += ViewOnClick;
            _clickOverlay.LongClick += ViewOnLongClick;
        }

        protected override void OnDetached()
        {
            var renderer = Container as IVisualElementRenderer;
            // Check disposed
            if (renderer?.Element != null)
            {
                _clickOverlay.Click -= ViewOnClick;
                _clickOverlay.LongClick -= ViewOnLongClick;

                ViewOverlayCollector.Delete(Container, this);
            }
        }

        private void ViewOnClick(object sender, EventArgs eventArgs)
        {
            NotifyTask.Create(
                async () =>
                {
                    await Task.Delay(50);
                    TapCommandEffect.GetTap(Element)?.Execute(TapCommandEffect.GetTapParameter(Element));
                });
        }

        private void ViewOnLongClick(object sender, View.LongClickEventArgs longClickEventArgs)
        {
            var cmd = TapCommandEffect.GetLongTap(Element);

            if (cmd == null)
            {
                longClickEventArgs.Handled = false;
                return;
            }

            NotifyTask.Create(
                async () =>
                {
                    await Task.Delay(50);
                    cmd.Execute(TapCommandEffect.GetLongTapParameter(Element));
                    longClickEventArgs.Handled = true;
                });
        }
    }
}