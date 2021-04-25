using System;
using System.Globalization;
using Xamarin.Forms;
namespace Wesley.Client.Converters
{
    public class SizeScalingByScreenConverter : IValueConverter
    {
        public static SizeScalingByScreenConverter Instance { get; } = new SizeScalingByScreenConverter();

        public double Convert(double value)
        {
            return (double)Convert(value, null, null, null);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return PlatformService.ScreenSize switch
            {
                ScreenSize.Small => (double)value,
                ScreenSize.Regular => (double)value * 1.33,
                _ => (double)value * 1.5,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum OS
    {
        Android = 1,
        iOS = 2,
    }

    public enum ScreenSize
    {
        Regular = 0,
        Small = 1,
        Big = 2,
    }

    public static class PlatformService
    {
        public static double DisplayScaleFactor { get; private set; }

        public static Size MainSize { get; private set; }

        public static ScreenSize ScreenSize
        {
            get
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    if (MainSize.Width <= 320)
                    {
                        return ScreenSize.Small;
                    }

                    if (MainSize.Width <= 375)
                    {
                        return ScreenSize.Regular;
                    }

                    return ScreenSize.Big;
                }

                // Android
                if (MainSize.Width <= 384)
                {
                    return ScreenSize.Small;
                }

                if (MainSize.Width <= 540)
                {
                    return ScreenSize.Regular;
                }

                return ScreenSize.Big;
            }
        }

        public static void Initialize(double scaleFactor, double width, double height)
        {
            DisplayScaleFactor = scaleFactor;

            if (width > height)
            {
                var temp = width;
                width = height;
                height = temp;
            }

            MainSize = new Size(width, height);
        }

        public static int DpToPixels(int dp) => (int)(DisplayScaleFactor * dp);

        public static int DpToPixels(double dp) => (int)(DisplayScaleFactor * dp);
    }
}
