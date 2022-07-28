using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
namespace Wesley.Client.Pages
{

    public partial class FeedbackPage : BaseContentPage<FeedbackPageViewModel>
    {
        public FeedbackPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }


    }
}
