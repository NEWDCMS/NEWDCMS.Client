using SkiaSharp;
using System;

namespace Wesley.BitImageEditor.ManipulationBitmap
{
    internal class TouchManipulationManager
    {
        public TouchManipulationMode Mode { set; get; }


        /// <summary>
        /// 单指操纵
        /// </summary>
        /// <param name="prevPoint"></param>
        /// <param name="newPoint"></param>
        /// <param name="pivotPoint"></param>
        /// <returns></returns>
        public SKMatrix OneFingerManipulate(SKPoint prevPoint, SKPoint newPoint, SKPoint pivotPoint)
        {
            if (Mode == TouchManipulationMode.None)
            {
                return SKMatrix.CreateIdentity();
            }

            SKMatrix touchMatrix = SKMatrix.CreateIdentity();
            SKPoint delta = newPoint - prevPoint;

            if (Mode == TouchManipulationMode.ScaleDualRotate)  // One-finger rotation
            {
                SKPoint oldVector = prevPoint - pivotPoint;
                SKPoint newVector = newPoint - pivotPoint;

                float scale = Magnitude(newVector) / Magnitude(oldVector);


                // Avoid rotation if fingers are too close to center
                if (Magnitude(newVector) > 30 && Magnitude(oldVector) > 30)
                {
                    float prevAngle = (float)Math.Atan2(oldVector.Y, oldVector.X);
                    float newAngle = (float)Math.Atan2(newVector.Y, newVector.X);

                    // Calculate rotation matrix
                    float angle = newAngle - prevAngle;
                    touchMatrix = SKMatrix.CreateRotation(angle, pivotPoint.X, pivotPoint.Y);

                    // Effectively rotate the old vector
                    float magnitudeRatio = Magnitude(oldVector) / Magnitude(newVector);
                    oldVector.X = magnitudeRatio * newVector.X;
                    oldVector.Y = magnitudeRatio * newVector.Y;

                    // Recalculate delta
                    delta = newVector - oldVector;
                }

                if (!float.IsNaN(scale) && !float.IsInfinity(scale))
                {
                    var tm = SKMatrix.CreateScale(scale, scale, pivotPoint.X, pivotPoint.Y);
                    touchMatrix.PostConcat(tm);
                }
            }

            // Multiply the rotation matrix by a translation matrix
            touchMatrix.PostConcat(SKMatrix.CreateTranslation(delta.X, delta.Y));

            return touchMatrix;
        }


        /// <summary>
        /// 两指操纵
        /// </summary>
        /// <param name="prevPoint"></param>
        /// <param name="newPoint"></param>
        /// <param name="pivotPoint"></param>
        /// <returns></returns>
        public SKMatrix TwoFingerManipulate(SKPoint prevPoint, SKPoint newPoint, SKPoint pivotPoint)
        {
            SKMatrix touchMatrix = SKMatrix.CreateIdentity();
            SKPoint oldVector = prevPoint - pivotPoint;
            SKPoint newVector = newPoint - pivotPoint;

            if (Mode == TouchManipulationMode.ScaleRotate ||
                Mode == TouchManipulationMode.ScaleDualRotate)
            {
                // Find angles from pivot point to touch points
                float oldAngle = (float)Math.Atan2(oldVector.Y, oldVector.X);
                float newAngle = (float)Math.Atan2(newVector.Y, newVector.X);

                // Calculate rotation matrix
                float angle = newAngle - oldAngle;
                touchMatrix = SKMatrix.CreateRotation(angle, pivotPoint.X, pivotPoint.Y);

                // Effectively rotate the old vector
                float magnitudeRatio = Magnitude(oldVector) / Magnitude(newVector);
                oldVector.X = magnitudeRatio * newVector.X;
                oldVector.Y = magnitudeRatio * newVector.Y;
            }

            float scaleX = 1;
            float scaleY = 1;

            if (Mode == TouchManipulationMode.AnisotropicScale)
            {
                scaleX = newVector.X / oldVector.X;
                scaleY = newVector.Y / oldVector.Y;

            }
            else if (Mode == TouchManipulationMode.IsotropicScale || Mode == TouchManipulationMode.ScaleRotate || Mode == TouchManipulationMode.ScaleDualRotate)
            {
                scaleX = scaleY = Magnitude(newVector) / Magnitude(oldVector);
            }

            if (!float.IsNaN(scaleX) && !float.IsInfinity(scaleX) && !float.IsNaN(scaleY) && !float.IsInfinity(scaleY))
            {
                touchMatrix.PostConcat(SKMatrix.CreateScale(scaleX, scaleY, pivotPoint.X, pivotPoint.Y));
            }

            return touchMatrix;
        }

        private float Magnitude(SKPoint point)
        {
            return (float)Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));
        }
    }
}
