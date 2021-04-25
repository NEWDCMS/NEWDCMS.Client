using Prism;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace Wesley.Behaviors
{
    /// <summary>
    /// 此行为在激活其可视元素时延迟加载内容
    /// </summary>
    /// <typeparam name="TVisualElement">A visual element</typeparam>
    public abstract class LazyLoadBehaviorBase<TVisualElement> : Behavior<TVisualElement>
        where TVisualElement : VisualElement
    {
        public DataTemplate LoadingTemplate { get; set; }

        public DataTemplate ContentTemplate { get; set; }

        protected override void OnAttachedTo(TVisualElement element)
        {
            base.OnAttachedTo(element);

            if (element is IActiveAware activeAwareElement)
                activeAwareElement.IsActiveChanged += OnIsActiveChanged;
        }

        protected override void OnDetachingFrom(TVisualElement element)
        {
            if (element is IActiveAware activeAwareElement)
                activeAwareElement.IsActiveChanged -= OnIsActiveChanged;

            base.OnDetachingFrom(element);
        }

        void OnIsActiveChanged(object sender, EventArgs e)
        {
            var element = (TVisualElement)sender;
            element.Behaviors.Remove(this);

            if (LoadingTemplate != null)
            {
                var loadingView = (View)LoadingTemplate.CreateContent();
                SetContent(element, loadingView);
                 
                Task.Run(() =>
                {
                    var contentView = (View)ContentTemplate.CreateContent();
                    Device.BeginInvokeOnMainThread(() => SetContent(element, contentView));
                });
            }
            else
            {
                var contentView = (View)ContentTemplate.CreateContent();
                SetContent(element, contentView);
            }
        }

        protected abstract void SetContent(TVisualElement element, View contentView);
    }
}
