using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Common
{

    public partial class AddAllocationProductPage : BaseContentPage<AddAllocationProductPageViewModel>
    {
        public AddAllocationProductPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
