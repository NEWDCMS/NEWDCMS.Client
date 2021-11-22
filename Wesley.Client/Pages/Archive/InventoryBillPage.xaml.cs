using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages.Archive
{

    public partial class InventoryBillPage : BaseContentPage<InventoryBillPageViewModel>
    {
        public InventoryBillPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }


        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    if (Content == null)
        //    {
        //        Device.StartTimer(TimeSpan.FromSeconds(0), () =>
        //        {
        //            try
        //            {
        //                InitializeComponent();
        //            }
        //            catch (Exception ex) { Crashes.TrackError(ex); }
        //            return false;
        //        });
        //        return;
        //    }
        //}
    }
}
