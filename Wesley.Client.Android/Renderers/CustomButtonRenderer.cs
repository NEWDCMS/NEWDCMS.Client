using Android.Graphics.Drawables;
using Wesley.Client.CustomViews;
using Wesley.Client.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using System;

[assembly: ExportRenderer(typeof(CustomButton), typeof(CustomButtonRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    /// <summary>
    /// 自定义按钮渲染
    /// </summary>
    public class CustomButtonRenderer : ButtonRenderer
    {
        public CustomButtonRenderer(Android.Content.Context context) : base(context)
        {
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }

        private Typeface TrySetFont(string fontName)
        {
            try
            {
                return Typeface.CreateFromAsset(Context.Assets, "" + fontName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("not found in assets. Exception: {0}", ex);
                try
                {
                    return Typeface.CreateFromFile("" + fontName);
                }
                catch (Exception ex1)
                {
                    Console.WriteLine("not found by file. Exception: {0}", ex1);

                    return Typeface.Default;
                }
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            try
            {
                base.OnElementChanged(e);
                Control.SetAllCaps(false);
                Control.SetPadding(0, 0, 0, 0);
                Control.SetBackgroundResource(Resource.Drawable.button_shadow);

                var button = e.NewElement as CustomButton;
                if (e.OldElement == null)
                {
                    var gradient = new GradientDrawable(GradientDrawable.Orientation.TopBottom,
                        new[] {
                        button.BackgroundStartColor.ToAndroid().ToArgb(),
                        button.BackgroundEndColor.ToAndroid().ToArgb()
                        });

                    gradient.SetCornerRadius((float)100.0);
                    Control.SetBackground(gradient);
                }
            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1.Message);
            }
        }
    }
}