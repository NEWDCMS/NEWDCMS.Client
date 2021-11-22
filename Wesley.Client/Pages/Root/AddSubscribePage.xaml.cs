using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;

namespace Wesley.Client.Pages
{

    public partial class AddSubscribePage : BaseContentPage<AddSubscribePageViewModel>
    {
        public AddSubscribePage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

    }
}
