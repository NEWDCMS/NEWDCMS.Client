using Wesley.Client.Enums;
using Wesley.Client.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace Wesley.Client.Models
{
    public class CutMenus
    {
        public int Type { get; set; }
        public string Name { get; set; }
        public string Describe { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public bool ShowSeparator { get; set; }
    }
    public class CutMenusGroup : List<CutMenus>
    {
        public int Type { get; set; }
        public CutMenusGroup(int type, List<CutMenus> menus) : base(menus)
        {
            Type = type;
        }
    }


    /// <summary>
    /// 表示菜单绑定
    /// </summary>
    public class MenuBinding : Base
    {
        [Reactive] public bool Selected { get; set; }
        public int Id { get; set; }
        [Reactive] public string Name { get; set; }
    }


    /// <summary>
    /// 表示页面级菜单
    /// </summary>
    public class SubMenu
    {
        public ReactiveCommand<MenuEnum, Unit> SelectedCommand { get; set; }
        public int Id { get; set; }
        public string Text { get; set; }
        public Action<SubMenu, ViewModelBase> CallBack { get; set; }
    }

    public class WeekDay : Base
    {
        public ReactiveCommand<WeekDay, Unit> SelectedCommand { get; set; }
        [Reactive] public bool Selected { get; set; }
        [Reactive] public string SelectedBg { get; set; }
        public DateTime Date { get; set; }
        public string Wname { get; set; }
        public string AMTimeRange { get; set; }
        public string PMTimeRange { get; set; }
    }

    public class TimeRange
    {
        public string Start { get; set; }
        public string End { get; set; }
    }
}
