using Android.Widget;
using DCMS.Client.Droid.Effects;
using DCMS.Client.Effects;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportEffect(typeof(AndroidTintableImageEffect), nameof(TintableImageEffect))]
namespace DCMS.Client.Droid.Effects
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class AndroidTintableImageEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            UpdateColor();
        }

        protected override void OnDetached()
        {
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if ((Element is Image) && args.PropertyName == Image.SourceProperty.PropertyName)
            {
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            var effect =
                (TintableImageEffect)Element.Effects.FirstOrDefault(x => x is TintableImageEffect);
            var color = effect?.TintColor.ToAndroid();

            if (Control is ImageView imageView && imageView.Handle != IntPtr.Zero && color.HasValue)
            {
                Android.Graphics.Color tint = color.Value;

                imageView.SetColorFilter(tint);
            }
        }
    }
}