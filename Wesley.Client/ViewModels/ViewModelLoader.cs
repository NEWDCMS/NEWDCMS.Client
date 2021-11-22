using Wesley.Infrastructure.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections;
using System.Reactive;
using System.Threading.Tasks;

namespace Wesley.Client.ViewModels
{
    #region  自定义视图模型加载器

    public interface IViewModelLoader
    {
        /// <summary>
        /// 重载执行
        /// </summary>
        IReactiveCommand ReloadCommand { get; }
        /// <summary>
        /// 刷新执行
        /// </summary>
        IReactiveCommand RefreshCommand { get; }
        /// <summary>
        /// 是否完成
        /// </summary>
        bool IsCompleted { get; }
        /// <summary>
        /// 是否还没开始
        /// </summary>
        bool IsNotStarted { get; }
        /// <summary>
        /// 是否没有完成
        /// </summary>
        bool IsNotCompleted { get; }
        /// <summary>
        /// 是否成功成功完成
        /// </summary>
        bool IsSuccessfullyCompleted { get; }
        /// <summary>
        /// 是否已取消
        /// </summary>
        bool IsCanceled { get; }
        /// <summary>
        /// 是否失败了
        /// </summary>
        bool IsFaulted { get; }
        /// <summary>
        /// 错误异常
        /// </summary>
        Exception Error { get; }
        /// <summary>
        /// 错误消息
        /// </summary>
        string ErrorMessage { get; }
        /// <summary>
        /// 空状态消息
        /// </summary>
        string EmptyStateMessage { get; }
        /// <summary>
        /// 显示Loader
        /// </summary>
        bool ShowLoader { get; }
        /// <summary>
        /// 显示Refresher
        /// </summary>
        bool ShowRefresher { get; }
        /// <summary>
        /// 显示结果
        /// </summary>
        bool ShowResult { get; }
        /// <summary>
        /// 显示错误
        /// </summary>
        bool ShowError { get; }
        /// <summary>
        /// 显示为空提示
        /// </summary>
        bool ShowEmptyState { get; }
        /// <summary>
        /// 显示错误通知
        /// </summary>
        bool ShowErrorNotification { get; }
    }

    public abstract class ViewModelLoaderBase : ReactiveObject, IViewModelLoader
    {
        private readonly Func<Exception, string> _errorHandler;
        protected ViewModelLoaderBase(Func<Exception, string> errorHandler = null, string emptyStateMessage = null)
        {
            _errorHandler = errorHandler ?? DefaultErrorHandler;
            EmptyStateMessage = emptyStateMessage;
        }

        public IReactiveCommand ReloadCommand { get; protected set; }
        public IReactiveCommand RefreshCommand { get; protected set; }
        public bool IsCompleted => CurrentLoadingTask.IsCompleted;
        public abstract bool IsNotStarted { get; }
        public bool IsNotCompleted => CurrentLoadingTask.IsNotCompleted;
        public bool IsSuccessfullyCompleted => CurrentLoadingTask.IsSuccessfullyCompleted;
        public bool IsCanceled => CurrentLoadingTask.IsCanceled;
        public bool IsFaulted => CurrentLoadingTask.IsFaulted;


        [Reactive] public bool ShowLoader { get; set; } = true;
        [Reactive] public bool ShowRefresher { get; set; }
        [Reactive] public bool ShowResult { get; set; }
        [Reactive] public bool ShowError { get; set; }
        [Reactive] public bool ShowEmptyState { get; set; }
        [Reactive] public bool ShowErrorNotification { get; set; }
        [Reactive] public string ErrorMessage { get; set; }
        [Reactive] public Exception Error { get; set; }
        [Reactive] public string EmptyStateMessage { get; set; }

        /// <summary>
        /// 同步互斥
        /// </summary>
        protected object SyncRoot { get; } = new object();


        /// <summary>
        /// 当前加载任务
        /// </summary>
        protected INotifyTask CurrentLoadingTask { get; set; }

        protected void OnTaskCompleted(INotifyTask task)
        {
            ShowRefresher = ShowLoader = false;
            //RaisePropertyChanged(nameof(IsCompleted));
            //RaisePropertyChanged(nameof(IsNotCompleted));
            //RaisePropertyChanged(nameof(IsNotStarted));
        }

        protected void OnTaskFaulted(INotifyTask faultedTask, bool isRefreshing)
        {
            // Log.Info("Task completed with fault");
            //RaisePropertyChanged(nameof(IsFaulted));
            ShowLoader = false;
            ShowError = !isRefreshing;
            ShowErrorNotification = isRefreshing;
            Error = faultedTask.InnerException;
            ErrorMessage = ToErrorMessage(faultedTask.InnerException);
        }

        protected virtual void OnTaskSuccessfullyCompleted(INotifyTask task)
        {
            ShowRefresher = ShowLoader = false;
            //RaisePropertyChanged(nameof(IsSuccessfullyCompleted));
            ShowResult = true;
        }

        protected string ToErrorMessage(Exception exception)
        {
            return _errorHandler.Invoke(exception);
        }

        /// <summary>
        /// 重置刷新
        /// </summary>
        /// <param name="isRefreshing"></param>
        protected virtual void Reset(bool isRefreshing)
        {
            ShowLoader = !isRefreshing;
            ShowRefresher = isRefreshing;

            if (!isRefreshing)
            {
                Error = null;
                ShowError = ShowResult = ShowEmptyState = false;
            }

            //RaisePropertyChanged(nameof(IsCompleted));
            //RaisePropertyChanged(nameof(IsNotCompleted));
            //RaisePropertyChanged(nameof(IsNotStarted));
            //RaisePropertyChanged(nameof(IsSuccessfullyCompleted));
            //RaisePropertyChanged(nameof(IsFaulted));
        }

        private static string DefaultErrorHandler(Exception exception)
        {
            return "发生未知错误";
        }
    }

    /// <summary>
    /// 标准
    /// </summary>
    public class ViewModelLoader : ViewModelLoaderBase
    {
        private Func<Task> _loadingTaskSource;

        public ViewModelLoader(Func<Exception, string> errorHandler = null, string emptyStateMessage = null)
            : base(errorHandler, emptyStateMessage)
        {
            CurrentLoadingTask = NotifyTask.NotStartedTask;
            //ReloadCommand = new Command(() => Load(_loadingTaskSource));
            //RefreshCommand = new Command(() => Load(_loadingTaskSource, isRefreshing: true));
            ReloadCommand = Load(_loadingTaskSource);
            RefreshCommand = Load(_loadingTaskSource, isRefreshing: true);
        }

        public override bool IsNotStarted => CurrentLoadingTask == NotifyTask.NotStartedTask;

        /*
        public void Load(Func<Task> loadingTaskSource, bool isRefreshing = false)
        {
            // Log.Info("Load");
            lock (SyncRoot)
            {
                if (CurrentLoadingTask != NotifyTask.NotStartedTask && CurrentLoadingTask.IsNotCompleted)
                {
                    // Log.Warn("A loading task is currently running: discarding this call");
                    return;
                }

                _loadingTaskSource = loadingTaskSource;

                CurrentLoadingTask = null;
                CurrentLoadingTask = new NotifyTask.Builder(_loadingTaskSource)
                    .WithWhenCompleted(OnTaskCompleted)
                    .WithWhenFaulted(faultedTask => OnTaskFaulted(faultedTask, isRefreshing))
                    .WithWhenSuccessfullyCompleted(OnTaskSuccessfullyCompleted)
                    .Build();
            }

            Reset(isRefreshing);
            CurrentLoadingTask.Start();
        }
        */

        public ReactiveCommand<Unit, Unit> Load(Func<Task> loadingTaskSource, bool isRefreshing = false)
        {
            return ReactiveCommand.Create<Unit>(e =>
            {
                lock (SyncRoot)
                {
                    if (CurrentLoadingTask != NotifyTask.NotStartedTask && CurrentLoadingTask.IsNotCompleted)
                    {
                        return;
                    }

                    _loadingTaskSource = loadingTaskSource;

                    CurrentLoadingTask = null;
                    CurrentLoadingTask = new NotifyTask.Builder(_loadingTaskSource)
                        .WithWhenCompleted(OnTaskCompleted)
                        .WithWhenFaulted(faultedTask => OnTaskFaulted(faultedTask, isRefreshing))
                        .WithWhenSuccessfullyCompleted(OnTaskSuccessfullyCompleted)
                        .Build();
                }

                //更新
                Reset(isRefreshing);
                CurrentLoadingTask.Start();
            });
        }

    }

    /// <summary>
    /// 泛型支持
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ViewModelLoader<TData> : ViewModelLoaderBase where TData : class
    {
        /// <summary>
        /// 要加载的任务源
        /// </summary>
        private Func<Task<TData>> _loadingTaskSource;

        public ViewModelLoader(Func<Exception, string> errorHandler = null, string emptyStateMessage = null)
            : base(errorHandler, emptyStateMessage)
        {
            CurrentLoadingTask = NotifyTask<TData>.NotStartedTask;

            //ReloadCommand = new Command(() => Load(_loadingTaskSource));
            //RefreshCommand = new Command(() => Load(_loadingTaskSource, isRefreshing: true));

            //ReloadCommand = ReactiveCommand.Create(() => Load(_loadingTaskSource));
            //RefreshCommand = ReactiveCommand.Create(() => Load(_loadingTaskSource, isRefreshing: true));

            ReloadCommand = Load(_loadingTaskSource);
            RefreshCommand = Load(_loadingTaskSource, isRefreshing: true);
        }

        public override bool IsNotStarted => CurrentLoadingTask == NotifyTask<TData>.NotStartedTask;

        [Reactive] public TData Result { get; set; }

        /*
        public void Load(Func<Task<TData>> loadingTaskSource, bool isRefreshing = false)
        {
            lock (SyncRoot)
            {
                if (CurrentLoadingTask != NotifyTask<TData>.NotStartedTask && CurrentLoadingTask.IsNotCompleted)
                {
                    // Log.Warn("加载任务当前正在运行：放弃此调用");
                    return;
                }

                if (CurrentLoadingTask == NotifyTask<TData>.NotStartedTask && loadingTaskSource == null)
                {
                    // Log.Warn("请求刷新，但尚未加载，正在中止...");
                    return;
                }

                _loadingTaskSource = loadingTaskSource;

                CurrentLoadingTask = null;
                CurrentLoadingTask = new NotifyTask<TData>.Builder(_loadingTaskSource)
                    .WithWhenCompleted(OnTaskCompleted)
                    .WithWhenFaulted(faultedTask => OnTaskFaulted(faultedTask, isRefreshing))
                    .WithWhenSuccessfullyCompleted(
                        (completedTask, result) =>
                        {
                            Result = result;
                            OnTaskSuccessfullyCompleted(completedTask);
                        })
                    .Build();
            }

            Reset(isRefreshing);
            CurrentLoadingTask.Start();
        }
        */

        public ReactiveCommand<Unit, Unit> Load(Func<Task<TData>> loadingTaskSource, bool isRefreshing = false, IObservable<bool> canExecute = null)
        {
            return ReactiveCommand.Create<Unit>(e =>
            {
                lock (SyncRoot)
                {
                    //加载任务当前正在运行
                    if (CurrentLoadingTask != NotifyTask<TData>.NotStartedTask && CurrentLoadingTask.IsNotCompleted)
                    {
                        return;
                    }

                    //任务未加载
                    if (CurrentLoadingTask == NotifyTask<TData>.NotStartedTask && loadingTaskSource == null)
                    {
                        return;
                    }

                    //任务
                    _loadingTaskSource = loadingTaskSource;

                    //构建一个通知任务
                    CurrentLoadingTask = null;
                    CurrentLoadingTask = new NotifyTask<TData>.Builder(_loadingTaskSource)
                        .WithWhenCompleted(OnTaskCompleted)
                        .WithWhenFaulted(faultedTask => OnTaskFaulted(faultedTask, isRefreshing))
                        .WithWhenSuccessfullyCompleted((completedTask, result) =>
                        {
                            Result = result;
                            OnTaskSuccessfullyCompleted(completedTask);
                        })
                        .InNewTask()
                        .Build();
                }

                //更新
                Reset(isRefreshing);
                CurrentLoadingTask.Start();

            }, canExecute);
        }


        protected override void Reset(bool isRefreshing)
        {
            base.Reset(isRefreshing);
            //RaisePropertyChanged(nameof(Result));
        }

        protected override void OnTaskSuccessfullyCompleted(INotifyTask task)
        {
            // Log.Info("Task successfully completed");
            //RaisePropertyChanged(nameof(IsSuccessfullyCompleted));
            if (EmptyStateMessage != null && (Result == null || (Result is ICollection collection && collection.Count == 0)))
            {
                ShowEmptyState = true;
                return;
            }
            ShowResult = true;
        }
    }

    #endregion
}
