using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms.Xaml;

namespace Wesley.Client.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraViewPage : BaseContentPage<CameraPageViewModel>
    {
        public CameraViewPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}