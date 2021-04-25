using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;

namespace Wesley.Client.Pages.Bills
{

    public partial class ReturnBillPage : BaseContentPage<ReturnBillPageViewModel>
    {

        #region Overrides

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Content == null)
            {
                try
                {
                    InitializeComponent();
                    //存储记录
                    NeedOverrideSoftBackButton = true;
                    ToolbarItems.Clear();
                    string btnIco = "\uf0c7";
                    if (ViewModel.ReferencePage.Equals("UnDeliveryPage") || ViewModel.DispatchItem != null) { btnIco = "签收"; }
                    foreach (var toolBarItem in this.GetToolBarItems(ViewModel, true, btnIco).ToList())
                    {
                        ToolbarItems.Add(toolBarItem);
                    }
                }
                catch (Exception ex) { Crashes.TrackError(ex); }
                return;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        #endregion

    }
}
