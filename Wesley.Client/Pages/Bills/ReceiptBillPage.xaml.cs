using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Bills
{

    public partial class ReceiptBillPage : BaseContentPage<ReceiptBillPageViewModel>
    {
        public ReceiptBillPage()
        {
            try
            {
                InitializeComponent();
               

                ToolbarItems?.Clear();
                string btnIco = "保存";
                foreach (var toolBarItem in this.GetToolBarItems(ViewModel, true, btnIco).ToList())
                {
                    ToolbarItems.Add(toolBarItem);
                }
                //Refresh();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void CustomEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != e.OldTextValue)
            {
                ((ICommand)ViewModel.TextChangedCommand)?.Execute(null);
            }
        }
    }
}
