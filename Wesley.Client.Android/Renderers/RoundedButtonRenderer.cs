using Android.Graphics;
using Android.Graphics.Drawables;
using Wesley.Client.CustomViews;
using Wesley.Client.Droid.Renderers;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RoundedButton), typeof(RoundedButtonRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    /// <summary>
    /// 圆形按钮
    /// </summary>
    public class RoundedButtonRenderer : ButtonRenderer
    {
        private GradientDrawable _normal;

        public RoundedButtonRenderer(Android.Content.Context context) : base(context)
        {
        }

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            base.OnDraw(canvas);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                var button = (RoundedButton)e.NewElement;

                if (button != null)
                {
                    button.SizeChanged += (s, args) =>
                    {
                        // for corner round
                        var radius = (float)Math.Min(button.Width, button.Height);
                        // Create a drawable for the button's normal state
                        _normal = new GradientDrawable();
                        _normal.SetCornerRadius(radius);
                    };


                    Control.SetAllCaps(false);


                    if (!string.IsNullOrEmpty((FileImageSource)button.ImageSource))
                        Control.SetPadding(Control.PaddingLeft * 4, Control.PaddingTop, Control.PaddingRight * 4, Control.PaddingBottom);

                    // For Custom Font
                    if (!string.IsNullOrEmpty(button.FontFamily))
                    {
                        Typeface typeface = Typeface.CreateFromAsset(this.Context.Assets, button.FontFamily);
                        Control.SetTypeface(typeface, TypefaceStyle.Bold);
                    }

                }
            }
        }
    }
}