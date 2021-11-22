using DCMS.Client.Enums;
using DCMS.Client.Models;
using DCMS.SlideOverKit;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
namespace DCMS.Client.Pages
{
    public partial class RightSideMasterPage : SlideMenuView
    {

        [Reactive] public ObservableCollection<SubMenu> MenuItems { get; set; } = new ObservableCollection<SubMenu>();
        private string ViewModelName { get; set; }

        public RightSideMasterPage()
        {
            InitializeComponent();

            this.IsFullScreen = true;
            this.WidthRequest = 200;
            this.MenuOrientations = MenuOrientation.RightToLeft;
            this.BackgroundColor = Color.White;
            this.BackgroundViewColor = Color.FromHex("#000000");
        }

        /// <summary>
        /// 绑定菜单
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="viewModelName"></param>
        public void SetBindMenus(IList<SubMenu> menus, string viewModelName)
        {
            ViewModelName = viewModelName;

            if (menus != null)
            {
                foreach (var op in menus)
                {
                    op.SelectedCommand = ReactiveCommand.Create<MenuEnum>(r =>
                    {
                        this.HideWithoutAnimations();
                        var key = string.Format(Constants.MENU_KEY, this.ViewModelName);
                        MessageBus.Current.SendMessage(r, key.ToUpper());
                        System.Diagnostics.Debug.Print($"{key}----------SendMessage------------->");
                    });
                }
                MenuItems = new ObservableCollection<SubMenu>(menus);
                RepeaterOptions.ItemsSource = MenuItems;
            }
            //RepeaterOptions.HeightRequest = MenuItems.Count * 37;
        }
    }
}

