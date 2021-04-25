using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Archive
{

    public partial class OrderQuantityAnalysisPage : BaseContentPage<OrderQuantityAnalysisPageViewModel>
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
                        ToolbarItems.Clear();

                        foreach (var toolBarItem in this.GetToolBarItems6(ViewModel,
                            ("Filter", new Models.FilterModel()
                            {
                                BusinessUserEnable = true,
                                BrandEnable = true,
                                ProductEnable = true,
                                CatagoryEnable = true
                            }
                        )).ToList())
                        {
                            ToolbarItems.Add(toolBarItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                    //=============================
                    return false;
                });
                return;
            }

        }
    }
}
