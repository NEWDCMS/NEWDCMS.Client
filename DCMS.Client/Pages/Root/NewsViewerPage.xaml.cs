using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages
{
    public partial class NewsViewerPage : BaseContentPage<NewsViewerPageViewModel>
    {
        public NewsViewerPage()
        {
            try
            {
                InitializeComponent();
                ToolbarItems?.Clear();
                var bar = PageExtensions.BulidButton("\uf00d", async () =>
                {
                    await Navigation.PopAsync();
                });
                ToolbarItems.Add(bar);
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
