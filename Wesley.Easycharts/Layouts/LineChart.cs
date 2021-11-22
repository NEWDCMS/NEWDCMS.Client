namespace DCMS.Easycharts
{
    using System.Linq;
    using SkiaSharp;

    /// <summary>
    /// 线性图表
    /// </summary>
    public class LineChart : PointChart
    {
        #region Constructors

        /// <summary>
        /// Ctor
        /// </summary>
        public LineChart()
        {
            this.PointSize = 10;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 线默认大小
        /// </summary>
        public float LineSize { get; set; } = 2;

        /// <summary>
        /// 画线类型
        /// </summary>
        public LineMode LineMode { get; set; } = LineMode.Spline;

        /// <summary>
        /// 背景透明度
        /// </summary>
        public byte LineAreaAlpha { get; set; } = 32;

        /// <summary>
        /// 启用或禁用Y方向线条区域的淡出渐变
        /// </summary>
        public bool EnableYFadeOutGradient { get; set; } = false;

        #endregion

        #region Methods

        /// <summary>
        /// 绘制内容
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public override void DrawContent(SKCanvas canvas, int width, int height)
        {
            if (this.Entries != null)
            {
                var labels = this.Entries.Select(x => x.Label).ToArray();
                var labelSizes = this.MeasureLabels(labels);
                var footerHeight = this.CalculateFooterHeaderHeight(labelSizes, this.LabelOrientation, labels);

                var valueLabels = this.Entries.Select(x => x.ValueLabel).ToArray();
                var valueLabelSizes = this.MeasureLabels(valueLabels);
                var headerHeight = this.CalculateFooterHeaderHeight(valueLabelSizes, this.ValueLabelOrientation, valueLabels);

                var itemSize = this.CalculateItemSize(width, height, footerHeight, headerHeight);
                var origin = this.CalculateYOrigin(itemSize.Height, headerHeight);
                var points = this.CalculatePoints(itemSize, origin, headerHeight);

                this.DrawArea(canvas, points, itemSize, origin);
                this.DrawLine(canvas, points, itemSize);
                this.DrawPoints(canvas, points);
                this.DrawHeader(canvas, valueLabels, valueLabelSizes, points, itemSize, height, headerHeight);
                this.DrawLineFooter(canvas, labels, labelSizes, points, itemSize, height, footerHeight);
            }
        }

        /// <summary>
        /// 绘制线
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="points"></param>
        /// <param name="itemSize"></param>
        protected void DrawLine(SKCanvas canvas, SKPoint[] points, SKSize itemSize)
        {
            if (points.Length > 1 && this.LineMode != LineMode.None)
            {
                using (var paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.White,
                    StrokeWidth = this.LineSize,
                    IsAntialias = true,
                })
                {
                    using (var shader = this.CreateXGradient(points))
                    {
                        paint.Shader = shader;

                        var path = new SKPath();

                        path.MoveTo(points.First());

                        var last = (this.LineMode == LineMode.Spline) ? points.Length - 1 : points.Length;
                        for (int i = 0; i < last; i++)
                        {
                            if (this.LineMode == LineMode.Spline)
                            {
                                var entry = this.Entries.ElementAt(i);
                                var nextEntry = this.Entries.ElementAt(i + 1);
                                var cubicInfo = this.CalculateCubicInfo(points, i, itemSize);
                                path.CubicTo(cubicInfo.control, cubicInfo.nextControl, cubicInfo.nextPoint);
                            }
                            else if (this.LineMode == LineMode.Straight)
                            {
                                path.LineTo(points[i]);
                            }
                        }

                        canvas.DrawPath(path, paint);
                    }
                }
            }
        }

        /// <summary>
        /// 绘制背景区域
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="points"></param>
        /// <param name="itemSize"></param>
        /// <param name="origin"></param>
        protected void DrawArea(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float origin)
        {
            if (this.LineAreaAlpha > 0 && points.Length > 1)
            {
                using (var paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.White,
                    IsAntialias = true,
                })
                {
                    using (var shaderX = this.CreateXGradient(points, (byte)(this.LineAreaAlpha * this.AnimationProgress)))
                    using (var shaderY = this.CreateYGradient(points, (byte)(this.LineAreaAlpha * this.AnimationProgress)))
                    {
                        paint.Shader = EnableYFadeOutGradient ? SKShader.CreateCompose(shaderY, shaderX, SKBlendMode.SrcOut) : shaderX;

                        var path = new SKPath();

                        path.MoveTo(points.First().X, origin);
                        path.LineTo(points.First());

                        var last = (this.LineMode == LineMode.Spline) ? points.Length - 1 : points.Length;
                        for (int i = 0; i < last; i++)
                        {
                            if (this.LineMode == LineMode.Spline)
                            {
                                var entry = this.Entries.ElementAt(i);
                                var nextEntry = this.Entries.ElementAt(i + 1);
                                var cubicInfo = this.CalculateCubicInfo(points, i, itemSize);
                                path.CubicTo(cubicInfo.control, cubicInfo.nextControl, cubicInfo.nextPoint);
                            }
                            else if (this.LineMode == LineMode.Straight)
                            {
                                path.LineTo(points[i]);
                            }
                        }

                        path.LineTo(points.Last().X, origin);

                        path.Close();

                        canvas.DrawPath(path, paint);
                    }
                }
            }
        }

        private (SKPoint point, SKPoint control, SKPoint nextPoint, SKPoint nextControl) CalculateCubicInfo(SKPoint[] points, int i, SKSize itemSize)
        {
            var point = points[i];
            var nextPoint = points[i + 1];
            var controlOffset = new SKPoint(itemSize.Width * 0.8f, 0);
            var currentControl = point + controlOffset;
            var nextControl = nextPoint - controlOffset;
            return (point, currentControl, nextPoint, nextControl);
        }

        private SKShader CreateXGradient(SKPoint[] points, byte alpha = 255)
        {
            var startX = points.First().X;
            var endX = points.Last().X;
            var rangeX = endX - startX;

            return SKShader.CreateLinearGradient(
                new SKPoint(startX, 0),
                new SKPoint(endX, 0),
                this.Entries.Select(x => x.Color.WithAlpha(alpha)).ToArray(),
                null,
                SKShaderTileMode.Clamp);
        }

        private SKShader CreateYGradient(SKPoint[] points, byte alpha = 255)
        {
            var startY = points.Max(i => i.Y);
            var endY = 0;

            return SKShader.CreateLinearGradient(
                new SKPoint(0, startY),
                new SKPoint(0, endY),
                new SKColor[] { SKColors.White.WithAlpha(alpha), SKColors.White.WithAlpha(0) },
                null,
                SKShaderTileMode.Clamp);
        }

        #endregion
    }
}