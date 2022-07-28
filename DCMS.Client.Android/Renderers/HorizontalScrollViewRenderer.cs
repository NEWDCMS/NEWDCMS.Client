using DCMS.Client.Droid.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using HorizontalScrollView = DCMS.Client.CustomViews.HorizontalScrollView;

[assembly: ExportRenderer(typeof(HorizontalScrollView), typeof(HorizontalScrollViewRenderer))]

namespace DCMS.Client.Droid.Renderers
{
    /// <summary>
    /// 水平滚动条
    /// </summary>
    public class HorizontalScrollViewRenderer : ScrollViewRenderer
    {
        public HorizontalScrollViewRenderer(Android.Content.Context context) : base(context)
        {

        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            var element = e.NewElement as HorizontalScrollView;
            element?.Render();

            if (e.OldElement != null || this.Element == null)
                return;

            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;

            e.NewElement.PropertyChanged += OnElementPropertyChanged;
        }

        private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (ChildCount <= 0)
                return;

            GetChildAt(0).HorizontalScrollBarEnabled = false;
            GetChildAt(0).VerticalScrollBarEnabled = false;
        }
    }
}