using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Common
{

    public partial class EditAllocationProductPage : BaseContentPage<EditAllocationProductPageViewModel>
    {
        public EditAllocationProductPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

        #region Overrides
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        #endregion
    }
}