using Wesley.Client.Models.Products;
using Wesley.Client.Services;
using Wesley.SlideOverKit;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
namespace Wesley.Client.Pages
{
    public partial class RightProductCategoryMasterPage : SlideMenuView
    {
        [Reactive] public ObservableCollection<CategoryModel> MenuItems { get; set; } = new ObservableCollection<CategoryModel>();
        private string ViewModelName { get; set; }

        private readonly IDialogService _dialogService;
        public RightProductCategoryMasterPage()
        {
            InitializeComponent();
            this.IsFullScreen = true;
            this.WidthRequest = 200;
            this.MenuOrientations = MenuOrientation.RightToLeft;
            this.BackgroundColor = Color.White;//Color.Transparent;
            this.BackgroundViewColor = Color.FromHex("#000000");
            _dialogService = App.Resolve<IDialogService>();
        }


        public void SetBindMenus(IList<CategoryModel> categories, string viewModelName)
        {
            ViewModelName = viewModelName;
            if (categories != null)
            {
                foreach (var op in categories)
                {
                    op.SelectedCommand = ReactiveCommand.Create<int>(r =>
                    {
                        op.Selected = !op.Selected;
                    });
                }
                MenuItems = new ObservableCollection<CategoryModel>(categories);
                RepeaterOptions.ItemsSource = MenuItems;
            }
            //RepeaterOptions.HeightRequest = MenuItems.Count * 37;
        }


        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var selects = MenuItems.Where(s => s.Selected).ToList();
            if (selects.Count > 0)
            {
                this.HideWithoutAnimations();
                MessageBus.Current.SendMessage(selects, string.Format(Constants.PRODUCT_MENU_KEY, this.ViewModelName));
            }
            else
            {
                _dialogService?.ShortAlert("请选择项目！");
            }
        }
    }
}

