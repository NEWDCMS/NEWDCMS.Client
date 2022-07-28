using DCMS.Client.iOS;
using DCMS.Client.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(StatusBarImplementation))]
namespace DCMS.Client.iOS
{
    public class StatusBarImplementation : IStatusBar
    {
        public StatusBarImplementation()
        {

        }

        public void HideStatusBar()
        {
            UIApplication.SharedApplication.StatusBarHidden = true;
        }

        public void ShowStatusBar()
        {
            UIApplication.SharedApplication.StatusBarHidden = false;
        }
    }
}