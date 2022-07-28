using Android.Content;
using Android.Graphics.Drawables;
using DCMS.Client.CustomViews;
using DCMS.Client.Droid.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using FrameRenderer = Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer;

[assembly: ExportRenderer(typeof(CustomFrame), typeof(CustomFrameRenderer))]
namespace DCMS.Client.Droid.Renderers
{
    /// <summary>
    /// 自定义Frame渲染
    /// </summary>
    public class CustomFrameRenderer : FrameRenderer
    {
        public CustomFrameRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && Control != null)
            {
                var frame = (Element as CustomFrame);

                UpdateCornerRadius();

                // Clips the content to the box we provide.
                if (frame.IsClippedToBounds)
                {
                    this.ClipToOutline = true;
                }

                if (frame.HasShadow)
                {
                    this.ClipToOutline = true;
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(CustomFrame.CornerRadius) ||
                e.PropertyName == nameof(CustomFrame))
            {
                UpdateCornerRadius();
            }
        }

        private void UpdateCornerRadius()
        {
            if (Control.Background is GradientDrawable backgroundGradient)
            {
                var cornerRadius = (Element as CustomFrame)?.CornerRadius;
                if (!cornerRadius.HasValue)
                {
                    return;
                }

                var topLeftCorner = Context.ToPixels(cornerRadius.Value.TopLeft);
                var topRightCorner = Context.ToPixels(cornerRadius.Value.TopRight);
                var bottomLeftCorner = Context.ToPixels(cornerRadius.Value.BottomLeft);
                var bottomRightCorner = Context.ToPixels(cornerRadius.Value.BottomRight);

                var cornerRadii = new[]
                {
                    topLeftCorner,
                    topLeftCorner,

                    topRightCorner,
                    topRightCorner,

                    bottomRightCorner,
                    bottomRightCorner,

                    bottomLeftCorner,
                    bottomLeftCorner,
                };

                backgroundGradient.SetCornerRadii(cornerRadii);
            }
        }
    }
}