using Xamarin.Forms;

namespace Wesley.Behaviors
{
    /// <summary>
    /// 延迟内容加载
    /// </summary>
    public class LazyLoadContentPageBehavior : LazyLoadBehaviorBase<ContentPage>
    {
        protected override void SetContent(ContentPage page, View contentView)
        {
            if (page != null && contentView != null)
                page.Content = contentView;
        }
    }
}
