using Xamarin.Forms;
namespace Wesley.Client.RenderedViews
{
    public class MaterialFrame : Frame
    {
        public static readonly BindableProperty ElevationProperty = BindableProperty.Create(
            nameof(Elevation),
            typeof(int),
            typeof(MaterialFrame),
            defaultValue: 2);

        public MaterialFrame()
        {
            HasShadow = false;
            CornerRadius = 0;
        }

        public int Elevation
        {
            get => (int)GetValue(ElevationProperty);
            set => SetValue(ElevationProperty, value);
        }
    }
}