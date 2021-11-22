using Xamarin.Forms;
namespace Wesley.Client.CustomViews
{
    /// <summary>
    /// 用于Toolbar绑定项目
    /// </summary>
    public class BindableToolbarItem : ToolbarItem
    {
        public static readonly BindableProperty IsVisibleProperty =
            BindableProperty.Create("BindableToolbarItem", typeof(bool), typeof(ToolbarItem),
                true, BindingMode.TwoWay, propertyChanged: OnIsVisibleChanged);

        public BindableToolbarItem()
        {
            InitVisibility();
        }
        /// <summary>
        /// 1表示在倒数第二的位置添加
        /// </summary>
        public int OrderIndex { get; set; }
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        private void InitVisibility()
        {
            OnIsVisibleChanged(this, false, IsVisible);
        }

        private static void OnIsVisibleChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var item = bindable as BindableToolbarItem;

            if (item != null && item.Parent == null)
                return;

            if (item != null)
            {
                var items = ((ContentPage)item.Parent).ToolbarItems;

                if ((bool)newvalue && !items.Contains(item))
                {
                    if (item.OrderIndex == -1)
                    {
                        Device.BeginInvokeOnMainThread(() => { items.Insert(0, item); });
                    }
                    else if (item.OrderIndex == 1)
                    {
                        int insertIndex = 0;
                        if (items.Count > 0)
                        {
                            insertIndex = items.Count - 1;
                        }
                        Device.BeginInvokeOnMainThread(() => { items.Insert(insertIndex, item); });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() => { items.Add(item); });
                    }
                }
                else if (!(bool)newvalue && items.Contains(item))
                {
                    Device.BeginInvokeOnMainThread(() => { items.Remove(item); });
                }
            }
        }
    }
}
