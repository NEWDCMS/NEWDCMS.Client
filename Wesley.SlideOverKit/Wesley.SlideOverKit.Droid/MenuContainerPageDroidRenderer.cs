using System;
using Xamarin.Forms;
using Wesley.SlideOverKit;
using Xamarin.Forms.Platform.Android;
using Wesley.SlideOverKit.Droid;
using Android.Views;
using Android.Content;

[assembly: ExportRenderer (typeof(MenuContainerPage), typeof(MenuContainerPageDroidRenderer))]
namespace Wesley.SlideOverKit.Droid
{
    public class MenuContainerPageDroidRenderer  : PageRenderer, ISlideOverKitPageRendererDroid
    {
        public Action<ElementChangedEventArgs<Page>> OnElementChangedEvent { get; set; }

        public Action<bool, int,int,int,int> OnLayoutEvent { get; set; }

        public Action<int,int,int,int> OnSizeChangedEvent { get; set; }

        public MenuContainerPageDroidRenderer (Context context):base(context)
        {
            new SlideOverKitDroidHandler ().Init (this, context);
        }

        protected override void OnElementChanged (ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged (e);
            OnElementChangedEvent?.Invoke(e);
        }

        protected override void OnLayout (bool changed, int l, int t, int r, int b)
        {
            try
            {
                base.OnLayout(changed, l, t, r, b);
                OnLayoutEvent?.Invoke(changed, l, t, r, b);
                //注意：在 CollectionView 中 ViewCell 引起的异常
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print($"MenuContainerPageDroidRenderer:{ex.Message}");
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            try
            {
                base.OnSizeChanged(w, h, oldw, oldh);
                OnSizeChangedEvent?.Invoke(w, h, oldw, oldh);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print($"OnSizeChanged:{ex.Message}");
            }
        }
    }
}

