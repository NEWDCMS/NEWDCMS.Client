namespace DCMS.Easycharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SkiaSharp;

    /// <summary>
    /// 指数图表
    /// </summary>
    public class PointChart : Chart
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microcharts.PointChart"/> class.
        /// </summary>
        public PointChart()
        {
            this.LabelOrientation = Orientation.Default;
            this.ValueLabelOrientation = Orientation.Default;
        }

        #endregion

        #region Fields

        private Orientation labelOrientation, valueLabelOrientation;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the size of the point.
        /// </summary>
        /// <value>The size of the point.</value>
        public float PointSize { get; set; } = 14;

        /// <summary>
        /// Gets or sets the point mode.
        /// </summary>
        /// <value>The point mode.</value>
        public PointMode PointMode { get; set; } = PointMode.Circle;

        /// <summary>
        /// Gets or sets the point area alpha.
        /// </summary>
        /// <value>The point area alpha.</value>
        public byte PointAreaAlpha { get; set; } = 100;

        /// <summary>
        /// Gets or sets the text orientation of labels.
        /// </summary>
        /// <value>The label orientation.</value>
        public Orientation LabelOrientation
        {
            get => this.labelOrientation;
            set => this.labelOrientation = (value == Orientation.Default) ? Orientation.Vertical : value;
        }

        /// <summary>
        /// Gets or sets the text orientation of value labels.
        /// </summary>
        /// <value>The label orientation.</value>
        public Orientation ValueLabelOrientation
        {
            get => this.valueLabelOrientation;
            set => this.valueLabelOrientation = (value == Orientation.Default) ? Orientation.Vertical : value;
        }

        private float ValueRange => this.MaxValue - this.MinValue;

        #endregion

        #region Methods

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

                this.DrawPointAreas(canvas, points, origin);
                this.DrawPoints(canvas, points);
                this.DrawHeader(canvas, valueLabels, valueLabelSizes, points, itemSize, height, headerHeight);
                this.DrawFooter(canvas, labels, labelSizes, points, itemSize, height, footerHeight);
            }
        }

        protected float CalculateYOrigin(float itemHeight, float headerHeight)
        {
            if (this.MaxValue <= 0)
            {
                return headerHeight;
            }

            if (this.MinValue > 0)
            {
                return headerHeight + itemHeight;
            }

            return headerHeight + ((this.MaxValue / this.ValueRange) * itemHeight);
        }

        protected SKSize CalculateItemSize(int width, int height, float footerHeight, float headerHeight)
        {
            var total = this.Entries.Count();
            var w = (width - ((total + 1) * this.Margin)) / total;
            var h = height - this.Margin - footerHeight - headerHeight;
            return new SKSize(w, h);
        }

        protected SKPoint[] CalculatePoints(SKSize itemSize, float origin, float headerHeight)
        {
            var result = new List<SKPoint>();

            for (int i = 0; i < this.Entries.Count(); i++)
            {
                var entry = this.Entries.ElementAt(i);
                var value = entry.Value;
                var x = this.Margin + (itemSize.Width / 2) + (i * (itemSize.Width + this.Margin));
                var y = headerHeight + ((1 - this.AnimationProgress) * (origin - headerHeight) + (((this.MaxValue - value) / this.ValueRange) * itemSize.Height) * this.AnimationProgress);
                var point = new SKPoint(x, y);
                result.Add(point);
            }

            return result.ToArray();
        }

        /// <summary>
        /// 绘制 Header
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="labels"></param>
        /// <param name="labelSizes"></param>
        /// <param name="points"></param>
        /// <param name="itemSize"></param>
        /// <param name="height"></param>
        /// <param name="headerHeight"></param>
        protected void DrawHeader(SKCanvas canvas, string[] labels, SKRect[] labelSizes, SKPoint[] points, SKSize itemSize, int height, float headerHeight)
        {
            this.DrawHeaderLabels(canvas,
                            labels,
                            points.Select(p => new SKPoint(p.X, headerHeight - this.Margin)).ToArray(),
                            labelSizes,
                            this.Entries.Select(x => x.Color.WithAlpha((byte)(255 * this.AnimationProgress))).ToArray(),
                            this.ValueLabelOrientation,
                            true,
                            itemSize,
                            height);
        }

        /// <summary>
        /// 绘制 Footer
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="labels"></param>
        /// <param name="labelSizes"></param>
        /// <param name="points"></param>
        /// <param name="itemSize"></param>
        /// <param name="height"></param>
        /// <param name="footerHeight"></param>
        protected void DrawFooter(SKCanvas canvas, string[] labels, SKRect[] labelSizes, SKPoint[] points, SKSize itemSize, int height, float footerHeight)
        {
            this.DrawFooterLabels(canvas,
                            labels,
                            points.Select(p => new SKPoint(p.X, height - footerHeight + this.Margin)).ToArray(),
                            labelSizes,
                            this.Entries.Select(x => this.LabelColor).ToArray(),
                            this.LabelOrientation,
                            false,
                            itemSize,
                            footerHeight);
        }

        /// <summary>
        /// 绘制 Footer
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="labels"></param>
        /// <param name="labelSizes"></param>
        /// <param name="points"></param>
        /// <param name="itemSize"></param>
        /// <param name="height"></param>
        /// <param name="footerHeight"></param>
        protected void DrawLineFooter(SKCanvas canvas, string[] labels, SKRect[] labelSizes, SKPoint[] points, SKSize itemSize, int height, float footerHeight)
        {
            this.DrawFooterLabels(canvas,
                            labels,
                            points.Select(p => new SKPoint(p.X, height - footerHeight + this.Margin)).ToArray(),
                            labelSizes,
                            this.Entries.Select(x => this.LabelColor).ToArray(),
                            this.LabelOrientation,
                            false,
                            itemSize,
                            footerHeight, true);
        }

        protected void DrawPoints(SKCanvas canvas, SKPoint[] points)
        {
            if (points.Length > 0 && PointMode != PointMode.None)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];
                    canvas.DrawPoint(point, entry.Color, this.PointSize, this.PointMode);
                }
            }
        }

        protected void DrawPointAreas(SKCanvas canvas, SKPoint[] points, float origin)
        {
            if (points.Length > 0 && this.PointAreaAlpha > 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];
                    var y = Math.Min(origin, point.Y);

                    using (var shader = SKShader.CreateLinearGradient(new SKPoint(0, origin), new SKPoint(0, point.Y), new[] { entry.Color.WithAlpha(this.PointAreaAlpha), entry.Color.WithAlpha((byte)(this.PointAreaAlpha / 3)) }, null, SKShaderTileMode.Clamp))
                    using (var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color.WithAlpha(this.PointAreaAlpha),
                    })
                    {
                        paint.Shader = shader;
                        var height = Math.Max(2, Math.Abs(origin - point.Y));
                        canvas.DrawRect(SKRect.Create(point.X - (this.PointSize / 2), y, this.PointSize, height), paint);
                    }
                }
            }
        }

        protected void DrawHeaderLabels(SKCanvas canvas, string[] texts, SKPoint[] points, SKRect[] sizes, SKColor[] colors, Orientation orientation, bool isTop, SKSize itemSize, float height)
        {
            if (points.Length > 0)
            {
                var maxWidth = sizes.Max(x => x.Width);
                var avgHeightAdustment = 0d;

                if (isTop == false)
                {
                    avgHeightAdustment = sizes.Average(s => s.Height);
                }

                for (int i = 0; i < points.Length; i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];

                    if (!string.IsNullOrEmpty(texts[i]))
                    {
                        using (new SKAutoCanvasRestore(canvas))
                        {
                            using (var paint = new SKPaint())
                            {
                                paint.TextSize = this.LabelTextSize;
                                paint.IsAntialias = true;
                                paint.Color = colors[i];
                                paint.IsStroke = false;
                                paint.Typeface = base.Typeface;
                                var bounds = sizes[i];
                                var text = texts[i];


                                //var fontManager = SKFontManager.Default;
                                //var emojiTypeface = fontManager.MatchCharacter('');
                                //paint.Typeface = emojiTypeface;


                                if (text != null)
                                {
                                    if (orientation == Orientation.Vertical)
                                    {
                                        var y = point.Y;
                                        if (isTop)
                                        {
                                            y -= bounds.Width;
                                        }

                                        canvas.RotateDegrees(90);
                                        canvas.Translate(y, -point.X + (bounds.Height / 2));
                                        canvas.DrawText(text, 0, 0, paint);
                                    }
                                    else
                                    {
                                        if (bounds.Width > itemSize.Width)
                                        {
                                            text = text.Substring(0, Math.Min(3, text.Length));
                                            paint.MeasureText(text, ref bounds);
                                        }

                                        if (bounds.Width > itemSize.Width)
                                        {
                                            text = text.Substring(0, Math.Min(1, text.Length));
                                            paint.MeasureText(text, ref bounds);
                                        }


                                        var y = point.Y;
                                        if (isTop)
                                        {
                                            y -= bounds.Height;
                                        }

                                        canvas.Translate(point.X - (bounds.Width / 2), y);
                                        canvas.DrawText(text, 0, 0, paint);
                                    }


                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 绘制Footer 标签
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="texts"></param>
        /// <param name="points"></param>
        /// <param name="sizes"></param>
        /// <param name="colors"></param>
        /// <param name="orientation"></param>
        /// <param name="isTop"></param>
        /// <param name="itemSize"></param>
        /// <param name="footerHeight"></param>
        /// <param name="isLineBar"></param>
        protected void DrawFooterLabels(SKCanvas canvas, string[] texts, SKPoint[] points, SKRect[] sizes, SKColor[] colors, Orientation orientation, bool isTop, SKSize itemSize, float footerHeight, bool isLineBar = false)
        {
            if (points.Length > 0)
            {
                var maxWidth = sizes.Max(x => x.Width);
                var avgHeightAdustment = 0d;

                if (isTop == false)
                {
                    avgHeightAdustment = sizes.Average(s => s.Height);
                }

                for (int i = 0; i < points.Length; i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];

                    if (!string.IsNullOrEmpty(texts[i]))
                    {
                        using (new SKAutoCanvasRestore(canvas))
                        {
                            using (var paint = new SKPaint())
                            {
                                paint.TextSize = this.LabelTextSize;
                                paint.IsAntialias = true;
                                paint.Color = colors[i];
                                paint.IsStroke = false;
                                paint.Typeface = base.Typeface;
                                var bounds = sizes[i];
                                var text = texts[i];

                                if (text != null)
                                {
                                    //水平方向
                                    if (orientation == Orientation.Vertical)
                                    {
                                        var y = point.Y;
                                        if (isTop)
                                        {
                                            y -= bounds.Width;
                                        }

                                        canvas.RotateDegrees(90);

                                        //canvas.DrawText(text, point.X - (bounds.Width / 2), height - (this.Margin + (this.LabelTextSize / 2)), paint);
                                        canvas.Translate(y, -point.X + (bounds.Height / 2));

                                        //canvas.Translate(point.X - (bounds.Width / 2), height - (this.Margin + (this.LabelTextSize / 2)));

                
                                        canvas.DrawText(text, 0, 0, paint);
                                    }
                                    //垂直方向
                                    else if (orientation == Orientation.Rotate)
                                    {
                                        if (bounds.Width > itemSize.Width)
                                        {
                                            text = text.Substring(0, Math.Min(3, text.Length));
                                            paint.MeasureText(text, ref bounds);
                                        }

                                        if (bounds.Width > itemSize.Width)
                                        {
                                            text = text.Substring(0, Math.Min(1, text.Length));
                                            paint.MeasureText(text, ref bounds);
                                        }


                                        var y = point.Y;
                                        if (isTop)
                                        {
                                            y -= bounds.Height;
                                        }

                                        //折线类
                                        if (isLineBar)
                                        {
                                            canvas.RotateDegrees(-45, point.X - (bounds.Width / 2), point.Y - (bounds.Height / 2) + (footerHeight / 2) + this.Margin);
                                            canvas.Translate(point.X - (bounds.Width / 2), point.Y + (bounds.Height / 2) + (footerHeight / 2) + this.Margin);
                                            canvas.DrawText(text, 0, 0, paint);
                                        }
                                        //条状类
                                        else
                                        {

                                            canvas.RotateDegrees(-45, point.X - bounds.Width, point.Y - (bounds.Height / 2) + (footerHeight / 2) + this.Margin);
                                            canvas.Translate(point.X - bounds.Width - this.Margin, point.Y + (bounds.Height / 2) + (footerHeight / 2) + this.Margin);
                                            canvas.DrawText(text, 0, 0, paint);
                                        }
                                    }
                                    else
                                    {
                                        if (bounds.Width > itemSize.Width)
                                        {
                                            text = text.Substring(0, Math.Min(3, text.Length));
                                            paint.MeasureText(text, ref bounds);
                                        }

                                        if (bounds.Width > itemSize.Width)
                                        {
                                            text = text.Substring(0, Math.Min(1, text.Length));
                                            paint.MeasureText(text, ref bounds);
                                        }


                                        var y = point.Y;
                                        if (isTop)
                                        {
                                            y -= bounds.Height;
                                        }

                                        canvas.Translate(point.X - (bounds.Width / 2), y);
                                        canvas.DrawText(text, 0, 0, paint);

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

      /// <summary>
      /// 计算Footer 高度
      /// </summary>
      /// <param name="valueLabelSizes"></param>
      /// <param name="orientation"></param>
      /// <param name="labels"></param>
      /// <returns></returns>
        protected float CalculateFooterHeaderHeight(SKRect[] valueLabelSizes, Orientation orientation, string[] labels)
        {
            var result = this.Margin;

            if (labels.Any(e => !string.IsNullOrEmpty(e)))
            {
                if (orientation == Orientation.Vertical)
                {
                    var maxValueWidth = valueLabelSizes.Max(x => x.Width);
                    if (maxValueWidth > 0)
                    {
                        result += maxValueWidth + this.Margin;
                    }
                }
                else
                {
                    //有角度市
                    var maxValueWidth = valueLabelSizes.Max(x => x.Width);
                    var height = (float)(Math.Sin(45) * maxValueWidth) + maxValueWidth + this.Margin;
                    result += height;
                }
            }

            return result;
        }

        /// <summary>
        /// Measures the value labels.
        /// </summary>
        /// <returns>The value labels.</returns>
        protected SKRect[] MeasureLabels(string[] labels)
        {
            using (var paint = new SKPaint())
            {
                paint.TextSize = this.LabelTextSize;
                return labels.Select(text =>
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        return SKRect.Empty;
                    }

                    var bounds = new SKRect();
                    paint.MeasureText(text, ref bounds);
                    return bounds;
                }).ToArray();
            }
        }

        #endregion



        #region 水平

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueLabelSizes"></param>
        /// <returns></returns>
        protected float CalculateLeftWidth(SKRect[] valueLabelSizes)
        {
            var result = this.Margin;//20

            if (this.Entries.Any(e => !string.IsNullOrEmpty(e.Label)))
            {
                result += this.LabelTextSize + this.Margin;
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueLabelSizes"></param>
        /// <returns></returns>
        protected float CalculateRightWidth(SKRect[] valueLabelSizes)
        {
            var result = this.Margin;

            if (this.Entries.Any())
            {
                var maxValueHeight = valueLabelSizes.Max(x => x.Height);
                if (maxValueHeight > 0)
                {
                    result += maxValueHeight + this.Margin;
                }
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemWidth"></param>
        /// <param name="rightWidth"></param>
        /// <returns></returns>
        public float CalculateXOrigin(float itemWidth, float rightWidth)
        {
            if (this.MaxValue <= 0)
            {
                return rightWidth;
            }

            if (this.MinValue > 0)
            {
                return rightWidth + itemWidth;
            }

            return rightWidth + ((this.MaxValue / this.ValueRange) * itemWidth);
        }

        /*
         //Margin:20
        //width:1080 
        //height: 368
        //leftWidth: 56
        //rightWidth: 65
        */

        protected SKSize CalculateLRItemSize(int width, int height, float leftWidth, float rightWidth)
        {
            var total = this.Entries.Count();
            var w = width - this.Margin - leftWidth - rightWidth;
            //var h = (height - ((total + 1) * this.Margin)) / total - (this.Margin / 2);
            var h = (height - ((total + 1) * this.Margin)) / total;
            return new SKSize(w, h);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemSize"></param>
        /// <param name="origin"></param>
        /// <param name="leftWidth"></param>
        /// <returns></returns>
        protected SKPoint[] CalculateLRPoints(SKSize itemSize, float origin, float leftWidth)
        {
            var result = new List<SKPoint>();
            //939 154 1004 56
            for (int i = 0; i < this.Entries.Count(); i++)
            {
                var entry = this.Entries.ElementAt(i);

                var x = leftWidth + (((this.MaxValue - entry.Value) / this.ValueRange) * itemSize.Width);
                var y = this.Margin + (itemSize.Height / 2) + (i * (itemSize.Height + this.Margin));
                //192 97
                //56 271
                /*
                    var x = this.Margin + (itemSize.Width / 2) + (i * (itemSize.Width + this.Margin));
                    var y = headerHeight + (((this.MaxValue - entry.Value) / this.ValueRange) * itemSize.Height);
                 */

                var point = new SKPoint(x, y);
                result.Add(point);
            }

            return result.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="points"></param>
        /// <param name="itemSize"></param>
        /// <param name="width"></param>
        /// <param name="leftWidth"></param>
        protected void DrawLeft(SKCanvas canvas, SKPoint[] points, SKSize itemSize, int width, float leftWidth)
        {
            this.DrawLRLabels(canvas, points, itemSize, width, leftWidth);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="points"></param>
        /// <param name="itemSize"></param>
        /// <param name="width"></param>
        /// <param name="leftWidth"></param>
        protected void DrawLRLabels(SKCanvas canvas, SKPoint[] points, SKSize itemSize, int width, float leftWidth)
        {
            for (int i = 0; i < this.Entries.Count(); i++)
            {
                var entry = this.Entries.ElementAt(i);
                var point = points[i];

                if (!string.IsNullOrEmpty(entry.Label))
                {
                    using (var paint = new SKPaint())
                    {
                        paint.TextSize = this.LabelTextSize;
                        paint.IsAntialias = true;
                        paint.Color = SKColors.White;
                        paint.IsStroke = false;
                        //Noto SansCJK

                        var fontManager = SKFontManager.Default;
                        var emojiTypeface = fontManager.MatchCharacter('雪');

                        paint.Typeface = emojiTypeface;

                        var bounds = new SKRect();
                        var text = entry.Label;
                        paint.MeasureText(text, ref bounds);

                        if (bounds.Height > itemSize.Height)
                        {
                            text = text.Substring(0, Math.Min(3, text.Length));
                            paint.MeasureText(text, ref bounds);
                        }

                        if (bounds.Height > itemSize.Height)
                        {
                            text = text.Substring(0, Math.Min(1, text.Length));
                            paint.MeasureText(text, ref bounds);
                        }
                        //canvas.DrawText(text, (width - leftWidth - rightWidth) + this.Margin, point.Y - (bounds.Height / 2) + (this.Margin / 2), paint);

                        canvas.DrawText(text, 40, point.Y + (bounds.Height / 2) - (this.Margin / 2), paint);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="points"></param>
        protected void DrawLRPoints(SKCanvas canvas, SKPoint[] points)
        {
            if (points.Length > 0 && PointMode != PointMode.None)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];
                    canvas.DrawPoint(point, entry.Color, this.PointSize, this.PointMode);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="points"></param>
        /// <param name="itemSize"></param>
        /// <param name="width"></param>
        /// <param name="leftWidth"></param>
        /// <param name="rightWidth"></param>
        protected void DrawLRValueLabel(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float width, float leftWidth, float rightWidth)
        {
            if (points.Length > 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];
                    //var isAbove = point.X > (this.Margin + (itemSize.Width / 2));

                    if (!string.IsNullOrEmpty(entry.ValueLabel))
                    {
                        using (new SKAutoCanvasRestore(canvas))
                        {
                            using (var paint = new SKPaint())
                            {
                                paint.TextSize = this.LabelTextSize;
                                paint.FakeBoldText = true;
                                paint.IsAntialias = true;
                                paint.Color = entry.Color;
                                paint.IsStroke = false;

                                var bounds = new SKRect();
                                var text = entry.ValueLabel;
                                paint.MeasureText(text, ref bounds);

                                //canvas.RotateDegrees(45);
                                //canvas.Translate(-point.Y + (bounds.Width / 2), this.Margin);
                                //itemSize.Width
                                //canvas.DrawText(text, width - (2 * this.Margin + (this.LabelTextSize / 2)+ leftWidth + rightWidth), point.Y - (bounds.Height / 2) + (this.Margin / 2), paint);
                                canvas.DrawText(text, (width - leftWidth - rightWidth) + this.Margin, point.Y - (bounds.Height / 2) + (this.Margin / 2), paint);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
