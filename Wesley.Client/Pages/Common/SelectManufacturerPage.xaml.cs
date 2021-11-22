using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Windows.Input;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Common
{

    public partial class SelectManufacturerPage : BaseContentPage<SelectManufacturerPageViewModel>
    {
        public SelectManufacturerPage()
        {
            try
            {
                InitializeComponent();
                ToolbarItems?.Clear();
                ToolbarItems.Add(PageExtensions.GetRefreshItem(ViewModel));
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
                    ((ICommand)ViewModel.Load)?.Execute(null);
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
