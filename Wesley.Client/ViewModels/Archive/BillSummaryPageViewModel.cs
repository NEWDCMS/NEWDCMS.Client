using Wesley.Client.CustomViews;
using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Pages;
using Wesley.Client.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;

namespace Wesley.Client.ViewModels
{
    public class BillSummaryPageViewModel : ViewModelBase, IMyTabbedPageSelectedTab
    {
        [Reactive] public int SelectedTab { get; set; }

        public BillSummaryPageViewModel(INavigationService navigationService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "单据汇总";

            //绑定页面菜单
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                //TODAY
                { MenuEnum.TODAY,(m,vm)=> {
                    var key = string.Format(Constants.MENU_KEY, SelectedTab);
                    MessageBus.Current.SendMessage((MenuEnum)m.Id, key.ToUpper());
                } },
                  //YESTDAY
                { MenuEnum.YESTDAY,(m,vm)=> {
                          var key = string.Format(Constants.MENU_KEY, SelectedTab);
                      MessageBus.Current.SendMessage((MenuEnum)m.Id, key.ToUpper());
                } },
                  //OTHER
                { MenuEnum.OTHER,(m,vm)=> {
                         var key = string.Format(Constants.MENU_KEY, SelectedTab);
                      MessageBus.Current.SendMessage((MenuEnum)m.Id, key.ToUpper());
                } },
                  //CLEARHISTORY
                { MenuEnum.CLEARHISTORY,(m,vm)=> {
                      var key = string.Format(Constants.MENU_KEY, SelectedTab);
                      MessageBus.Current.SendMessage((MenuEnum)m.Id, key.ToUpper());
                } },
            });
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            //控制显示菜单
            _popupMenu?.Show(8, 9, 14, 21);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("SelectedTab"))
            {
                //parameters.TryGetValue<int>("Product", out int SelectedTab);
                //SelectedTab = parameters.GetValue<int>("SelectedTab");
            }
        }

        public void SetSelectedTab(int tabIndex) { }
    }
}
