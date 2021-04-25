using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Wesley.Client
{
    public class AggregateExceptionArgs : EventArgs
    {
        public AggregateException AggregateException { get; set; }
    }

    public class Sync
    {
        public static event EventHandler<AggregateExceptionArgs> AggregateExceptionCatched;
        public static void Run(Action doWork, Action<Exception> errorAction = null)
        {
            try
            {
                Task task = new Task(() =>
                {
                    try
                    {
                        doWork?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        errorAction?.Invoke(ex);
                    }
                });

                task.Start();
            }
            catch (Exception ex)
            {
                try
                {
                    AggregateExceptionArgs args = new AggregateExceptionArgs()
                    {
                        AggregateException = new AggregateException(ex)
                    };
                    AggregateExceptionCatched?.Invoke(null, args);
                }
                catch (Exception)
                {
                }
            }
        }
        public static async Task<T> RunResult<T>(Func<T> doWork, Action<Exception> errorAction = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            var result = await Task.Run<T>(() =>
            {
                try
                {
                    return doWork.Invoke();
                }
                catch (Exception ex)
                {
                    try
                    {
                        errorAction?.Invoke(ex);
                        return default;
                    }
                    catch (Exception)
                    {
                        return default;
                    }
                }
            });
            return result;
        }
        public static Task<ObservableCollection<T>> Run<T>(Func<IList<T>> doWork, Action<Exception> errorAction = null)
        {
            Task<ObservableCollection<T>> task = Task.Factory.StartNew(() =>
            {
                try
                {
                    var t = doWork?.Invoke();
                    return new ObservableCollection<T>(t);
                }
                catch (Exception ex)
                {
                    try
                    {
                        errorAction?.Invoke(ex);
                        return new ObservableCollection<T>((IEnumerable<T>)default(T));
                    }
                    catch (Exception)
                    {
                        return new ObservableCollection<T>((IEnumerable<T>)default(T));
                    }
                }
            });
            return task;
        }

    }
}
