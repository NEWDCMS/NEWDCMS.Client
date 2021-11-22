using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Common
{

    public partial class AddGiftProductPage : BaseContentPage<AddGiftProductPageViewModel>
    {
        public AddGiftProductPage()
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

