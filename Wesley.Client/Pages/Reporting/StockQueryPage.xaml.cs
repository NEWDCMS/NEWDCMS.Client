using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using Xamarin.Forms;

namespace Wesley.Client.Pages.Reporting
{

    public partial class StockQueryPage : BaseContentPage<StockQueryPageViewModel>
    {
        public StockQueryPage()
        {
            try
            {
                InitializeComponent();
                ToolbarItems?.Clear();
                foreach (var toolBarItem in this.GetToolBarItems(ViewModel, showSubMit: false, showPrint: false).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }
            catch (Exception ex) { Crashes.TrackError(ex); }
        }
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SerchKey.TextChanged -= SearchBar_TextChanged;
                if (e.NewTextValue != e.OldTextValue)
                {
                    SerchKey.Text = e.NewTextValue;
                }
                ((System.Windows.Input.ICommand)ViewModel.Load)?.Execute(null);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                SerchKey.TextChanged += SearchBar_TextChanged;
            }
        }
    }
}
