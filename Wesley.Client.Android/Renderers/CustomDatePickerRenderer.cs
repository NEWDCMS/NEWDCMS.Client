using Android.Graphics;
using Wesley.Client.CustomViews;
using Wesley.Client.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    /// <summary>
    /// 自定义事件选择器
    /// </summary>
    public class CustomDatePickerRenderer : DatePickerRenderer
    {
        public CustomDatePickerRenderer(Android.Content.Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                var nativeEditText = Control as Android.Widget.EditText;
                nativeEditText.Background = null;

                // For Custom Font
                if (e.NewElement is CustomDatePicker datePicker && !string.IsNullOrEmpty(datePicker.FontFamily))
                {
                    Typeface typeface = Typeface.CreateFromAsset(this.Context.Assets, datePicker.FontFamily);
                    Control.SetTypeface(typeface, TypefaceStyle.Normal);
                }
            }
        }

    }
}