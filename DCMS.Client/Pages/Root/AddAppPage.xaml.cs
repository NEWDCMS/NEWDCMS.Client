using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;

namespace Wesley.Client.Pages
{

    public partial class AddAppPage : BaseContentPage<AddAppPageViewModel>
    {
        public AddAppPage()
        {
            try
            {
                InitializeComponent();
                NavigationPage.SetHasNavigationBar(this, true);
                NavigationPage.SetHasBackButton(this, true);
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
