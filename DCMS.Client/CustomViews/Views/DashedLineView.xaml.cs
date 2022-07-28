using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
namespace Wesley.Client.CustomViews
{

    public partial class DashedLineView : ContentView
    {
        public Color LineColor
        {
            get; set;
        } = Color.Black;

        public float DashSize
        {
            get; set;
        } = 20;

        public float WhiteSize
        {
            get; set;
        } = 20;

        public float Phase
        {
            get; set;
        } = 0;

        public DashedLineView()
        {
            Content = new SKCanvasView();
            ((SKCanvasView)Content).PaintSurface += Canvas_PaintSurface;
        }

        private void Canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = LineColor.ToSKColor(),
                StrokeWidth = HeightRequest > 0 ? (float)HeightRequest : (float)WidthRequest,
                StrokeCap = SKStrokeCap.Butt,
                PathEffect = SKPathEffect.CreateDash(new float[] { DashSize, WhiteSize }, Phase)
            };

            SKPath path = new SKPath();
            if (HeightRequest > 0)
            {
                // Horizontal
                path.MoveTo(0, 0);
                path.LineTo(info.Width, 0);
            }
            else
            {
                // Vertikal
                path.MoveTo(0, 0);
                path.LineTo(0, info.Height);
            }

            canvas.DrawPath(path, paint);
        }
    }
}