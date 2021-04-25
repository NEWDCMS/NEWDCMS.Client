using Xamarin.Forms;
namespace Wesley.Client.CustomViews
{
    public class CustomFrame : Frame
    {
        public static new readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CustomFrame), typeof(CornerRadius), typeof(CustomDatePicker));

        public CustomFrame()
        {
            // MK Clearing default values (e.g. on iOS it's 5)
            base.CornerRadius = 0;
            base.IsClippedToBounds = true;
        }

        public new CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
    }
}
