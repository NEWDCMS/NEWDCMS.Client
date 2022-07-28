using System;
namespace Wesley.Client
{
    public class ScrollToItemEventArgs : EventArgs
    {
        public object Item { get; set; }
        public int? Index { get; set; }
    }
}
