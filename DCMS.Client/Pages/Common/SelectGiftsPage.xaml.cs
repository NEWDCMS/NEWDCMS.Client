using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Common
{

    public partial class SelectGiftsPage : BaseContentPage<SelectGiftsPageViewModel>
    {
        public SelectGiftsPage()
        {
            try
            {
                InitializeComponent();
                NeedOverrideSoftBackButton = true;
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
