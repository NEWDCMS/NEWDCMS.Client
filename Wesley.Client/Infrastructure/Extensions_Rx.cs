using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;

namespace Wesley.Client
{
    public class ItemChanged<T, TRet>
    {
        public ItemChanged(T obj, TRet value)
        {
            this.Object = obj;
            this.Value = value;
        }

        public T Object { get; }
        public TRet Value { get; }
    }



    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// 合并指定的originalSource和newSource.
        /// </summary>
        /// <param name="originalSource">Original source.</param>
        /// <param name="newSource">New source.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <remarks>
        /// Current version is way to generic!
        /// Add and remove will only work if objects are the same ( which is higly unlikely )
        /// There is no actual merging of items that exist in both sources but are updated in the new source
        /// </remarks>
        public static void Merge<T>(this ObservableCollection<T> originalSource, IEnumerable<T> newSource)
        {
            try
            {
                var add = newSource.Except(originalSource);
                var remove = originalSource.Except(newSource);

                foreach (var i in remove)
                    originalSource.Remove(i);

                foreach (var i in add)
                    originalSource.Add(i);

                //If there are any changes make sure the order is the same as how we got the data in the new source
                if (add.Count() > 0 || remove.Count() > 0)
                    for (int i = 0; i < newSource.Count(); i++)
                        originalSource.Move(originalSource.IndexOf(newSource.ElementAt(i)), i);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Passes the last and current values from the stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        /// <returns></returns>
        public static IObservable<Tuple<T, T>> WithPrevious<T>(this IObservable<T> ob)
            => ob.Scan(Tuple.Create(default(T), default(T)), (acc, current) => Tuple.Create(acc.Item2, current));



        /// <summary>
        /// This is to make chaining easier when a scheduler is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static IObservable<T> ObserveOnIf<T>(this IObservable<T> ob, IScheduler? scheduler)
        {
            if (scheduler != null)
                ob.ObserveOn(scheduler);

            return ob;
        }


        /// <summary>
        /// Runs an action only once when the first result is received
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obs"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IObservable<T> DoOnce<T>(this IObservable<T> obs, Action<T> action)
        {
            var count = 1;
            return obs.Do(x =>
            {
                if (count == 0)
                {
                    Interlocked.Increment(ref count);
                    action(x);
                }
            });
        }

        /// <summary>
        /// A function from ReactiveUI - useful for non-ui stuff too ;)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="compositeDisposable"></param>
        /// <returns></returns>
        public static T DisposedBy<T>(this T @this, CompositeDisposable compositeDisposable) where T : IDisposable
        {
            compositeDisposable.Add(@this);
            return @this;
        }


        /// <summary>
        /// A handy way for replying and completing an observer - common for single valued observables
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        /// <param name="value"></param>
        public static void Respond<T>(this IObserver<T> ob, T value)
        {
            ob.OnNext(value);
            ob.OnCompleted();
        }


        /// <summary>
        /// This will buffer observable pings and timestamp them until the predicate check does not pass
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisObs"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IObservable<List<Timestamped<T>>> BufferWhile<T>(this IObservable<T> thisObs, Func<T, bool> predicate)
            => Observable.Create<List<Timestamped<T>>>(ob =>
            {
                var list = new List<Timestamped<T>>();
                return thisObs
                    .Timestamp()
                    .Subscribe(x =>
                    {
                        if (predicate(x.Value))
                        {
                            list.Add(x);
                        }
                        else if (list != null)
                        {
                            ob.OnNext(list);
                            list.Clear();
                        }
                    });
            });




        public static IObservable<ItemChanged<TSender, string>> WhenAnyProperty<TSender>(this TSender This) where TSender : INotifyPropertyChanged
            => Observable
                .FromEventPattern<PropertyChangedEventArgs>(This, nameof(INotifyPropertyChanged.PropertyChanged))
                .Select(x => new ItemChanged<TSender, string>(This, x.EventArgs.PropertyName));


        public static IDisposable SubscribeVoid<T>(this IObservable<T> observable, Action onNext)
            => observable.Subscribe(_ => onNext());


        public static IDisposable SubscribeVoid<T>(this IObservable<T> observable, Action onNext, Action<Exception> onError)
            => observable.Subscribe(
                _ => onNext(),
                onError
            );


        public static IDisposable SubscribeVoid<T>(this IObservable<T> observable, Action onNext, Action<Exception> onError, Action onComplete)
            => observable.Subscribe(
                _ => onNext(),
                onError,
                onComplete
            );


        public static IDisposable SubscribeAsync<T>(this IObservable<T> observable, Func<T, Task> onNextAsync)
            => observable
                .Select(x => Observable.FromAsync(() => onNextAsync(x)))
                .Concat()
                .Subscribe();


        public static IDisposable SubscribeAsync<T>(this IObservable<T> observable,
                                                    Func<T, Task> onNextAsync,
                                                    Action<Exception> onError)
            => observable
                .Select(x => Observable.FromAsync(() => onNextAsync(x)))
                .Concat()
                .Subscribe(
                    _ => { },
                    onError
                );


        public static IDisposable SubscribeAsync<T>(this IObservable<T> observable,
                                                    Func<T, Task> onNextAsync,
                                                    Action<Exception> onError,
                                                    Action onComplete)
            => observable
                .Select(x => Observable.FromAsync(() => onNextAsync(x)))
                .Concat()
                .Subscribe(
                    _ => { },
                    onError,
                    onComplete
                );

        public static IDisposable SubscribeAsyncConcurrent<T>(this IObservable<T> observable, Func<T, Task> onNextAsync)
            => observable
                .Select(x => Observable.FromAsync(() => onNextAsync(x)))
                .Merge()
                .Subscribe();


        public static IDisposable SubscribeAsyncConcurrent<T>(this IObservable<T> observable, Func<T, Task> onNextAsync, int maxConcurrent)
            => observable
                .Select(x => Observable.FromAsync(() => onNextAsync(x)))
                .Merge(maxConcurrent)
                .Subscribe();
    }
}
