using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Wesley.Client.Pages
{
    public partial class MainLayoutPage : Xamarin.Forms.TabbedPage
    {
        public MainLayoutPage()
        {
            try
            {
                InitializeComponent();
                DeviceDisplay.KeepScreenOn = true;
                this.On<Android>().DisableSwipePaging();
                this.On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
                NavigationPage.SetHasNavigationBar(this, false);
                NavigationPage.SetHasBackButton(this, false);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

                /*
                 
                 */
            }
        }
    }
}

