using Xamarin.Forms;
namespace Wesley.Client.Pages.DataTemplates
{
    public class BaseTemplate : ContentView
    {

        #region ParentContext
        public static readonly BindableProperty ParentContextProperty = BindableProperty.Create(nameof(ParentContext), typeof(object), typeof(BaseTemplate), propertyChanged: (obj, old, newV) =>
        {
            var me = obj as BaseTemplate;
            if (newV != null && !(newV is object)) return;
            var oldParentContext = old;
            var newParentContext = newV;
            me?.ParentContextChanged(oldParentContext, newParentContext);
        });

        private void ParentContextChanged(object oldParentContext, object newParentContext)
        {
        }

        /// <summary>
        /// A bindable property
        /// </summary>
        public object ParentContext
        {
            get => GetValue(ParentContextProperty);
            set => SetValue(ParentContextProperty, value);
        }
        #endregion


    }
}
