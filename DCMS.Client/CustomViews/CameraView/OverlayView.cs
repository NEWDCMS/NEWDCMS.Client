using Xamarin.Forms;

namespace Wesley.Client.Camera
{
    public enum OverlayShape
    {
        Rect,
        Oval,
    }
    public class OverlayView : View
    {

        public static readonly BindableProperty ShowOverlayProperty = BindableProperty.Create(
            propertyName: nameof(ShowOverlay),
            returnType: typeof(bool),
            declaringType: typeof(OverlayView),
            defaultValue: true,
            defaultBindingMode: BindingMode.TwoWay);

        public bool ShowOverlay {
            get => (bool) GetValue(ShowOverlayProperty);
            set => SetValue(ShowOverlayProperty, value);
        }

        public static readonly BindableProperty OverlayOpacityProperty = BindableProperty.Create(
            propertyName: nameof(OverlayOpacity),
            returnType: typeof(float),
            declaringType: typeof(OverlayView),
            defaultValue: 0.6f,
            defaultBindingMode: BindingMode.TwoWay);

        public float OverlayOpacity {
            get => (float) GetValue(OverlayOpacityProperty);
            set => SetValue(OverlayOpacityProperty, value);
        }

        public static readonly BindableProperty OverlayBackgroundColorProperty = BindableProperty.Create(
            propertyName: nameof(OverlayBackgroundColor),
            returnType: typeof(Color),
            declaringType: typeof(OverlayView),
            defaultValue: Color.Gray,
            defaultBindingMode: BindingMode.TwoWay);

        public Color OverlayBackgroundColor {
            get => (Color) GetValue(OverlayBackgroundColorProperty);
            set => SetValue(OverlayBackgroundColorProperty, value);
        }

        public static readonly BindableProperty ShapeProperty = BindableProperty.Create(
            propertyName: nameof(Shape),
            returnType: typeof(OverlayShape),
            declaringType: typeof(OverlayView),
            defaultValue: OverlayShape.Rect,
            defaultBindingMode: BindingMode.TwoWay);

        public OverlayShape Shape {
            get => (OverlayShape) GetValue(ShapeProperty);
            set => SetValue(ShapeProperty, value);
        }
    }
}