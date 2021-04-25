namespace Wesley.Client.Paging
{
    public interface IInfiniteListLoader
    {
        /// <summary>
        /// 此方法必须由负责显示数据的UI元素调用。
        /// 例如，在android上，滚动监听器可以引用IInfiniteListLoader并从on scroll调用它。
        /// 此方法的实现执行时间必须是透明的，因为它应该立即返回，并且不会阻止调用方。
        /// </summary>
        /// <param name="lastVisibleIndex">Index of the last visible item.</param>
        void OnScroll(int lastVisibleIndex);
    }
}