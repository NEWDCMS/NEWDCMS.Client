using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;

namespace Wesley.Client.Pages.Bills
{

    public partial class SaleBillPage : BaseContentPage<SaleBillPageViewModel>
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
                    foreach (var toolBarItem in this.GetToolBarItems(ViewModel, true, "\uf0c7").ToList())
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
