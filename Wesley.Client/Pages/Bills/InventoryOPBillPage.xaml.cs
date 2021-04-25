using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Bills
{

    public partial class InventoryOPBillPage : BaseContentPage<InventoryOPBillPageViewModel>
    {
        #region Overrides

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
                        foreach (var toolBarItem in this.GetToolBarItems<InventoryOPBillPageViewModel>(ViewModel, false).ToList())
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

        #endregion

    }
}
