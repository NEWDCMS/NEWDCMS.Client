using Wesley.Mvvm;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Wesley.Infrastructure.Tasks
{
    /// <summary>
    /// 定义任务通知接口，用于表示监视任务并在任务完成时引发属性更改通知
    /// </summary>
    public interface INotifyTask
    {
        /// <summary>
        /// 获取正在监视的任务。此属性从不更改，也从不为<c>Null</c>
        /// </summary>
        Task Task { get; }

        /// <summary>
        /// 获取在任务完成（成功、失败或取消）时成功完成的任务。这个属性永远不会改变，也永远不会为<c>null</c>.
        /// </summary>
        Task TaskCompleted { get; }

        /// <summary>
        /// 获取当前任务状态。此属性在任务完成时引发通知
        /// </summary>
        TaskStatus Status { get; }

        /// <summary>
        /// 获取任务是否已启动.
        /// </summary>
        bool IsNotStarted { get; }

        /// <summary>
        /// 获取任务是否已完成。此属性在值更改为 <c>true</c> 时引发通知.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// 获取任务是否没有完成。此属性在值更改为 <c>true</c> 时引发通知.
        /// </summary>
        bool IsNotCompleted { get; }

        /// <summary>
        ///获取任务是否已成功完成. 此属性在值更改为 <c>true</c> 时引发通知.
        /// </summary>
        bool IsSuccessfullyCompleted { get; }

        /// <summary>
        /// 获取任务是否已取消. 此属性在值更改为 <c>true</c> 时引发通知.
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// 获取任务是否已失败. 此属性在值更改为 <c>true</c> 时引发通知.
        /// </summary>
        bool IsFaulted { get; }

        /// <summary>
        /// 获取任务的包装错误异常。如果任务没有出错，则返回<c>null</c>。
        /// 此属性仅在任务出错时引发通知
        /// </summary>
        AggregateException Exception { get; }

        /// <summary>
        /// 获取任务的原始错误异常。如果任务没有出错，则返回 Null 
        /// </summary>
        Exception InnerException { get; }

        /// <summary>
        /// 获取异常错误消息.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// 如果是冷任务，手动启动
        /// </summary>
        void Start();

        /// <summary>
        /// 取消回调：任务将执行到最后，但不会调用任何回调或npc.
        /// </summary>
        void CancelCallbacks();
    }

    /// <summary>
    /// 定义泛型接口，用于表示监视返回结果的任务，并在任务完成时引发属性更改通知
    /// </summary>
    public interface INotifyTask<TResult> : INotifyTask
    {
        /// <summary>
        /// 获取任务的结果。如果任务尚未成功完成，则返回构造函数中指定的“默认结果”值。此属性在任务成功完成时引发通知。
        /// </summary>
        TResult Result { get; }
    }

    /// <summary>
    /// 用于表示监视任务并在任务完成时引发属性更改通知抽象基类，实现 INotifyPropertyChanged.
    /// </summary>
    public abstract partial class NotifyTaskBase : INotifyTask, INotifyPropertyChanged
    {
        /// <summary>
        /// 如果为true，则在构造函数中监视任务以启动它
        /// </summary>
        private readonly bool _isHot;

        /// <summary>
        /// 如果为true，则将任务包装在新任务中
        /// </summary>
        private readonly bool _inNewTask;

        /// <summary>
        /// 取消任务时调用的回调
        /// </summary>
        private readonly Action<INotifyTask> _whenCanceled;

        /// <summary>
        /// 任务出错时调用回调
        /// </summary>
        private readonly Action<INotifyTask> _whenFaulted;

        /// <summary>
        /// 当任务完成（无论成功与否）时调用回调
        /// </summary>
        private readonly Action<INotifyTask> _whenCompleted;

        private bool _areCallbacksCancelled;

        /// <summary>
        /// 错误处理
        /// </summary>
        protected readonly Action<string, Exception> _errorHandler;

        /// <summary>
        /// 初始化监视指定任务的任务通知程序
        /// </summary>
        protected NotifyTaskBase(
            Task task,
            Action<INotifyTask> whenCanceled = null,
            Action<INotifyTask> whenFaulted = null,
            Action<INotifyTask> whenCompleted = null,
            bool inNewTask = false,
            bool isHot = false,
            Action<string, Exception> errorHandler = null)
        {
            Task = task;
            _whenCanceled = whenCanceled;
            _whenFaulted = whenFaulted;
            _whenCompleted = whenCompleted;
            _inNewTask = inNewTask;
            _isHot = isHot;
            _errorHandler = errorHandler ?? DefaultErrorHandler;
        }

        /// <summary>
        /// 通知侦听器属性值更改的事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public Task Task { get; }

        /// <inheritdoc />
        public Task TaskCompleted { get; protected set; }

        /// <inheritdoc />
        public TaskStatus Status => Task.Status;

        /// <inheritdoc />
        public bool IsCompleted => Task.IsCompleted;

        /// <inheritdoc />
        public bool IsNotStarted => Task.Status == TaskStatus.Created;

        /// <inheritdoc />
        public bool IsNotCompleted => !Task.IsCompleted;

        /// <inheritdoc />
        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

        /// <inheritdoc />
        public bool IsCanceled => Task.IsCanceled;

        /// <inheritdoc />
        public bool IsFaulted => Task.IsFaulted;

        /// <inheritdoc />
        public AggregateException Exception => Task.Exception;

        /// <inheritdoc />
        public Exception InnerException => Exception?.InnerException;

        /// <inheritdoc />
        public string ErrorMessage => InnerException?.Message;

        protected virtual bool HasCallbacks => _whenCanceled != null || _whenCompleted != null || _whenFaulted != null;

        /// <inheritdoc />
        public void Start()
        {
            if (!_isHot)
            {
                TaskCompleted = MonitorTaskAsync(Task);
            }
        }

        /// <summary>
        /// 取消回调
        /// </summary>
        public void CancelCallbacks()
        {
            _areCallbacksCancelled = true;
        }

        /// <summary>
        /// 默认错误处理
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        protected static void DefaultErrorHandler(string message, Exception exception)
        {
            Trace.WriteLine($"NotifyTask|ERROR|{message}, Exception:{Environment.NewLine}{exception}");
        }

        /// <summary>
        /// 监视任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        protected async Task MonitorTaskAsync(Task task)
        {
            try
            {
                if (task != null && task.Status != TaskStatus.Faulted)
                {
                    //如果需要包含在新线程时
                    if (_inNewTask)
                    {
                        await Task.Run(async () => await task);
                    }
                    else
                    {
                        await task;
                    }
                }
            }
            catch (TaskCanceledException canceledException)
            {
                _errorHandler?.Invoke("任务已经取消", canceledException);
            }
            catch (Exception exception)
            {
                _errorHandler?.Invoke("包装任务出错", exception);
            }
            finally
            {
                if (task != null)
                {
                    //调用回调
                    InvokeCallbacks(task);
                }
            }
        }

        /// <summary>
        /// 当任务成功并完成时
        /// </summary>
        /// <param name="propertyChanged"></param>
        protected virtual void OnSuccessfullyCompleted(PropertyChangedEventHandler propertyChanged)
        {
            //通知状态和已经成功完成
            propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("Status"));
            propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("IsSuccessfullyCompleted"));
        }

        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="task"></param>
        private void InvokeCallbacks(Task task)
        {
            var propertyChanged = PropertyChanged;
            if (_areCallbacksCancelled || (propertyChanged == null && !HasCallbacks))
            {
                return;
            }

            //通知完成状态和是否没有完成
            propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("IsCompleted"));
            propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("IsNotCompleted"));

            try
            {
                //调用完成时回调
                _whenCompleted?.Invoke(this);
            }
            catch (Exception exception)
            {
                _errorHandler?.Invoke("调用完成回调时出错", exception);
            }

            //如果任务取消
            if (task.IsCanceled)
            {
                //通知状态和取消
                propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("Status"));
                propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("IsCanceled"));

                try
                {
                    //调用取消回调
                    _whenCanceled?.Invoke(this);
                }
                catch (Exception exception)
                {
                    _errorHandler?.Invoke("调用取消回调时出错", exception);
                }
            }
            //如果失败
            else if (task.IsFaulted)
            {
                //通知异常，错误，状态和失败
                propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("Exception"));
                propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("InnerException"));
                propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("ErrorMessage"));
                propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("Status"));
                propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("IsFaulted"));

                try
                {
                    //调用失败回调
                    _whenFaulted?.Invoke(this);
                }
                catch (Exception exception)
                {
                    _errorHandler?.Invoke("调用失败回调时出错", exception);
                }
            }
            //成功且完成时
            else
            {
                OnSuccessfullyCompleted(propertyChanged);
            }
        }
    }
}