using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Wesley.Client.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : BaseContentPage<HomePageViewModel>
    {
        public HomePage()
        {
            try
            {
                InitializeComponent();
                NavigationPage.SetHasNavigationBar(this, false);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
