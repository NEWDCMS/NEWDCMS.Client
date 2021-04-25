using Android.Animation;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.Core.Graphics.Drawable;
using Wesley.Client.CustomViews;
using Wesley.Client.Droid.Renderers;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CircleProgress), typeof(CircleProgressRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    /// <summary>
    /// 自定义 Circle Progress Renderer
    /// </summary>
    public class CircleProgressRenderer : ViewRenderer<CircleProgress, ProgressBar>
    {
        private ProgressBar pBar;
        private Drawable pBarBackDrawable;
        private Drawable pBarForeDrawable;
        public CircleProgressRenderer(Android.Content.Context context) : base(context)
        {
            SetWillNotDraw(false);
        }

        public static void InitRender()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CircleProgress> e)
        {

            base.OnElementChanged(e);
            if (Control == null)
            {
                pBar = CreateNativeControl();
                SetNativeControl(pBar);
                CreateAnimation();
            }
        }

        protected override ProgressBar CreateNativeControl()
        {
            //Please use GetDrawable(this Context, string) instead
            pBarBackDrawable = DrawableCompat.Wrap(this.Context.GetDrawable("CircularProgress_background"));
            pBarForeDrawable = DrawableCompat.Wrap(this.Context.GetDrawable("CircularProgress_drawable"));

            DrawableCompat.SetTint(pBarBackDrawable, Element.BackColor.ToAndroid());
            DrawableCompat.SetTint(pBarForeDrawable, Element.ForeColor.ToAndroid());

            var nativeControl = new ProgressBar(this.Context, null, global::Android.Resource.Attribute.ProgressBarStyleHorizontal)
            {
                Indeterminate = false,
                Max = Element.Maximun,
                ProgressDrawable = pBarForeDrawable,
                Rotation = -90f,
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent),
            };

            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Kitkat)
#pragma warning disable CS0618 // 类型或成员已过时
                nativeControl.SetBackgroundDrawable(pBarBackDrawable);
#pragma warning restore CS0618 // 类型或成员已过时
            else
                nativeControl.SetBackground(pBarBackDrawable);

            return nativeControl;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            Rect bounds = new Rect();
            TextPaint paint = new TextPaint();
            paint.Color = Element.TextColor.ToAndroid();
            paint.TextSize = Element.TextSize;
            paint.GetTextBounds(Element.Text.ToString(), 0, Element.Text.ToString().Length, bounds);
            if (((this.Width / 2) - (Element.TextMargin * 4)) < bounds.Width())
            {
                float ratio = (float)((this.Width / 2) - Element.TextMargin * 4) / (float)bounds.Width();
                paint.TextSize = paint.TextSize * ratio;
                paint.GetTextBounds(Element.Text.ToString(), 0, Element.Text.ToString().Length, bounds);
            }

            int x = this.Width / 2 - bounds.CenterX();
            int y = this.Height / 2 - bounds.CenterY();
            canvas.DrawText(Element.Text.ToString(), x, y, paint);
        }

        private void CreateAnimation()
        {
            ObjectAnimator anim = ObjectAnimator.OfInt(pBar, "progress", Element.Minimun, Element.Value);
            anim.SetDuration(Element.AnimationDuration);
            anim.SetInterpolator(new DecelerateInterpolator());
            anim.Start();
        }

    }
}
