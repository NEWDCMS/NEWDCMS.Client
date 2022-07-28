using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
namespace Wesley.Client.Pages.Common
{
    public partial class SelectProductPage : BaseContentPage<SelectProductPageViewModel>
    {
        public SelectProductPage()
        {
            try
            {
                InitializeComponent();

                //计算DockRight占位宽度
                this.DockRight.WidthRequest = App.ScreenWidth - 100;
                ToolbarItems?.Clear();
                //string btnIco = "保存";
                foreach (var toolBarItem in this.GetProductToolBarItems(ViewModel, true).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //}
    }
}
