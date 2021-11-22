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
    public class ViewBillPageViewModel : ViewModelBase, IMyTabbedPageSelectedTab
    {
        [Reactive] public int SelectedTab { get; set; }

        public ViewBillPageViewModel(INavigationService navigationService,
              IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "查看单据";
            _navigationService = navigationService;
            _dialogService = dialogService;

            //绑定页面菜单
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                //TODAY
                { MenuEnum.TODAY,(m,vm)=> {
                        var key = string.Format(Constants.MENU_VIEW_KEY, SelectedTab);
                        MessageBus.Current.SendMessage((MenuEnum)m.Id, key.ToUpper());
                } },
                //YESTDAY
                { MenuEnum.YESTDAY,(m,vm)=> {
                       var key = string.Format(Constants.MENU_VIEW_KEY, SelectedTab);
                       MessageBus.Current.SendMessage((MenuEnum)m.Id, key.ToUpper());
                } },
                //OTHER
                { MenuEnum.OTHER,(m,vm)=> {
                        var key = string.Format(Constants.MENU_VIEW_KEY, SelectedTab);
                        MessageBus.Current.SendMessage((MenuEnum)m.Id, key.ToUpper());
                } },
                //CLEARHISTORY
                { MenuEnum.CLEARHISTORY,(m,vm)=> {
                       var key = string.Format(Constants.MENU_VIEW_KEY, SelectedTab);
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

        public void SetSelectedTab(int tabIndex)
        {
            SelectedTab = tabIndex;
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                if (parameters.ContainsKey("SelectedTab"))
                {
                    parameters.TryGetValue("SelectedTab", out int selectedTab);
                    this.SelectedTab = selectedTab;
                }
            }
            catch (Exception ex)
            {
                //Crashes.TrackError(ex);
            }
        }
    }
}
