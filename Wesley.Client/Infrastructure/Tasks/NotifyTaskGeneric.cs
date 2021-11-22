using Wesley.Mvvm;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Wesley.Infrastructure.Tasks
{
    /// <summary>
    /// 用于泛型通知任务实现
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public partial class NotifyTask<TResult> : NotifyTaskBase, INotifyTask<TResult>
    {
        /// <summary>
        /// 定义未开始任务
        /// </summary>
        public static readonly INotifyTask<TResult> NotStartedTask = new NotStartedTask<TResult>();

        /// <summary>
        /// 当“任务”的结果尚未完成时
        /// </summary>
        private readonly TResult _defaultResult;

        /// <summary>
        /// 任务成功完成时调用回调
        /// </summary>
        private readonly Action<INotifyTask, TResult> _whenSuccessfullyCompleted;

        /// <summary>
        /// 继承基类初始化监视指定任务的任务通知程序
        /// </summary>
        /// <param name="task"></param>
        /// <param name="defaultResult"></param>
        /// <param name="whenCanceled"></param>
        /// <param name="whenFaulted"></param>
        /// <param name="whenCompleted"></param>
        /// <param name="whenSuccessfullyCompleted"></param>
        /// <param name="inNewTask"></param>
        /// <param name="isHot"></param>
        /// <param name="errorHandler"></param>
        internal NotifyTask(
            Task<TResult> task,
            TResult defaultResult = default(TResult),
            Action<INotifyTask> whenCanceled = null,
            Action<INotifyTask> whenFaulted = null,
            Action<INotifyTask> whenCompleted = null,
            Action<INotifyTask, TResult> whenSuccessfullyCompleted = null,
            bool inNewTask = false,
            bool isHot = false,
            Action<String, Exception> errorHandler = null)
            : base(task, whenCanceled, whenFaulted, whenCompleted, inNewTask, isHot, errorHandler)
        {
            _defaultResult = defaultResult;
            _whenSuccessfullyCompleted = whenSuccessfullyCompleted;
            Task = task;

            if (isHot)
            {
                TaskCompleted = MonitorTaskAsync(task);
            }
        }

        /// <summary>
        /// 获取正在监视的任务
        /// </summary>
        public new Task<TResult> Task { get; }

        /// <summary>
        /// 获取任务的结果。如果任务尚未成功完成，则返回构造函数中指定的“默认结果”值。此属性在任务成功完成时引发通知。
        /// </summary>
        public TResult Result => (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : _defaultResult;

        /// <summary>
        /// 是否有回调
        /// </summary>
        protected override bool HasCallbacks => base.HasCallbacks || _whenSuccessfullyCompleted != null;

        /// <summary>
        /// 创建监视指定任务的新任务通知程序。
        /// </summary>
        public static NotifyTask<TResult> Create(
            Task<TResult> task,
            Action<INotifyTask> whenCompleted = null,
            Action<INotifyTask> whenFaulted = null,
            Action<INotifyTask, TResult> whenSuccessfullyCompleted = null,
            TResult defaultResult = default(TResult))
        {
            return new NotifyTask<TResult>(
                task,
                whenCompleted: whenCompleted,
                whenFaulted: whenFaulted,
                whenSuccessfullyCompleted: whenSuccessfullyCompleted,
                defaultResult: defaultResult,
                isHot: true);
        }


        /// <summary>
        /// 当成功完成时
        /// </summary>
        /// <param name="propertyChanged"></param>
        protected override void OnSuccessfullyCompleted(PropertyChangedEventHandler propertyChanged)
        {
            //通知结果
            propertyChanged?.Invoke(this, PropertyChangedEventArgsCache.Instance.Get("Result"));
            base.OnSuccessfullyCompleted(propertyChanged);

            try
            {
                //执行成功完成回调
                _whenSuccessfullyCompleted?.Invoke(this, Result);
            }
            catch (Exception exception)
            {
                _errorHandler?.Invoke("执行成功完成回调出错", exception);
            }
        }
    }

    /// <summary>
    /// 表示没有开始的任务
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class NotStartedTask<TResult> : INotifyTask<TResult>
    {
        public TResult Result => default(TResult);

        public Task Task { get; }

        public Task TaskCompleted { get; }

        public TaskStatus Status => TaskStatus.Created;

        public bool IsNotStarted => true;

        public bool IsRunning { get; }

        public bool IsCompleted { get; }

        public bool IsNotCompleted => true;

        public bool IsSuccessfullyCompleted { get; }

        public bool IsCanceled { get; }

        public bool IsFaulted { get; }

        public string Name { get; }

        public AggregateException Exception { get; }

        public Exception InnerException { get; }

        public string ErrorMessage { get; }

        public void Start()
        {
            //抛出不支持异常
            throw new NotSupportedException();
        }

        public void CancelCallbacks()
        {
            //抛出不支持异常
            throw new NotSupportedException();
        }
    }
}