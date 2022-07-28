using Xamarin.Forms;

namespace DCMS.Behaviors
{
    /// <summary>
    /// 延迟内容视图加载
    /// </summary>
    public class LazyLoadContentViewBehavior : LazyLoadBehaviorBase<ContentView>
    {
        protected override void SetContent(ContentView element, View contentView)
        {
            if (element != null && contentView != null)
                element.Content = contentView;
        }
    }
}
