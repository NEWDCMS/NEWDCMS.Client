using System;

using Xamarin.Forms;
using System.Collections.Generic;

namespace Wesley.SlideOverKit
{
    public class MenuContainerPage : ContentPage, IMenuContainerPage
    {
        public MenuContainerPage ()
        {
            //PopupViews = new Dictionary<string, SlidePopupView> ();
        }

        SlideMenuView slideMenu;
        public SlideMenuView SlideMenu
        {
            get
            {
                return slideMenu;
            }
            set
            {
                if (slideMenu != null)
                    slideMenu.Parent = null;

                slideMenu = value;

                if (slideMenu != null)
                    slideMenu.Parent = this;
            }
        }

        public Action ShowMenuAction { get; set; }

        public Action HideMenuAction { get; set; }

        public void ShowMenu ()
        {
            if (SlideMenu.IsShown)
            {
                HideMenuAction?.Invoke();
            }
            else
            {
                ShowMenuAction?.Invoke();
            }
        }

        public void HideMenu ()
        {
            HideMenuAction?.Invoke();
        }


        public virtual void OnSoftBackButtonPressed()
        {
        }
        public virtual bool NeedOverrideSoftBackButton { get; set; } = false;
    }


    public class MenuTabbedPage : TabbedPage, IMenuContainerPage, IPopupContainerPage
    {
        public MenuTabbedPage()
        {
            PopupViews = new Dictionary<string, SlidePopupView>();
        }

        SlideMenuView slideMenu;
        public SlideMenuView SlideMenu
        {
            get
            {
                return slideMenu;
            }
            set
            {
                if (slideMenu != null)
                    slideMenu.Parent = null;
                slideMenu = value;
                if (slideMenu != null)
                    slideMenu.Parent = this;
            }
        }

        public Action ShowMenuAction { get; set; }

        public Action HideMenuAction { get; set; }

        public Dictionary<string, SlidePopupView> PopupViews { get; set; }

        public Action<string> ShowPopupAction { get; set; }

        public Action HidePopupAction { get; set; }

        public void ShowMenu()
        {
            ShowMenuAction?.Invoke();
        }

        public void HideMenu()
        {
            HideMenuAction?.Invoke();
        }

        public void ShowPopup(string name)
        {
            ShowPopupAction?.Invoke(name);
        }

        public void HidePopup()
        {
            HidePopupAction?.Invoke();
        }
    }
}


