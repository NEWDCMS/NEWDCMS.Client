using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;

namespace Wesley.Client.Pages.Bills
{

    public partial class SaleOrderBillPage : BaseContentPage<SaleOrderBillPageViewModel>
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
                    ToolbarItems.Clear();
                    //存储记录
                    NeedOverrideSoftBackButton = true;
                    string btnIco = "\uf0c7";
                    foreach (var toolBarItem in this.GetToolBarItems(ViewModel, true, btnIco).ToList())
                    {
                        ToolbarItems.Add(toolBarItem);
                    }
                }
                catch (Exception ex) { Crashes.TrackError(ex); }
                return;
            }
        }

        #endregion

    }
}
