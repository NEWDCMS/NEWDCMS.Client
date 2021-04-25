using Wesley.Client.Models.Sales;
using Wesley.Client.Services;

using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace Wesley.Client.ViewModels
{
    public class ReconciliationProductsPageViewModel : ViewModelBase
    {
        [Reactive] public IList<AccountProductGroup> Products { get; private set; } = new ObservableCollection<AccountProductGroup>();
        [Reactive] public int TotalCount { get; set; }
        [Reactive] public decimal TotalAmount { get; set; }

        public ReconciliationProductsPageViewModel(INavigationService navigationService,


            IDialogService dialogService) : base(navigationService,


                dialogService)
        {
            Title = "销售商品";
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("ReconciliationProducts"))
            {
                parameters.TryGetValue("ReconciliationProducts", out List<AccountProductModel> rconciliationProducts);
                if (rconciliationProducts != null)
                {
                    this.Products.Clear();
                    var gProducts = rconciliationProducts.GroupBy(s => s.CategoryName).ToList();
                    foreach (var group in gProducts)
                    {
                        Products.Add(new AccountProductGroup(group.Key, group.ToList()));
                    }
                    this.TotalCount = rconciliationProducts.Sum(s => s.Quantity);
                    this.TotalAmount = rconciliationProducts.Sum(s => s.Amount);
                }
            }
        }
    }
}
