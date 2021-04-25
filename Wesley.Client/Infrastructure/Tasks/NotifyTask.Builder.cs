using System;
using System.Threading.Tasks;

namespace Wesley.Infrastructure.Tasks
{

    /// <summary>
    /// 表示通知任务的抽象基类
    /// </summary>
    public abstract partial class NotifyTaskBase
    {
        public abstract class BuilderBase
        {
            /// <summary>
            /// 当任务已经完成时
            /// </summary>
            protected Action<INotifyTask> WhenCompleted { get; set; }
            /// <summary>
            /// 当任务已经取消时
            /// </summary>
            protected Action<INotifyTask> WhenCanceled { get; set; }
            /// <summary>
            /// 当任务已经失败时
            /// </summary>
            protected Action<INotifyTask> WhenFaulted { get; set; }
            /// <summary>
            /// 是否新任务
            /// </summary>
            protected bool InANewTask { get; set; }
            /// <summary>
            /// 任务名称
            /// </summary>
            protected string Name { get; set; }
        }
    }

    public partial class NotifyTask
    {
        /// <summary>
        /// 任务构建器
        /// </summary>
        public class Builder : BuilderBase
        {
            public Builder(Func<Task> task)
            {
                TaskFunc = task;
            }

            /// <summary>
            /// 执行任务委托
            /// </summary>
            protected Func<Task> TaskFunc { get; }

            /// <summary>
            /// 当任务成功完成时执行的委托方法
            /// </summary>
            protected Action<INotifyTask> WhenSuccessfullyCompleted { get; private set; }

            /// <summary>
            /// 当任务完成时
            /// </summary>
            /// <param name="whenCompleted"></param>
            /// <returns></returns>
            public Builder WithWhenCompleted(Action<INotifyTask> whenCompleted)
            {
                WhenCompleted = whenCompleted;
                return this;
            }

            /// <summary>
            /// 当任务取消时
            /// </summary>
            /// <param name="whenCanceled"></param>
            /// <returns></returns>
            public Builder WithWhenCanceled(Action<INotifyTask> whenCanceled)
            {
                WhenCanceled = whenCanceled;
                return this;
            }

            /// <summary>
            /// 当任务失败时
            /// </summary>
            /// <param name="whenFaulted"></param>
            /// <returns></returns>
            public Builder WithWhenFaulted(Action<INotifyTask> whenFaulted)
            {
                WhenFaulted = whenFaulted;
                return this;
            }

            /// <summary>
            /// 新任务
            /// </summary>
            /// <returns></returns>
            public Builder InNewTask()
            {
                InANewTask = true;
                return this;
            }

            /// <summary>
            /// 当任务成功完成时
            /// </summary>
            /// <param name="whenSuccessfullyCompleted"></param>
            /// <returns></returns>
            public Builder WithWhenSuccessfullyCompleted(Action<INotifyTask> whenSuccessfullyCompleted)
            {
                WhenSuccessfullyCompleted = whenSuccessfullyCompleted;
                return this;
            }

            /// <summary>
            /// 构建通知任务
            /// </summary>
            /// <returns></returns>
            public NotifyTask Build()
            {
                return new NotifyTask( 
                    TaskFunc(),
                    WhenCanceled,
                    WhenFaulted,
                    WhenCompleted,
                    WhenSuccessfullyCompleted,
                    InANewTask);
            }
        }
    }

    public partial class NotifyTask<TResult>
    {
        public class Builder : BuilderBase
        {
            public Builder(Func<Task<TResult>> taskFunc)
            {
                TaskFunc = taskFunc;
            }

            protected Func<Task<TResult>> TaskFunc { get; }

            protected Action<INotifyTask, TResult> WhenSuccessfullyCompleted { get; private set; }

            protected TResult DefaultResult { get; private set; }

            public Builder WithWhenCompleted(Action<INotifyTask> whenCompleted)
            {
                WhenCompleted = whenCompleted;
                return this;
            }

            public Builder WithWhenCanceled(Action<INotifyTask> whenCanceled)
            {
                WhenCanceled = whenCanceled;
                return this;
            }

            public Builder WithWhenFaulted(Action<INotifyTask> whenFaulted)
            {
                WhenFaulted = whenFaulted;
                return this;
            }

            public Builder InNewTask()
            {
                InANewTask = true;
                return this;
            }

            public Builder WithWhenSuccessfullyCompleted(Action<INotifyTask, TResult> whenSuccessfullyCompleted)
            {
                WhenSuccessfullyCompleted = whenSuccessfullyCompleted;
                return this;
            }

            public Builder WithDefaultResult(TResult defaultResult)
            {
                DefaultResult = defaultResult;
                return this;
            }

            public NotifyTask<TResult> Build()
            {
                return new NotifyTask<TResult>(
                    TaskFunc(),
                    DefaultResult,
                    WhenCanceled,
                    WhenFaulted,
                    WhenCompleted,
                    WhenSuccessfullyCompleted,
                    InANewTask);
            }
        }
    }
}