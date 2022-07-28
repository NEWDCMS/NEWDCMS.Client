using Android.Content;
using AndroidX.Concurrent.Futures;
using AndroidX.Work;
using DCMS.Client.Services;
using Google.Common.Util.Concurrent;
using Microsoft.AppCenter.Crashes;
using System;
using System.Threading;
using Android.Runtime;
using Java.Interop;
using Java.Lang;
using Java.Util.Concurrent;

namespace DCMS.Client.Droid.KeepLive.workmanager
{
    //public class JobWorker : Worker
    //{
    //    Context _context;
    //    public const string TAG = "DataSync";
    //    private static Context context = global::Android.App.Application.Context;

    //    public JobWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
    //    {
    //        _context = context;
    //    }

    //    /// <summary>
    //    /// 要执行的任务
    //    /// </summary>
    //    /// <returns></returns>
    //    public override Result DoWork()
    //    {
    //        try
    //        {
    //            System.Diagnostics.Debug.Print($"同步数据.....");
    //            if (Settings.IsAuthenticated)
    //            {
    //                var _globalService = App.Resolve<IGlobalService>();
    //                var _settingService = App.Resolve<ISettingService>();
    //                _globalService?.GetAPPFeatures(true, new CancellationToken());
    //                _globalService?.SynchronizationPermission(new CancellationToken());
    //                _settingService?.GetCompanySettingAsync(new CancellationToken());
    //                Settings.IsInitData = true;
    //            }
    //        }
    //        catch (Java.Lang.Exception ex)
    //        {
    //            Crashes.TrackError(ex);
    //        }
    //        return Result.InvokeSuccess();
    //    }
    //}

    //public class DataSync : IDataSync
    //{
    //    private static Context context = global::Android.App.Application.Context;
    //    public void Run()
    //    {
    //        var cb = new Constraints.Builder();
    //        cb.SetRequiredNetworkType(NetworkType.Connected); // 网络状态
    //        cb.SetRequiresBatteryNotLow(true);// 不在电量不足时执行
    //        cb.SetRequiresCharging(true);// 在充电时执行
    //        cb.SetRequiresStorageNotLow(true);// 不在存储容量不足时执行
    //        cb.SetRequiresDeviceIdle(true);// 在待机状态下执行，需要 API 23
    //        var constraints = cb.Build();

    //        //定义 WorkRequest(OneTimeWorkRequest 任务只需要执行一遍)
    //        var wkRequest = new OneTimeWorkRequest.Builder(typeof(JobWorker))
    //            .SetConstraints(constraints)
    //            .AddTag(JobWorker.TAG)
    //            .Build();

    //        //加入任务队列 
    //        WorkManager.GetInstance(context).BeginUniqueWork(
    //        JobWorker.TAG, ExistingWorkPolicy.Keep, wkRequest)
    //        .Enqueue();
    //    }
    //}

    //public class DataSyncListenableWorker : ListenableWorker, CallbackToFutureAdapter.IResolver
    //{
    //    public const string TAG = "DataSyncListenableWorker";
    //    public DataSyncListenableWorker(Context context, WorkerParameters workerParams) : base(context, workerParams) { }
    //    public override IListenableFuture StartWork()
    //    {
    //        return CallbackToFutureAdapter.GetFuture(this);
    //    }
    //    public Java.Lang.Object AttachCompleter(CallbackToFutureAdapter.Completer p0)
    //    {
    //        Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 5), () =>
    //        {
    //            try
    //            {
    //                System.Diagnostics.Debug.Print($"DataSyncJob:同步数据.....");

    //                if (Settings.IsAuthenticated)
    //                {
    //                    var _globalService = App.Resolve<IGlobalService>();
    //                    var _settingService = App.Resolve<ISettingService>();
    //                    _globalService?.GetAPPFeatures(false, new CancellationToken());
    //                    _globalService?.SynchronizationPermission(new CancellationToken());
    //                    _settingService?.GetCompanySettingAsync(new CancellationToken());
    //                }
    //            }
    //            catch (Java.Lang.Exception ex)
    //            {
    //                Crashes.TrackError(ex);
    //            }

    //            return p0.Set(Result.InvokeSuccess());
    //        });

    //        return TAG;
    //    }
    //}


    public class DataSyncListenableWorkerCall : IDataSyncListenableWorkerCall
    {
        public void Call()
        {
            try
            {
                System.Diagnostics.Debug.Print($"DataSyncJob:同步数据.....");

                if (Settings.IsAuthenticated)
                {
                    var _globalService = App.Resolve<IGlobalService>();
                    var _settingService = App.Resolve<ISettingService>();
                    _globalService?.GetAPPFeatures(false, new CancellationToken());
                    _globalService?.SynchronizationPermission(new CancellationToken());
                    _settingService?.GetCompanySettingAsync(new CancellationToken());
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Crashes.TrackError(ex);
            }

            //var cb = new Constraints.Builder();
            //cb.SetRequiredNetworkType(NetworkType.Connected); // 网络状态
            //cb.SetRequiresBatteryNotLow(true);// 不在电量不足时执行
            //cb.SetRequiresCharging(true);// 在充电时执行
            //cb.SetRequiresStorageNotLow(true);// 不在存储容量不足时执行
            //cb.SetRequiresDeviceIdle(true);// 在待机状态下执行，需要 API 23
            //var constraints = cb.Build();

            ////注意事项
            ///*
            // 1. PeriodicWorkRequest最小重复执行时间间隔必须是PeriodicWorkRequest.MIN_PERIODIC_INTERVAL_MILLIS（900000毫秒，15分钟），小于这个值将取该临界值作为重复执行时间间隔；
            // 2. PeriodicWorkRequest 跟 OneTimeWorkRequest 一样可以可以添加任务约束、跟踪任务状态、跟踪任务的中间状态以及取消和停止共工作等；
            // 3. PeriodicWorkRequest无法使用链接功能，如果需要将服务链接起来，请使用 OneTimeWorkRequest；
            // 4. 重复性工作在单次任务执行完毕后即返回了 Result.success() ，也不会监听到 State.SUCCEEDED 状态。
            //*/

            ////定义 WorkRequest(OneTimeWorkRequest 任务只需要执行一遍)
            //var wkRequest = new OneTimeWorkRequest.Builder(typeof(DataSyncListenableWorker))
            //    .SetConstraints(constraints)
            //    .AddTag(DataSyncListenableWorker.TAG)
            //    .Build();

            ////加入任务队列 
            //WorkManager.GetInstance(context).BeginUniqueWork(
            //DataSyncListenableWorker.TAG, ExistingWorkPolicy.Keep, wkRequest)
            //.Enqueue();
        }
    }
}