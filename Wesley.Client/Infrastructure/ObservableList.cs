using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Wesley.Client
{
    public class CommandItem : ReactiveObject
    {
        [Reactive] public string ImageUri { get; set; }
        [Reactive] public string Text { get; set; }
        [Reactive] public string Detail { get; set; }
        public ICommand PrimaryCommand { get; set; }
        public ICommand SecondaryCommand { get; set; }
        public object Data { get; set; }
    }

    public class ObservableList<T> : ObservableCollection<T>
    {
        public ObservableList() { }
        public ObservableList(IEnumerable<T> items) : base(items) { }


        /// <summary>
        /// 添加一个项集合，然后激发CollectionChanged事件-比单独添加更有效
        /// </summary>
        /// <param name="items"></param>
        public virtual void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                this.Items.Add(item);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
        }


        /// <summary>
        /// 清除并设置新集合
        /// </summary>
        /// <param name="items"></param>
        public virtual void ReplaceAll(IEnumerable<T> items)
        {
            this.Clear();
            foreach (var item in items)
                this.Items.Add(item);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
