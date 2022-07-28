using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Wesley.Infrastructure.Tasks
{
    /// <summary>
    /// 用于常规通知任务实现
    /// </summary>
    public partial class NotifyTask : NotifyTaskBase
    {
        public static readonly INotifyTask NotStartedTask = new NotStartedTask();

        /// <summary>
        /// 任务成功完成时回调.
        /// </summary>
        private readonly Action<INotifyTask> _whenSuccessfullyCompleted;

        /// <inheritdoc />
        internal NotifyTask(
            Task task,
            Action<INotifyTask> whenCanceled = null,
            Action<INotifyTask> whenFaulted = null,
            Action<INotifyTask> whenCompleted = null,
            Action<INotifyTask> whenSuccessfullyCompleted = null,
            bool inNewTask = false,
            bool isHot = false,
            Action<String, Exception> errorHandler = null)
            : base(task, whenCanceled, whenFaulted, whenCompleted, inNewTask, isHot, errorHandler)
        {
            _whenSuccessfullyCompleted = whenSuccessfullyCompleted;

            if (isHot)
            {
                TaskCompleted = MonitorTaskAsync(task);
            }
        }

        protected override bool HasCallbacks => base.HasCallbacks || _whenSuccessfullyCompleted != null;

        /// <summary>
        /// 创建具有错误回调的指定任务
        /// </summary>
        public static NotifyTask Create(
            Task task,
            Action<INotifyTask> whenCompleted = null,
            Action<INotifyTask> whenFaulted = null,
            Action<INotifyTask> whenSuccessfullyCompleted = null,
            string name = null)
        {
            return new NotifyTask(
                task,
                whenCompleted: whenCompleted,
                whenFaulted: whenFaulted,
                whenSuccessfullyCompleted: whenSuccessfullyCompleted,
                isHot: true);
        }

        /// <summary>
        /// 创建具有（任务有返回值）错误回调的指定任务
        /// </summary>
        public static NotifyTask Create(
            Func<Task> task,
            Action<INotifyTask> whenCompleted = null,
            Action<INotifyTask> whenFaulted = null,
            Action<INotifyTask> whenSuccessfullyCompleted = null,
            string name = null)
        {
            return new NotifyTask(
                task(),
                whenCompleted: whenCompleted,
                whenFaulted: whenFaulted,
                whenSuccessfullyCompleted: whenSuccessfullyCompleted,
                isHot: true);
        }

        protected override void OnSuccessfullyCompleted(PropertyChangedEventHandler propertyChanged)
        {
            base.OnSuccessfullyCompleted(propertyChanged);

            try
            {
                //调用成功完成回调
                _whenSuccessfullyCompleted?.Invoke(this);
            }
            catch (Exception exception)
            {
                _errorHandler?.Invoke("调用成功完成回调时出错", exception);
            }
        }
    }


    /// <summary>
    /// 没有开始任务
    /// </summary>
    public class NotStartedTask : INotifyTask
    {
        public Task Task { get; }

        public Task TaskCompleted { get; }

        public TaskStatus Status => TaskStatus.Created;

        public bool IsNotStarted => true;

        public bool IsCompleted { get; }

        public bool IsNotCompleted => true;

        public bool IsSuccessfullyCompleted { get; }

        public bool IsCanceled { get; }

        public bool IsFaulted { get; }

        public AggregateException Exception { get; }

        public Exception InnerException { get; }

        public string ErrorMessage { get; }

        public void Start()
        {
            throw new NotSupportedException();
        }

        public void CancelCallbacks()
        {
            throw new NotSupportedException();
        }
    }
}