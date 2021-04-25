using Wesley.Client.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;

namespace Wesley.Client.Models
{

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
        [Reactive] public bool Selected { get; set; }
        public int Id { get; set; }
        public MenuEnum MenuEnum
        {
            get { return (MenuEnum)Id; }
            set { Id = (int)value; }
        }
        public string Icon { get; set; }
        public string Text { get; set; }
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
