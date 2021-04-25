using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using System.Reactive.Linq;



namespace Wesley.Client.Pages.Archive
{

    public partial class MyReportingPage : BaseContentPage<MyReportingPageViewModel>
    {
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Content == null)
            {
                try
                {
                    InitializeComponent();
                    ToolbarItems.Clear();
                    foreach (var toolBarItem in this.GetToolBarItems7<MyReportingPageViewModel>(ViewModel).ToList())
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
    }
}
