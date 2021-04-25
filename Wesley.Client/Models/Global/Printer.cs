using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;


namespace Wesley.Client.Models.Global
{
    [Serializable]
    public class Printer : ReactiveObject
    {
        [Reactive] public string Name { get; set; }
        [Reactive] public string LocalName { get; set; }
        [Reactive] public string Address { get; set; }
        [Reactive] public bool Selected { get; set; }
        public ReactiveCommand<Printer, Unit> SelectCommand { get; set; }
    }
}
