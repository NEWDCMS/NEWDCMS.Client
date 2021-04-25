using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Archive
{

    public partial class PurchaseOrderBillPage : BaseContentPage<PurchaseOrderBillPageViewModel>
    {
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Content == null)
            {
                Device.StartTimer(TimeSpan.FromSeconds(0), () =>
                {
                    try
                    {
                        InitializeComponent();
                        //存储记录
                        NeedOverrideSoftBackButton = true;
                        ToolbarItems.Clear();
                        foreach (var toolBarItem in this.GetToolBarItems<PurchaseOrderBillPageViewModel>(ViewModel, true).ToList())
                        {
                            ToolbarItems.Add(toolBarItem);
                        }
                    }
                    catch (Exception ex) { Crashes.TrackError(ex); }
                    return false;
                });
                return;
            }
        }
    }
}
