using Wesley.BitImageEditor.Helper;
using Wesley.Client.Effects;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;

namespace Wesley.BitImageEditor.Croping
{
    internal struct TouchPoint
    {
        public int CornerIndex { set; get; }
        public SKPoint Offset { set; get; }
    }


    /// <summary>
    /// 图片裁切画布视图
    /// </summary>
    internal class ImageCropperCanvasView : SKCanvasView, IDisposable
    {
        private SKBitmap bitmap;
        private float? aspectRatio;
        private readonly CroppingRectangle croppingRect;
        private SKMatrix inverseBitmapMatrix;
        private Dictionary<long, TouchPoint> touchPoints = new Dictionary<long, TouchPoint>();
        private Dictionary<long, SKPoint> touchPointsInside = new Dictionary<long, SKPoint>();
        private SKPoint bitmapLocationfirst = new SKPoint();
        private SKPoint bitmapLocationlast = new SKPoint();
        private const int corner = 30;

        internal ImageCropperCanvasView(SKBitmap bitmap, float? aspectRatio = null)
        {
            this.bitmap = bitmap;
            croppingRect = new CroppingRectangle(new SKRect(0, 0, bitmap.Width, bitmap.Height), aspectRatio);
            SetAspectRatio(aspectRatio);
        }

        /// <summary>
        /// 裁切位图
        /// </summary>
        internal SKBitmap CroppedBitmap
        {
            get
            {
                SKRect cropRect = croppingRect.Rect;
                SKBitmap croppedBitmap = new SKBitmap((int)cropRect.Width, (int)cropRect.Height);
                SKRect dest = new SKRect(0, 0, cropRect.Width, cropRect.Height);
                SKRect source = new SKRect(cropRect.Left, cropRect.Top, cropRect.Right, cropRect.Bottom);
                using (SKCanvas canvas = new SKCanvas(croppedBitmap))
                {
                    canvas.DrawBitmap(bitmap, source, dest);
                }
                return croppedBitmap;
            }
        }

        /// <summary>
        /// 设置纵横比
        /// </summary>
        /// <param name="crop"></param>
        internal void SetAspectRatio(CropItem crop)
        {
            switch (crop?.Action)
            {
                case CropRotateType.CropRotate:
                    Rotate();
                    break;
                case CropRotateType.CropFree:
                    SetAspectRatio(null, false);
                    break;
                case CropRotateType.CropFull:
                    SetAspectRatio(null, true);
                    break;
                case CropRotateType.CropSquare:
                    SetAspectRatio(1f);
                    break;
                case CropRotateType.Crop2_3:
                    SetAspectRatio(2f / 3f);
                    break;
                case CropRotateType.Crop3_2:
                    SetAspectRatio(3f / 2f);
                    break;
                case CropRotateType.Crop3_4:
                    SetAspectRatio(3f / 4f);
                    break;
                case CropRotateType.Crop4_3:
                    SetAspectRatio(4f / 3f);
                    break;
                case CropRotateType.Crop16_9:
                    SetAspectRatio(16f / 9f);
                    break;
                case CropRotateType.Crop9_16:
                    SetAspectRatio(9f / 16f);
                    break;
            }
        }

        internal void OnTouchEffectTouchAction(TouchActionEventArgs args)
        {
            SKPoint pixelLocation = new SKPoint((float)(CanvasSize.Width * args.Location.X / Width), (float)(CanvasSize.Height * args.Location.Y / Height)); ;
            SKPoint bitmapLocation = inverseBitmapMatrix.MapPoint(pixelLocation);

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    // 将半径转换为位图/裁剪比例
                    float radius = inverseBitmapMatrix.ScaleX * 50;

                    // 找到手指碰到的角落
                    int cornerIndex = croppingRect.HitTest(bitmapLocation, radius);

                    if (cornerIndex != -1 && !touchPoints.ContainsKey(args.Id))
                    {
                        TouchPoint touchPoint = new TouchPoint
                        {
                            CornerIndex = cornerIndex,
                            Offset = bitmapLocation - croppingRect.Corners[cornerIndex]
                        };

                        touchPoints.Add(args.Id, touchPoint);
                    }
                    else
                    {
                        //当你点击字典，我们写的ID和一个变量记录第一点的有效性。
                        if (croppingRect.TestPointInsideSquare(bitmapLocation) && !touchPointsInside.ContainsKey(args.Id))
                        {
                            touchPointsInside.Add(args.Id, bitmapLocation);
                            bitmapLocationfirst = bitmapLocation;
                        }
                    }
                    break;

                case TouchActionType.Moved:
                    if (touchPoints.ContainsKey(args.Id))
                    {
                        TouchPoint touchPoint = touchPoints[args.Id];
                        croppingRect.MoveCorner(touchPoint.CornerIndex, bitmapLocation - touchPoint.Offset);
                        InvalidateSurface();
                    }
                    if (touchPointsInside.ContainsKey(args.Id))
                    {

                        bitmapLocationlast = bitmapLocation;
                        SKPoint point = new SKPoint(bitmapLocationlast.X - bitmapLocationfirst.X, bitmapLocationlast.Y - bitmapLocationfirst.Y);
                        //移动
                        croppingRect.MoveAllCorner(point);
                        bitmapLocationfirst = bitmapLocationlast;
                        InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    if (touchPoints.ContainsKey(args.Id))
                        touchPoints.Remove(args.Id);

                    else if (touchPointsInside.ContainsKey(args.Id))
                        touchPointsInside.Remove(args.Id);
                    break;
            }
        }


        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            base.OnPaintSurface(args);

            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SkiaHelper.backgroundColor);

            // 计算显示位图的矩形
            var rect = SkiaHelper.CalculateRectangle(new SKRect(0, 0, info.Width, info.Height), bitmap);
            canvas.DrawBitmap(bitmap, rect.rect);

            // 计算用于显示裁剪矩形的矩阵变换
            SKMatrix bitmapScaleMatrix = SKMatrix.CreateIdentity();
            SKMatrix.CreateScaleTranslation(rect.scaleX, rect.scaleX, rect.rect.Left, rect.rect.Top);

            // 显示矩形
            SKRect scaledCropRect = bitmapScaleMatrix.MapRect(croppingRect.Rect);


            using (SKPaint edgeStroke = new SKPaint())
            {
                edgeStroke.Style = SKPaintStyle.Stroke;
                edgeStroke.Color = SKColors.White;
                edgeStroke.StrokeWidth = 3;
                edgeStroke.IsAntialias = true;
                canvas.DrawRect(scaledCropRect, edgeStroke);
            }

            canvas.DrawSurrounding(rect.rect, scaledCropRect, SKColors.Gray.WithAlpha(190));

            // Display heavier corners
            using (SKPaint cornerStroke = new SKPaint())
            using (SKPath path = new SKPath())
            {
                cornerStroke.Style = SKPaintStyle.Stroke;
                cornerStroke.Color = SKColors.White;
                cornerStroke.StrokeWidth = 7;

                path.MoveTo(scaledCropRect.Left, scaledCropRect.Top + corner);
                path.LineTo(scaledCropRect.Left, scaledCropRect.Top);
                path.LineTo(scaledCropRect.Left + corner, scaledCropRect.Top);

                path.MoveTo(scaledCropRect.Right - corner, scaledCropRect.Top);
                path.LineTo(scaledCropRect.Right, scaledCropRect.Top);
                path.LineTo(scaledCropRect.Right, scaledCropRect.Top + corner);

                path.MoveTo(scaledCropRect.Right, scaledCropRect.Bottom - corner);
                path.LineTo(scaledCropRect.Right, scaledCropRect.Bottom);
                path.LineTo(scaledCropRect.Right - corner, scaledCropRect.Bottom);

                path.MoveTo(scaledCropRect.Left + corner, scaledCropRect.Bottom);
                path.LineTo(scaledCropRect.Left, scaledCropRect.Bottom);
                path.LineTo(scaledCropRect.Left, scaledCropRect.Bottom - corner);

                canvas.DrawPath(path, cornerStroke);
            }

            // 反转变换以进行触摸跟踪
            bitmapScaleMatrix.TryInvert(out inverseBitmapMatrix);
        }

        private void Rotate()
        {
            var rotatedBitmap = new SKBitmap(bitmap.Height, bitmap.Width);

            using (var canvas = new SKCanvas(rotatedBitmap))
            {
                canvas.Translate(rotatedBitmap.Width, 0);
                canvas.RotateDegrees(90);
                canvas.DrawBitmap(bitmap, 0, 0);
            }

            bitmap.Dispose();
            bitmap = rotatedBitmap;

            SetAspectRatio(aspectRatio);
        }

        private void SetAspectRatio(float? aspectRatio = null, bool isFullRect = false)
        {
            this.aspectRatio = aspectRatio;
            croppingRect.SetRect(new SKRect(0, 0, bitmap.Width, bitmap.Height), aspectRatio, isFullRect);
            InvalidateSurface();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    aspectRatio = null;
                    touchPoints?.Clear();
                    touchPoints = null;
                    touchPointsInside?.Clear();
                    touchPointsInside = null;
                }

                bitmap?.Dispose();
                bitmap = null;
                disposedValue = true;
            }
        }

        ~ImageCropperCanvasView()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
