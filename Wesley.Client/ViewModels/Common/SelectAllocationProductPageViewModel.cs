using Acr.UserDialogs;
using Wesley.Client.Models;
using Wesley.Client.Models.Products;
using Wesley.Client.Services;

using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class SelectAllocationProductPageViewModel : ViewModelBaseCutom
    {
        [Reactive] public IList<MenuBinding> MathodSeries { get; set; } = new ObservableCollection<MenuBinding>();
        [Reactive] public IReactiveCommand SelectCommand { get; set; }
        [Reactive] public MenuBinding Selecter { get; set; }

        public SelectAllocationProductPageViewModel(INavigationService navigationService,
            IProductService productService,
            ITerminalService terminalService,
            IUserService userService,
            IWareHousesService wareHousesService,
            IAccountingService accountingService,


            IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "加载调拨商品";


            //载入商品
            this.Load = ReactiveCommand.Create(() =>
            {
                var lists = new List<MenuBinding>()
                {
                     new MenuBinding(){Id=0, Name="获取仓库今天销售的商品" },
                     new MenuBinding(){Id=1, Name="获取仓库昨天的销售商品" },
                     new MenuBinding(){Id=2, Name="获取仓库近2天的销售商品" },
                     new MenuBinding(){Id=3, Name="获取仓库上次调拨的销售商品" },
                     new MenuBinding(){Id=4, Name="获取仓库今天退货的商品" },
                     new MenuBinding(){Id=5, Name="获取仓库昨天退货的商品" },
                     new MenuBinding(){Id=6, Name="获取仓库前天退货的商品" },
                     new MenuBinding(){Id=7, Name="获取库存所在指定类别的商品" },
                };
                this.MathodSeries = new ObservableCollection<MenuBinding>(lists);
            });


            //类别选择
            this.SelectCommand = ReactiveCommand.Create<object>(async e =>
            {
                await SelectCatagory((data) =>
                 {
                     Filter.CatagoryId = data.Id;
                     Filter.CatagoryName = data.Name;
                 });
            });

            //编辑项目
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
           .Skip(1)
           .Where(x => x != null)
           .SubOnMainThread(async menuType =>
           {
               int? type = menuType?.Id;
               List<int> categoryIds = new List<int> { Filter.CatagoryId };
               int wareHouseId = Filter.WareHouseId;

               if (wareHouseId == 0)
               {
                   _dialogService.ShortAlert("仓库未指定");
                   return;
               }

               if (type.HasValue)
               {
                   using (UserDialogs.Instance.Loading("提取中..."))
                   {
                       var result = await _productService.GetAllocationProductsAsync(type, categoryIds.ToArray(), wareHouseId, this.ForceRefresh);
                       if (result != null)
                       {
                           ProductSeries = new ObservableCollection<ProductModel>(result);
                       }
                   };
                   await _navigationService.GoBackAsync(new NavigationParameters() { { "ProductSeries", ProductSeries } });
               }
               else
               {
                   await _navigationService.GoBackAsync();
               }
           }, ex => _dialogService.ShortAlert(ex.ToString()));

            this.SelectCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.ExceptionsSubscribe();
            this.BindBusyCommand(Load);
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
