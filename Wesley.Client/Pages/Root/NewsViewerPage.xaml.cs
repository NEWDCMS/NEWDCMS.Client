using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace Wesley.Client.Pages
{

    public partial class NewsViewerPage : BaseContentPage<NewsViewerPageViewModel>
    {
        private bool _loadedPage = false;
        private bool _stopTimer = false;

        public NewsViewerPage()
        {
            try
            {
                InitializeComponent();
                ToolbarItems.Clear();
                var bar = PageExtensions.BulidButton("\uf00d", async () => { await Navigation.PopAsync(); });
                ToolbarItems.Add(bar);
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_loadedPage) return;

            Device.StartTimer(TimeSpan.FromSeconds(0), () =>
            {
                if (!_stopTimer)
                {
                    //Content = new LoadingContentView();
                    _loadedPage = true;
                }

                _stopTimer = false;
                return false;
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _stopTimer = true;
        }

    }
}
