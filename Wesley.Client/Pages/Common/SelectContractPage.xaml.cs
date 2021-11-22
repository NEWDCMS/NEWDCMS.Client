using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Common
{

    public partial class SelectContractPage : BaseContentPage<SelectContractPageViewModel>
    {
        public SelectContractPage()
        {
            try
            {
                InitializeComponent();
                ToolbarItems?.Clear();
                ToolbarItems.Add(PageExtensions.GetRefreshItem(ViewModel));
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
    }
}
