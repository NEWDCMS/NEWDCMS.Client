using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Windows.Input;
namespace Wesley.Client.Pages
{

    public partial class AddReportPage : BaseContentPage<AddReportPageViewModel>
    {
        public AddReportPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        public ICommand GotoHomeCmd { protected set; get; }


    }
}
