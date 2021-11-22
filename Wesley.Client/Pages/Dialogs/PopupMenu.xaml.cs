using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.ViewModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Wesley.Client.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupMenu : BasePopupPage<PopupMenuViewModel>
    {
        private List<SubMenu> MenuItems { get; set; }
        private ViewModelBase VM { get; set; }
        private readonly Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>> KeyValues;

        public PopupMenu(ViewModelBase vm, Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>> keyValues)
        {
            InitializeComponent();

            VM = vm;
            KeyValues = keyValues;
            if (KeyValues != null)
            {
                var lists = new List<SubMenu>();
                foreach (var m in KeyValues)
                {
                    var cur = GlobalSettings.ToolBarMenus
                        .Where(s => (int)m.Key == s.Id)
                        .FirstOrDefault();

                    if (cur != null)
                    {
                        cur.CallBack = m.Value;
                        lists.Add(cur);
                    }
                }
                MenuItems = lists;
            }
        }

        public void Show(params int[] menus)
        {
            if (menus != null)
            {
                var lists = MenuItems.Where(s => menus.Contains(s.Id)).ToList();
                SecondaryToolbarListView.ItemsSource = lists;
                SecondaryToolbarListView.HeightRequest = lists.Count * 38;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();
        }

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();
            return false;
        }

        private async void CloseAllPopup()
        {
            await PopupNavigation.Instance.PopAllAsync();
        }


        private async void SecondaryToolbarListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is SubMenu select)
            {
                select.CallBack?.Invoke(select, VM);
                await PopupNavigation.Instance.RemovePageAsync(this);
            }
        }
    }
}