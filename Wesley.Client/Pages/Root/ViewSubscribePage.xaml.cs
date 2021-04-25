using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages
{

    public partial class ViewSubscribePage : BaseContentPage<ViewSubscribePageViewModel>
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
                    var item = PageExtensions.BulidButton("\uf1f8", () =>
                    {
                        if (ViewModel != null)
                        {
                            ViewModel.RemoveAllCommand?.Execute(null);
                        }
                    });
                    ToolbarItems.Add(item);
                }
                catch (Exception ex) { Crashes.TrackError(ex); }
                return;
            }
        }

    }
}
