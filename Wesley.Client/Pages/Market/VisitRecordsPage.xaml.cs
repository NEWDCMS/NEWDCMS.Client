using Wesley.Client.Models;
using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Archive
{

    [DesignTimeVisible(false)]
    public partial class VisitRecordsPage : BaseContentPage<VisitRecordsPageViewModel>
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
                        foreach (var toolBarItem in this.GetToolBarItems(ViewModel,
                            ("Filter", new FilterModel()
                            {
                                TerminalEnable = true,
                                DistrictEnable = true,
                                BusinessUserEnable = true
                            })).ToList())
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


        private async Task OpenAnimation(View view, uint length = 250)
        {
            view.RotationX = -90;
            view.IsVisible = true;
            view.Opacity = 0;
            _ = view.FadeTo(1, length);
            await view.RotateXTo(0, length);
        }

        private async Task CloseAnimation(View view, uint length = 250)
        {
            _ = view.FadeTo(0, length);
            await view.RotateXTo(-90, length);
            view.IsVisible = false;
        }

        //private async void MainExpander_Tapped(object sender, EventArgs e)
        //{
        //    var expander = sender as Expander;
        //    var imgView = expander.FindByName<Grid>("ImageView");
        //    var detailsView = expander.FindByName<Grid>("DetailsView");

        //    if (expander.IsExpanded)
        //    {
        //        await OpenAnimation(imgView);
        //        await OpenAnimation(detailsView);
        //    }
        //    else
        //    {
        //        await CloseAnimation(detailsView);
        //        await CloseAnimation(imgView);
        //    }
        //}
    }
}
