using System;
using System.Linq;
using Xamarin.Forms;

namespace Wesley.Client.Effects
{
    public static class ImageEffect
    {

        public static BindableProperty TintColorProperty =
            BindableProperty.CreateAttached(
                "TintColor",
                typeof(Color),
                typeof(ImageEffect),
                Color.Default,
                propertyChanged: OnTintColorPropertyPropertyChanged);

        public static Color GetTintColor(BindableObject element)
        {
            return (Color)element.GetValue(TintColorProperty);
        }

        public static void SetTintColor(BindableObject element, Color value)
        {
            element.SetValue(TintColorProperty, value);
        }

        private static void OnTintColorPropertyPropertyChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            if (!(bindable is Image))
            {
                //throw new InvalidOperationException("Tint effect is only appliable on CachedImage and Image");
            }

            AttachEffect((View)bindable, (Color)newValue);
        }

        private static void AttachEffect(View element, Color color)
        {
            if (element.Effects.FirstOrDefault(x => x is TintableImageEffect) is TintableImageEffect effect)
            {
                element.Effects.Remove(effect);
            }

            element.Effects.Add(new TintableImageEffect(color));
        }
    }

    public class TintableImageEffect : RoutingEffect
    {
        public TintableImageEffect(Color color) : base($"Wesley.Client.{nameof(TintableImageEffect)}")
        {
            TintColor = color;
        }

        public Color TintColor { get; }
    }
}
