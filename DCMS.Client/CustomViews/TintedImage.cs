using Xamarin.Forms;
namespace Wesley.Client.Plugin
{

    /// <summary>
    /// 自定义着色图像
    /// </summary>
    public class TintedImage : Image
    {
        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(TintedImage), Color.Black);

        public Color TintColor
        {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }
    }
}
