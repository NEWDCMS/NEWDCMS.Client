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

                On<Android>().SetToolbarPlacement(value: ToolbarPlacement.Bottom);
                //设置该元素是否有导航栏
                NavigationPage.SetHasNavigationBar(this, false);
                //设置该元素是否有返回键
                NavigationPage.SetHasBackButton(this, false);
            }
            catch (System.Exception ex)
            {
                DisplayAlert("MainLayoutPage", ex.Message, "ok");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}

