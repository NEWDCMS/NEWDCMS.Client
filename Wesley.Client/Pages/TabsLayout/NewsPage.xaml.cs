using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;


namespace Wesley.Client.Pages
{

    public partial class NewsPage : BaseContentPage<NewsPageViewModel>
    {
        public NewsPage()
        {
            try
            {
                InitializeComponent();
                NavigationPage.SetHasNavigationBar(this, true);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
