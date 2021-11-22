using Wesley.Client.Models.Products;
using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
namespace Wesley.Client.Pages.Common
{

    public partial class SelectAllocationProductPage : BaseContentPage<SelectAllocationProductPageViewModel>
    {
        public SelectAllocationProductPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex) { Crashes.TrackError(ex); }

        }

        /// <summary>
        /// 选择商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is ProductModel item)
            {
                if (item.StockQty.Value == 0)
                {
                    ViewModel.Alert("零库存商品无效！");
                    return;
                }
                item.Selected = !item.Selected;
            }
        }
    }
}
