using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using System.Threading;


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

    class AsyncSemaphore
    {
        private readonly static Task s_completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> m_waiters = new Queue<TaskCompletionSource<bool>>();
        private int m_currentCount;

        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0) throw new ArgumentOutOfRangeException("initialCount");
            m_currentCount = initialCount;
        }

        public Task WaitAsync()
        {
            lock (m_waiters)
            {
                if (m_currentCount > 0)
                {
                    --m_currentCount;
                    return s_completed;
                }
                else
                {
                    var waiter = new TaskCompletionSource<bool>();
                    m_waiters.Enqueue(waiter);
                    return waiter.Task;
                }
            }
        }

        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (m_waiters)
            {
                if (m_waiters.Count > 0)
                    toRelease = m_waiters.Dequeue();
                else
                    ++m_currentCount;
            }
            if (toRelease != null)
                toRelease.SetResult(true);
        }
    }

    public class AsyncLock
    {
        private readonly AsyncSemaphore m_semaphore;
        private readonly Task<Releaser> m_releaser;

        public AsyncLock()
        {
            m_semaphore = new AsyncSemaphore(1);
            m_releaser = Task.FromResult(new Releaser(this));
        }

        public Task<Releaser> LockAsync()
        {
            var wait = m_semaphore.WaitAsync();
            return wait.IsCompleted ?
                m_releaser :
                wait.ContinueWith((_, state) => new Releaser((AsyncLock)state),
                    this, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock m_toRelease;

            internal Releaser(AsyncLock toRelease) { m_toRelease = toRelease; }

            public void Dispose()
            {
                if (m_toRelease != null)
                    m_toRelease.m_semaphore.Release();
            }
        }
    }
}
