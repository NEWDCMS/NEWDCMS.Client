using Android.Graphics;
using Wesley.Client.Droid.Renderers;
using Wesley.Client.Plugin;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRendererAttribute(typeof(TintedImage), typeof(TintedImageRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    public class TintedImageRenderer : ImageRenderer
    {

        public TintedImageRenderer(Android.Content.Context context) : base(context)
        {

        }

        public static void Init()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            SetTint();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == TintedImage.TintColorProperty.PropertyName || e.PropertyName == TintedImage.SourceProperty.PropertyName)
                SetTint();
        }

        void SetTint()
        {
            if (Control == null || Element == null)
                return;

            if (((TintedImage)Element).TintColor.Equals(Xamarin.Forms.Color.Transparent))
            {
                //Turn off tinting

                if (Control.ColorFilter != null)
                    Control.ClearColorFilter();

                return;
            }

            //Apply tint color
            var colorFilter = new PorterDuffColorFilter(((TintedImage)Element).TintColor.ToAndroid(), PorterDuff.Mode.SrcIn);
            Control.SetColorFilter(colorFilter);
        }
    }
}