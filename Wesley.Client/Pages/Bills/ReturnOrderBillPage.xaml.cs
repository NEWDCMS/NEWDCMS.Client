using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Bills
{

    public partial class ReturnOrderBillPage : BaseContentPage<ReturnOrderBillPageViewModel>
    {
        #region Overrides 页面显示前自定义操作
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
                        foreach (var toolBarItem in this.GetToolBarItems<ReturnOrderBillPageViewModel>(ViewModel, true).ToList())
                        {
                            ToolbarItems.Add(toolBarItem);
                        }
                        //Refresh();
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
