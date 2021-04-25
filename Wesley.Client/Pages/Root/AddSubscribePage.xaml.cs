using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Windows.Input;

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

        public ICommand GotoHomeCmd { protected set; get; }
        //private void Switch_Toggled(object sender, ToggledEventArgs e)
        //{
        //    try
        //    {
        //        //var control = (Switch)sender;
        //        var apps = ViewModel?.AppList?.Select(a => a).Where(a => a.Selected).ToList();
        //        if (apps != null && apps.Any())
        //            Settings.CommonSubscribeDatas = JsonConvert.SerializeObject(apps);
        //    }
        //    catch (Exception ex)
        //    {
        //         Crashes.TrackError(ex);
        //    }
        //}
    }
}
