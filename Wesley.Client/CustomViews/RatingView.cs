﻿using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;
namespace SkiaRate
{
    public static class MaterialColors
    {
        public static SKColor Red => SKColor.Parse("F44336");
        public static SKColor Pink => SKColor.Parse("E91E63");
        public static SKColor Purple => SKColor.Parse("9C27B0");
        public static SKColor DeepPurple => SKColor.Parse("673AB7");
        public static SKColor Indigo => SKColor.Parse("3F51B5");
        public static SKColor Blue => SKColor.Parse("2196F3");
        public static SKColor LightBlue => SKColor.Parse("03A9F4");
        public static SKColor Cyan => SKColor.Parse("00BCD4");
        public static SKColor Teal => SKColor.Parse("009688");
        public static SKColor Green => SKColor.Parse("4CAF50");
        public static SKColor LightGreen => SKColor.Parse("8BC34A");
        public static SKColor Lime => SKColor.Parse("CDDC39");
        public static SKColor Yellow => SKColor.Parse("FFEB3B");
        public static SKColor Amber => SKColor.Parse("FFC107");
        public static SKColor Orange => SKColor.Parse("FF9800");
        public static SKColor DeepOrange => SKColor.Parse("FF5722");
        public static SKColor Brown => SKColor.Parse("795548");
        public static SKColor Grey => SKColor.Parse("9E9E9E");
        public static SKColor BlueGrey => SKColor.Parse("607D8B");
        public static SKColor Black => SKColor.Parse("000000");
        public static SKColor White => SKColor.Parse("FFFFFF");
    }

    public static class PathConstants
    {
        public const string Star = "M9 11.3l3.71 2.7-1.42-4.36L15 7h-4.55L9 2.5 7.55 7H3l3.71 2.64L5.29 14z";

        public const string MaterialStar = @"M19.65,9.04l-4.84-0.42l-1.89-4.45c-0.34-0.81-1.5-0.81-1.84,0L9.19,8.63L4.36,9.04c-0.88,0.07-1.24,1.17-0.57,1.7l3.67,3.18l-1.1,4.72c-0.2,0.86,0.73,1.54,1.49,1.08L12,17.27l4.15,2.51c0.76,0.46,1.69-0.22,1.49-1.08l-1.1-4.73l3.67-3.18C20.88,10.21,20.53,9.11,19.65,9.04zM12,15.4l-3.76,2.27l1-4.28l-3.32-2.88l4.38-0.38L12,6.1l1.71,4.04l4.38,0.38l-3.32,2.88l1,4.28L12,15.4z";

        public const string Heart =
            "M12 21.35l-1.45-1.32C5.4 15.36 2 12.28 2 8.5 2 5.42 4.42 3 7.5 3c1.74 0 3.41.81 4.5 2.09C13.09 3.81 14.76 3 16.5 3 19.58 3 22 5.42 22 8.5c0 3.78-3.4 6.86-8.55 11.54L12 21.35z";

        public const string Circle = "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z";
        public const string Bar = "M6 6h36v2H6z";

        public const string BatteryAlert =
            "M15.67 4H14V2h-4v2H8.33C7.6 4 7 4.6 7 5.33v15.33C7 21.4 7.6 22 8.33 22h7.33c.74 0 1.34-.6 1.34-1.33V5.33C17 4.6 16.4 4 15.67 4zM13 18h-2v-2h2v2zm0-4h-2V9h2v5z";

        public const string BatteryCharging =
            "M15.67 4H14V2h-4v2H8.33C7.6 4 7 4.6 7 5.33v15.33C7 21.4 7.6 22 8.33 22h7.33c.74 0 1.34-.6 1.34-1.33V5.33C17 4.6 16.4 4 15.67 4zM11 20v-5.5H9L13 7v5.5h2L11 20z";

        public const string Like =
            "M1 21h4V9H1v12zm22-11c0-1.1-.9-2-2-2h-6.31l.95-4.57.03-.32c0-.41-.17-.79-.44-1.06L14.17 1 7.59 7.59C7.22 7.95 7 8.45 7 9v10c0 1.1.9 2 2 2h9c.83 0 1.54-.5 1.84-1.22l3.02-7.05c.09-.23.14-.47.14-.73v-1.91l-.01-.01L23 10z";

        public const string Dislike =
            "M15 3H6c-.83 0-1.54.5-1.84 1.22l-3.02 7.05c-.09.23-.14.47-.14.73v1.91l.01.01L1 14c0 1.1.9 2 2 2h6.31l-.95 4.57-.03.32c0 .41.17.79.44 1.06L9.83 23l6.59-6.59c.36-.36.58-.86.58-1.41V5c0-1.1-.9-2-2-2zm4 0v12h4V3h-4z";

        public const string Theaters =
            "M18 3v2h-2V3H8v2H6V3H4v18h2v-2h2v2h8v-2h2v2h2V3h-2zM8 17H6v-2h2v2zm0-4H6v-2h2v2zm0-4H6V7h2v2zm10 8h-2v-2h2v2zm0-4h-2v-2h2v2zm0-4h-2V7h2v2z";

        public const string Problem = "M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z";
    }

    public enum RatingType
    {
        Full,
        Half,
        Floating
    }

    public class RatingView : SKCanvasView
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(
            nameof(Value),
            typeof(double),
            typeof(RatingView),
            default(double),
            propertyChanged: OnValueChanged);

        public static readonly BindableProperty PathProperty = BindableProperty.Create(
            nameof(Path),
            typeof(string),
            typeof(RatingView),
            PathConstants.Star,
            propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty CountProperty = BindableProperty.Create(
            nameof(Count),
            typeof(int),
            typeof(RatingView),
            5,
            propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty ColorOnProperty = BindableProperty.Create(
            nameof(ColorOn),
            typeof(Color),
            typeof(RatingView),
            MaterialColors.Amber.ToFormsColor(),
            propertyChanged: ColorOnChanged);

        public static readonly BindableProperty OutlineOnColorProperty = BindableProperty.Create(
            nameof(OutlineOnColor),
            typeof(Color),
            typeof(RatingView),
            SKColors.Transparent.ToFormsColor(),
            propertyChanged: OutlineOnColorChanged);

        public static readonly BindableProperty OutlineOffColorProperty = BindableProperty.Create(
            nameof(OutlineOffColor),
            typeof(Color),
            typeof(RatingView),
            MaterialColors.Grey.ToFormsColor(),
            propertyChanged: OutlineOffColorChanged);

        public static readonly BindableProperty RatingTypeProperty = BindableProperty.Create(
            nameof(RatingType),
            typeof(RatingType),
            typeof(RatingView),
            RatingType.Floating,
            propertyChanged: OnPropertyChanged);

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, ClampValue(value));
        }

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public int Count
        {
            get => (int)GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }

        public Color ColorOn
        {
            get => (Color)GetValue(ColorOnProperty);
            set => SetValue(ColorOnProperty, value);
        }

        public Color OutlineOnColor
        {
            get => (Color)GetValue(OutlineOnColorProperty);
            set => SetValue(OutlineOnColorProperty, value);
        }

        public Color OutlineOffColor
        {
            get => (Color)GetValue(OutlineOffColorProperty);
            set => SetValue(OutlineOffColorProperty, value);
        }

        public RatingType RatingType
        {
            get => (RatingType)GetValue(RatingTypeProperty);
            set => SetValue(RatingTypeProperty, value);
        }

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as RatingView;
            view.InvalidateSurface();
        }

        private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as RatingView;
            view.Value = view.ClampValue((double)newValue);
            OnPropertyChanged(bindable, oldValue, newValue);
        }

        private static void ColorOnChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as RatingView;
            view.SKColorOn = ((Color)newValue).ToSKColor();
            OnPropertyChanged(bindable, oldValue, newValue);
        }

        private static void OutlineOffColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as RatingView;
            view.SKOutlineOffColor = ((Color)newValue).ToSKColor();
            OnPropertyChanged(bindable, oldValue, newValue);
        }

        private static void OutlineOnColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as RatingView;
            view.SKOutlineOnColor = ((Color)newValue).ToSKColor();
            OnPropertyChanged(bindable, oldValue, newValue);
        }

        private SKPoint ConvertToPixel(Point pt)
        {
            if (Width == 0 || Height == 0)
                return new SKPoint((float)(CanvasSize.Width * pt.X / 1), (float)(CanvasSize.Height * pt.Y / 1));
            else
                return new SKPoint((float)(CanvasSize.Width * pt.X / Width), (float)(CanvasSize.Height * pt.Y / Height));
        }

        #region properties

        /// <summary>
        ///     Gets or sets the spacing between two rating elements
        /// </summary>
        public float Spacing { get; set; } = 8;

        /// <summary>
        ///     Gets or sets the color of the canvas background.
        /// </summary>
        /// <value>The color of the canvas background.</value>
        public SKColor CanvasBackgroundColor { get; set; } = SKColors.Transparent;

        /// <summary>
        ///     Gets or sets the width of the stroke.
        /// </summary>
        /// <value>The width of the stroke.</value>
        public float StrokeWidth { get; set; } = 0.1f;

        #endregion

        #region public methods

        /// <summary>
        ///     Clamps the value between 0 and the number of items.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="val">Value.</param>
        public double ClampValue(double val)
        {
            if (val < 0)
            {
                return 0;
            }

            if (val > Count)
            {
                return Count;
            }

            return val;
        }

        /// <summary>
        ///     Sets the Rating value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetValue(double x, double y)
        {
            var val = CalculateValue(x);
            switch (RatingType)
            {
                case RatingType.Full:
                    Value = ClampValue(Math.Ceiling(val));
                    break;
                case RatingType.Half:
                    Value = ClampValue(Math.Round(val * 2) / 2);
                    break;
                case RatingType.Floating:
                    Value = ClampValue(val);
                    break;
            }
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        /// <summary>
        ///     Draws the rating view
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Draw(SKCanvas canvas, int width, int height)
        {
            canvas.Clear(CanvasBackgroundColor);

            var path = SKPath.ParseSvgPathData(Path);

            var itemWidth = (width - (Count - 1) * Spacing) / Count;
            var scaleX = itemWidth / path.Bounds.Width;
            scaleX = (itemWidth - scaleX * StrokeWidth) / path.Bounds.Width;

            ItemHeight = height;
            var scaleY = ItemHeight / path.Bounds.Height;
            scaleY = (ItemHeight - scaleY * StrokeWidth) / path.Bounds.Height;

            CanvasScale = Math.Min(scaleX, scaleY);
            ItemWidth = path.Bounds.Width * CanvasScale;

            canvas.Scale(CanvasScale);
            canvas.Translate(StrokeWidth / 2, StrokeWidth / 2);
            canvas.Translate(-path.Bounds.Left, 0);
            canvas.Translate(0, -path.Bounds.Top);

            using (var strokePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKOutlineOnColor,
                StrokeWidth = StrokeWidth,
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true,
            })
            using (var fillPaint = new SKPaint { Style = SKPaintStyle.Fill, Color = SKColorOn, IsAntialias = true, })
            {
                for (var i = 0; i < Count; i++)
                {
                    if (i <= Value - 1) // Full
                    {
                        canvas.DrawPath(path, fillPaint);
                        canvas.DrawPath(path, strokePaint);
                    }
                    else if (i < Value) //Partial
                    {
                        var filledPercentage = (float)(Value - Math.Truncate(Value));
                        strokePaint.Color = SKOutlineOffColor;
                        canvas.DrawPath(path, strokePaint);

                        using (var rectPath = new SKPath())
                        {
                            var rect = SKRect.Create(
                                path.Bounds.Left + path.Bounds.Width * filledPercentage,
                                path.Bounds.Top,
                                path.Bounds.Width * (1 - filledPercentage),
                                ItemHeight);
                            rectPath.AddRect(rect);
                            canvas.ClipPath(rectPath, SKClipOperation.Difference);
                            canvas.DrawPath(path, fillPaint);
                        }
                    }
                    else //Empty
                    {
                        strokePaint.Color = SKOutlineOffColor;
                        canvas.DrawPath(path, strokePaint);
                    }

                    canvas.Translate((ItemWidth + Spacing) / CanvasScale, 0);
                }
            }
        }

        #endregion

        #region private

        private float ItemWidth { get; set; }
        private float ItemHeight { get; set; }
        private float CanvasScale { get; set; }
        private SKColor SKColorOn { get; set; } = MaterialColors.Amber;
        private SKColor SKOutlineOnColor { get; set; } = SKColors.Transparent;
        private SKColor SKOutlineOffColor { get; set; } = MaterialColors.Grey;

        private double CalculateValue(double x)
        {
            if (x < ItemWidth)
            {
                return x / ItemWidth;
            }

            if (x < ItemWidth + Spacing)
            {
                return 1;
            }

            return 1 + CalculateValue(x - (ItemWidth + Spacing));
        }

        #endregion
    }
}