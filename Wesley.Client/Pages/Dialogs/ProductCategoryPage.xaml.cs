using Wesley.Client.ViewModels;
using System.Windows.Input;

namespace Wesley.Client.Pages
{
    public partial class ProductCategoryPage : BasePopupPage<ProductCategoryPageViewModel>
    {
        public ProductCategoryPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (this.BindingContext is ProductCategoryPageViewModel vm)
            {
                ((ICommand)vm.InitCatagory)?.Execute(null);
            }
        }
    }
}