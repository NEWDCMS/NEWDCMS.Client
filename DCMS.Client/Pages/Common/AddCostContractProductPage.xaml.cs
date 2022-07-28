using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;

namespace Wesley.Client.Pages.Common
{

    public partial class AddCostContractProductPage : BaseContentPage<AddCostContractProductPageViewModel>
    {
        public AddCostContractProductPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
