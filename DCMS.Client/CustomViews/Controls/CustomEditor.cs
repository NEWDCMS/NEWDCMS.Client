using Xamarin.Forms;
namespace Wesley.Client.CustomViews
{

    /// <summary>
    /// 自定义多行文本编辑器
    /// </summary>
    public class CustomEditor : Editor
    {
        public new static BindableProperty PlaceholderProperty = BindableProperty.Create(
            nameof(Placeholder),
            typeof(string),
            typeof(CustomEditor), string.Empty);

        public new string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public void InvalidateLayout()
        {
            InvalidateMeasure();
        }
    }
}