using System;
using Xamarin.Forms;
using Wesley.SlideOverKit;
using Xamarin.Forms.Platform.Android;
using Wesley.SlideOverKit.Droid;
using Android.Views;
using Android.Content;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer (typeof(MenuTabbedPage), typeof(MenuTabbedPageDroidRenderer))]
namespace Wesley.SlideOverKit.Droid
{
    public class MenuTabbedPageDroidRenderer : TabbedPageRenderer, ISlideOverKitTabbedPageRendererDroid
    {
        public Action<ElementChangedEventArgs<TabbedPage>> OnElementChangedEvent { get; set; }

        public Action<bool, int, int, int, int> OnLayoutEvent { get; set; }

        public Action<int, int, int, int> OnSizeChangedEvent { get; set; }

        public MenuTabbedPageDroidRenderer(Context context) : base(context)
        {
            new TabSlideOverKitDroidHandler().Init(this, context);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);
            OnElementChangedEvent?.Invoke(e);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            OnLayoutEvent?.Invoke(changed, l, t, r, b);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            OnSizeChangedEvent?.Invoke(w, h, oldw, oldh);
        }
    }
}

