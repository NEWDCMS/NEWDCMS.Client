using DCMS.Client.Models.Products;
using DCMS.Client.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Rg.Plugins.Popup.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace DCMS.Client.ViewModels
{
    public class ProductCategoryPageViewModel : ViewModelBase
    {
        public IReactiveCommand ComplatedCommand { get; }
        public IReactiveCommand SelectedCommand { get; }
        public IReactiveCommand InitCatagory { get; }

        private readonly IProductService _productService;

        [Reactive] public new ObservableCollection<CategoryModel> BindCategories { get; set; } = new ObservableCollection<CategoryModel>();

        public ProductCategoryPageViewModel(INavigationService navigationService,
            IDialogService dialogService
            ,
            IProductService productService
            ) : base(navigationService, dialogService)
        {
            _productService = productService;

            //初始类别
            this.InitCatagory = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await _productService.GetAllCategoriesAsync(true, new System.Threading.CancellationToken());
                if (result != null && result.Any())
                {
                    var categories = result.ToList();
                    if (categories != null && categories.Any())
                    {
                        foreach (var op in categories)
                        {
                            op.SelectedCommand = ReactiveCommand.Create<int>(r =>
                            {
                                op.Selected = !op.Selected;
                            });
                        }
                        this.BindCategories = new ObservableCollection<CategoryModel>(categories);
                    }
                }
            },
            this.WhenAny(x => x.BindCategories, x => x.GetValue().Count == 0));

            //确认选择
            this.ComplatedCommand = ReactiveCommand.Create(async () =>
            {
                var selects = BindCategories.Where(s => s.Selected).ToList();
                if (selects.Count > 0)
                {
                    MessageBus.Current.SendMessage(selects, string.Format(Constants.PRODUCT_MENU_KEY, "ProductCategoryPageViewModel"));

                    if (PopupNavigation.Instance.PopupStack.Count > 0)
                        await PopupNavigation.Instance.PopAllAsync();
                }
                else
                {
                    _dialogService?.ShortAlert("请选择项目！");
                }
            });
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            //载入类别
            ((ICommand)InitCatagory)?.Execute(null);
        }
    }
}
