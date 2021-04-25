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

        //private void Switch_Toggled(object sender, ToggledEventArgs e)
        //{
        //    try
        //    {
        //        //var control = (Switch)sender;
        //        //var apps = ViewModel?.AppList?.Select(a => a).Where(a => a.Selected == true).ToList();
        //        //Settings.CommonFunctions = JsonConvert.SerializeObject(apps);
        //    }
        //    catch (Exception ex)
        //    {
        //         Crashes.TrackError(ex);
        //    }
        //}
    }
}
