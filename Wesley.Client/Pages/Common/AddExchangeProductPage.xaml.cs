using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Common
{

    public partial class AddExchangeProductPage : BaseContentPage<AddExchangeProductPageViewModel>
    {
        public AddExchangeProductPage()
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

