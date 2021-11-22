using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;

namespace Wesley.Client.Pages
{
    public partial class LoginPage : BaseContentPage<LoginPageViewModel>
    {
        public LoginPage()
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
