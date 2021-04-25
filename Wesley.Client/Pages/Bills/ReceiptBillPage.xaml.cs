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
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Content == null)
            {
                Device.StartTimer(TimeSpan.FromSeconds(0), () =>
                {
                    try
                    {
                        InitializeComponent();
                        ToolbarItems.Clear();

                        string btnIco = "\uf0c7";
                        foreach (var toolBarItem in this.GetToolBarItems<ReceiptBillPageViewModel>(ViewModel, true, btnIco).ToList())
                        {
                            ToolbarItems.Add(toolBarItem);
                        }
                        //Refresh();
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                    return false;
                });
                return;
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
