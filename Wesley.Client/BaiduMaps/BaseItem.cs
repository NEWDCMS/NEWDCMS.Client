using Wesley.Client.Models.Terminals;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Wesley.Client.BaiduMaps
{
    public class BaseItem : BindableObject
    {
        public object NativeObject { get; set; }
        public object Tag { get; set; }
        public TerminalModel Terminal { get; set; }

        public event EventHandler<TagEventArgs> Clicked;
        public void SendClicked(TerminalModel terminal)
        {
            Clicked?.Invoke(this, new TagEventArgs(terminal));
        }
    }

    public static class BaseItemHelper
    {
        public static T Find<T>(this IList<T> list, object native)
            where T : BaseItem
        {
            foreach (T item in list)
            {
                if (item.NativeObject == native)
                {
                    return item;
                }
            }

            return null;
        }
    }
}

