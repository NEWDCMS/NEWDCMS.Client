using Android.Content;
using Android.Graphics;
using Wesley.Client.CustomViews;
using Wesley.Client.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomCollectionView), typeof(CustomCollectionViewRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    public class CustomCollectionViewRenderer : CollectionViewRenderer
    {
        public CustomCollectionViewRenderer(Context context) : base(context)
        {
        }
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            base.DispatchDraw(canvas);
            ((CustomCollectionView)Element).OnLoaded();
        }
    }
}