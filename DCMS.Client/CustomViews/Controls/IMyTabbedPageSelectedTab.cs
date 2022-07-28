namespace Wesley.Client.CustomViews
{
    public interface IMyTabbedPageSelectedTab
    {
        int SelectedTab { get; set; }
        void SetSelectedTab(int tabIndex);
    }
}
