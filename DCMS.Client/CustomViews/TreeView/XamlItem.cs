using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;

namespace Wesley.Client.CustomViews
{
    [Serializable]
    public class XamlItem : ReactiveObject
    {
        public string Key { get; set; }
        public int ItemId { get; set; }
        public int? ParentId { get; set; } = 0;
        [Reactive] public bool Selected { get; set; }
        public ReactiveCommand<XamlItem, Unit> SelectedCommand { get; set; }
    }
}
