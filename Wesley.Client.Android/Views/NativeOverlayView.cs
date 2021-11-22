using Android.Content;
using Android.Graphics;
using Android.Views;
using Wesley.Client.Camera;
using Xamarin.Forms.Platform.Android;

namespace Wesley.Client.Droid.Views
{
    /// <summary>
    /// 自定义Native 覆盖物视图
    /// </summary>
    public class NativeOverlayView : View
    {
        private Bitmap _windowFrame;

        private readonly Color _strokeColor = Xamarin.Forms.Color.FromHex("#53a245").ToAndroid();
        private bool _showOverlay;

        public bool ShowOverlay {
            get => _showOverlay;
            set {
                var repaint = !_showOverlay;
                _showOverlay = value;
                if (repaint) {
                    redraw();
                }
            }
        }


        private float _overlayOpacity = 0.5f;

        public float Opacity {
            get => _overlayOpacity;
            set {
                _overlayOpacity = value;
                redraw();
            }
        }

        private Color _overlayColor = Color.Gray;

        public Color OverlayBackgroundColor {
            get => _overlayColor;
            set {
                _overlayColor = value;
                redraw();
            }
        }

        private OverlayShape _overlayShape = OverlayShape.Rect;

        public OverlayShape Shape {
            get => _overlayShape;
            set {
                _overlayShape = value;
                redraw();
            }
        }

        public NativeOverlayView(Context context, bool showOverlay = false) : base(context){
            ShowOverlay = showOverlay;
            SetWillNotDraw(false);
        }

        protected override void OnDraw(Canvas canvas){
            base.OnDraw(canvas);
            if (!ShowOverlay)
                return;
            if (_windowFrame == null) {
                createWindowFrame();
            }
            canvas.DrawBitmap(_windowFrame, 0, 0, null);
        }

        private void redraw(){
            if (!ShowOverlay)
                return;
            _windowFrame?.Recycle();
            _windowFrame = null;
            Invalidate();
        }

        private void createWindowFrame(){
            float width = Width;
            float height = Height;

            _windowFrame = Bitmap.CreateBitmap((int) width, (int) height, Bitmap.Config.Argb8888);
            var osCanvas = new Canvas(_windowFrame);
            var fillPaint = new Paint(PaintFlags.AntiAlias) {
                Color = OverlayBackgroundColor,
                Alpha = (int) (255 * Opacity),
            };
            fillPaint.SetStyle(Paint.Style.Fill);

            var strokePaint = new Paint(PaintFlags.AntiAlias) {
                Color = _strokeColor,
                Alpha = 255,
                StrokeWidth = 8
            };
            strokePaint.SetStyle(Paint.Style.Stroke);

            var outerRectangle = new RectF(0, 0, width, height);
            osCanvas.DrawRect(outerRectangle, fillPaint);
            fillPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));

            if (Shape == OverlayShape.Rect) {
                var myRect = new RectF(width * 0.1f, height * 0.3f, width * 0.9f, height * 0.7f);
                osCanvas.DrawRoundRect(myRect, 8, 8, fillPaint);
                osCanvas.DrawRoundRect(myRect, 8, 8, strokePaint);
            }
            else if (Shape == OverlayShape.Oval) {
                var myOval = new RectF(width * 0.2f, height * 0.15f, width * 0.8f, height * 0.85f);
                osCanvas.DrawOval(myOval, fillPaint);
                osCanvas.DrawOval(myOval, strokePaint);
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b){
            base.OnLayout(changed, l, t, r, b);
            _windowFrame?.Recycle();
            _windowFrame = null;
        }
    }
}