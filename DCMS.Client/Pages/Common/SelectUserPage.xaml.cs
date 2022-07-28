using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Windows.Input;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Common
{

    public partial class SelectUserPage : BaseContentPage<SelectUserPageViewModel>
    {
        public SelectUserPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }

        /// <summary>
        /// 搜索客户输入更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SearchBar.TextChanged -= SearchBar_TextChanged;
                if (e.NewTextValue != e.OldTextValue)
                {
                    SearchBar.Text = e.NewTextValue;
                }

                if (ViewModel != null)
                {
                    ViewModel.Filter.SerchKey = SearchBar.Text;
                    ((ICommand)ViewModel.Load)?.Execute(null);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                SearchBar.TextChanged += SearchBar_TextChanged;
            }
        }
    }
}
