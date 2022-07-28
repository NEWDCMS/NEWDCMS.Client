using Android.Content;
using DCMS.Client.Droid.Views;
using DCMS.Client.Camera;
using DCMS.Client.Droid.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(OverlayView), typeof(CameraViewRenderer))]
namespace DCMS.Client.Droid.Renderers
{
    public class CameraViewRenderer : ViewRenderer<OverlayView, NativeOverlayView>
    {
        public CameraViewRenderer(Context context) : base(context){ }

        protected override void OnElementChanged(ElementChangedEventArgs<OverlayView> e){
            base.OnElementChanged(e);

            if (Control == null) {
                SetNativeControl(new NativeOverlayView(Context));
            }

            if (e.NewElement != null) {
                Control.Opacity = Element.OverlayOpacity;
                Control.ShowOverlay = Element.ShowOverlay;
                Control.OverlayBackgroundColor = Element.OverlayBackgroundColor.ToAndroid();
                Control.Shape = Element.Shape;
            }

            if (e.OldElement != null) { }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e){
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == OverlayView.OverlayOpacityProperty.PropertyName) {
                Control.Opacity = Element.OverlayOpacity;
            }
            else if (e.PropertyName == OverlayView.OverlayBackgroundColorProperty.PropertyName) {
                Control.OverlayBackgroundColor = Element.OverlayBackgroundColor.ToAndroid();
            }
            else if (e.PropertyName == OverlayView.ShapeProperty.PropertyName) {
                Control.Shape = Element.Shape;
            }
            else if (e.PropertyName == OverlayView.ShowOverlayProperty.PropertyName) {
                Control.ShowOverlay = Element.ShowOverlay;
            }
        }
    }
}