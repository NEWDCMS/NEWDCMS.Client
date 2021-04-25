namespace Wesley.Easycharts
{
    using System;
    using System.Linq;

    using SkiaSharp;

    /// <summary>
    /// 水平条图表
    /// </summary>
    public class HorizontalBarChart : PointChart
    {
        #region Constructors

        public HorizontalBarChart()
        {
            this.PointSize = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the bar background area alpha.
        /// </summary>
        /// <value>The bar area alpha.</value>
        public byte BarAreaAlpha { get; set; } = 32;

        #endregion

        #region Methods

        /// <summary>
        /// Draws the content of the chart onto the specified canvas.
        /// </summary>
        /// <param name="canvas">The output canvas.</param>
        /// <param name="width">The width of the chart.</param>
        /// <param name="height">The height of the chart.</param>
        public override void DrawContent(SKCanvas canvas, int width, int height)
        {

            var labels = this.Entries.Select(x => x.Label).ToArray();

            //1080 368
            //var valueLabelSizes = MeasureValueLabels();
            var valueLabelSizes = this.MeasureLabels(labels);
            //左宽 56
            var leftWidth = CalculateLeftWidth(valueLabelSizes);
            //右宽 65
            var rightWidth = CalculateRightWidth(valueLabelSizes);

            var itemSize = CalculateLRItemSize(width, height, leftWidth, rightWidth);  

            var origin = CalculateXOrigin(itemSize.Width, rightWidth);

            var points = this.CalculateLRPoints(itemSize, origin, rightWidth);

            this.DrawBarAreas(canvas, points, itemSize, origin, rightWidth);

            this.DrawBars(canvas, points, itemSize, origin, rightWidth);

            this.DrawLRPoints(canvas, points);

            //标签
            this.DrawLeft(canvas, points, itemSize, width, leftWidth);
            //标签值
            this.DrawLRValueLabel(canvas, points, itemSize, width, leftWidth, rightWidth);
        }

        /// <summary>
        /// Draws the value bars.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="points">The points.</param>
        /// <param name="itemSize">The item size.</param>
        /// <param name="origin">The origin.</param>
        /// <param name="rightWidth">The right height.</param>
        protected void DrawBars(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float origin, float rightWidth)
        {
            //最小宽度
            const float MinBarWidth = 4;
            if (points.Length > 0)
            {
                for (int i = 0; i < this.Entries.Count(); i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];

                    using var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color,
                    };


                    var x = Math.Min(origin, point.X);
                    var y = point.Y - (itemSize.Height / 2);
                  
                    var width = Math.Max(MinBarWidth, Math.Abs(origin - point.X));
                    if (width < MinBarWidth)
                    {
                        width = MinBarWidth;
                        if (x + width > this.Margin + itemSize.Width)
                        {
                            y = rightWidth + itemSize.Width - width;
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"===============================================================");
                    System.Diagnostics.Debug.WriteLine($"{width}:{itemSize.Height}");
                    // var rect = SKRect.Create(x, y, itemSize.Width, height);
                    var rect = SKRect.Create(this.Margin, y, width, itemSize.Height);
                    //绘制矩形
                    canvas.DrawRect(rect, paint);
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
         /// <param name="rightWidth"></param>
        protected void DrawBarAreas(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float origin, float rightWidth)
        {
            if (points.Length > 0 && this.PointAreaAlpha > 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];

                    using var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color.WithAlpha(this.BarAreaAlpha),
                    };

                    /*
                           var height = Math.Abs(max - point.Y);
                        var y = Math.Min(max, point.Y);
                        canvas.DrawRect(SKRect.Create(point.X - (itemSize.Width / 2), y, itemSize.Width, height), paint);
                     */
                    //var max = entry.Value > 0 ? rightWidth : rightWidth + itemSize.Width;
                    //var width = Math.Abs(max - point.X);
                    //var x = Math.Min(max, point.X);


                    var x = Math.Min(origin, point.X);
                    var y = point.Y - (itemSize.Height / 2);

                    var width = Math.Max(4, Math.Abs(origin - point.X));
  
                    System.Diagnostics.Debug.WriteLine($"==============================2=================================");
                    System.Diagnostics.Debug.WriteLine($"{itemSize.Width}:{itemSize.Height}");

                    var rect = SKRect.Create(this.Margin, y, itemSize.Width, itemSize.Height);
                    //var rect = SKRect.Create(x, point.Y - (itemSize.Width / 2), itemSize.Width, itemSize.Height);
                    canvas.DrawRect(rect, paint);
                }
            }
        }

        #endregion
    }
}
