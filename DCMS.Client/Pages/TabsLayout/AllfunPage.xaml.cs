using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;

namespace Wesley.Client.Pages
{

    public partial class AllfunPage : BaseContentPage<AllfunPageViewModel>
    {
        public AllfunPage()
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
