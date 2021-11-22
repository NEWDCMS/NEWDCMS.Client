using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Common
{

    public partial class AddProductArchivePage : BaseContentPage<AddProductArchivePageViewModel>
    {
        public AddProductArchivePage()
        {
            try
            {
                InitializeComponent();
                this.SetToolBarItems(ViewModel);
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }




        /// <summary>
        /// 价格计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalcPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.OldTextValue != e.NewTextValue)
            {
                if (ViewModel != null)
                {
                    ViewModel.TextChangedCommend.Execute(null);
                }
            }
        }

    }
}
