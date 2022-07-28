
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Threading;

namespace Wesley.Client
{

    /// <summary>
    /// 线程安全的集合，将ObservableCollection替换掉即可
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        //获取当前线程的SynchronizationContext对象
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        public AsyncObservableCollection() { }
        public AsyncObservableCollection(IEnumerable<T> list) : base(list) { }
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {

            if (SynchronizationContext.Current == _synchronizationContext)
            {
                //如果操作发生在同一个线程中，不需要进行跨线程执行
                if (e != null)
                    RaiseCollectionChanged(e);
            }
            else
            {
                //如果不是发生在同一个线程中
                //准确说来，这里是在一个非UI线程中，需要进行UI的更新所进行的操作
                if (e != null)
                    _synchronizationContext.Post(RaiseCollectionChanged, e);
            }
        }
        private void RaiseCollectionChanged(object param)
        {
            if (param != null)
                base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                if (e != null)
                    RaisePropertyChanged(e);
            }
            else
            {
                if (e != null)
                    _synchronizationContext.Post(RaisePropertyChanged, e);
            }
        }
        private void RaisePropertyChanged(object param)
        {
            if (param != null)
                base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }
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
            if (items == null) return;

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

            if (items == null) return;

            foreach (var item in items)
                this.Items.Add(item);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }



        public IObservable<Unit> WhenCollectionChanged() => Observable.Create<Unit>(ob =>
        {
            var handler = new NotifyCollectionChangedEventHandler((sender, args) =>
                ob.Respond(Unit.Default)
            );
            this.CollectionChanged += handler;
            return () => this.CollectionChanged -= handler;
        });

    }
}
