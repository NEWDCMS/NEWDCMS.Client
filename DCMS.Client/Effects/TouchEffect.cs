using System.Linq;
using Xamarin.Forms;

namespace Wesley.Client.Effects
{
    /// <summary>
    /// 关联 - TouchEffectPlatform
    /// </summary>
    public static class TouchEffect
    {

        public static readonly BindableProperty ColorProperty =
            BindableProperty.CreateAttached(
                "Color",
                typeof(Color),
                typeof(TouchEffect),
                Color.Default,
                propertyChanged: PropertyChanged
            );

        public static void SetColor(BindableObject view, Color value)
        {
            view.SetValue(ColorProperty, value);
        }

        public static Color GetColor(BindableObject view)
        {
            return (Color)view.GetValue(ColorProperty);
        }

        private static void PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is View view))
                return;

            var eff = view.Effects.FirstOrDefault(e => e is TouchRoutingEffect);
            if (GetColor(bindable) != Color.Default)
            {
                view.InputTransparent = false;

                if (eff != null)
                    return;

                view.Effects.Add(new TouchRoutingEffect());
                if (EffectsConfig.AutoChildrenInputTransparent && bindable is Layout &&
                    !EffectsConfig.GetChildrenInputTransparent(view))
                {
                    EffectsConfig.SetChildrenInputTransparent(view, true);
                }
            }
            else
            {
                if (eff == null || view.BindingContext == null) return;
                view.Effects.Remove(eff);
                if (EffectsConfig.AutoChildrenInputTransparent && bindable is Layout &&
                    EffectsConfig.GetChildrenInputTransparent(view))
                {
                    EffectsConfig.SetChildrenInputTransparent(view, false);
                }
            }
        }
    }
    public class TouchRoutingEffect : RoutingEffect
    {
        public event TouchActionEventHandler TouchAction;
        public bool Capture { set; get; }
        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }

        public TouchRoutingEffect() : base($"Wesley.Client.{nameof(TouchEffect)}") { }
    }
}