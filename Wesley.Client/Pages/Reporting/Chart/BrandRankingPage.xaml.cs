using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Reporting
{

    public partial class BrandRankingPage : BaseContentPage<BrandRankingPageViewModel>
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
                        try
                        {
                            InitializeComponent();
                            ToolbarItems.Clear();
                            foreach (var toolBarItem in this.GetToolBarItems(ViewModel, ("Filter", new Models.FilterModel()
                            {
                                BrandEnable = true,
                                BusinessUserEnable = true
                            })).ToList())
                            {
                                ToolbarItems.Add(toolBarItem);
                            }

                        }
                        catch (Exception ex) { Crashes.TrackError(ex); }


                        ////Chart
                        //chart.DescriptionChart.Text = "";
                        //chart.DrawBorders = false;

                        //chart.AxisLeft.DrawGridLines = false;
                        //chart.AxisLeft.DrawAxisLine = false;
                        //chart.AxisLeft.Enabled = true;

                        //chart.AxisRight.DrawAxisLine = false;
                        //chart.AxisRight.DrawGridLines = false;
                        //chart.AxisRight.Enabled = false;
                    }
                    catch (Exception ex) { Crashes.TrackError(ex); }
                    return false;
                });
                return;
            }

        }
    }
}
