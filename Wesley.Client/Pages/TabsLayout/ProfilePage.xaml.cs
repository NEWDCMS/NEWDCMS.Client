using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;

namespace Wesley.Client.Pages
{
    public partial class ProfilePage : BaseContentPage<ProfilePageViewModel>
    {
        public ProfilePage()
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
