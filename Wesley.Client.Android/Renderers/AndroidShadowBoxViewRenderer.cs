using Android.Content;
using Wesley.Client.Droid.Renderers;
using Wesley.Client.RenderedViews;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ShadowBoxView), typeof(AndroidShadowBoxViewRenderer))]

namespace Wesley.Client.Droid.Renderers
{
    /// <summary>
    /// Renderer to update all frames with better shadows matching material design standards.
    /// </summary>
    public class AndroidShadowBoxViewRenderer : VisualElementRenderer<ShadowBoxView>
    {
        public AndroidShadowBoxViewRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ShadowBoxView> e)
        {
            UpdateBackgroundColor();
        }

        protected override void UpdateBackgroundColor()
        {
            switch (Element.ShadowType)
            {
                case ShadowType.Top:
                    SetBackgroundResource(Resource.Drawable.top_shadow);
                    break;

                case ShadowType.Bottom:
                    SetBackgroundResource(Resource.Drawable.bottom_shadow);
                    break;
            }
        }
    }
}