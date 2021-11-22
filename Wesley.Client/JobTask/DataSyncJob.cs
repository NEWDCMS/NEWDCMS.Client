using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Shiny;
using Shiny.Jobs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DCMS.Client
{
    /// <summary>
    /// 数据同步Job
    /// </summary>
    public class DataSyncJob : IJob, IShinyStartupTask
    {
        //private readonly CoreDelegateServices services;
        //public DataSyncJob(CoreDelegateServices services) => this.services = services;

        public Task Run(JobInfo jobInfo, CancellationToken cancelToken)
        {

            //发送通知
            //await this.services.Notifications.Send(
            //    this.GetType(),
            //    true,
            //    "数据同步工作已启动",
            //    $"{jobInfo.Identifier} 启动"
            //);

            try
            {
                var _globalService = App.Resolve<IGlobalService>();
                var _settingService = App.Resolve<ISettingService>();

                var seconds = jobInfo.Parameters.Get("SecondsToRun", 30);
                Debug.Print($"DataSyncJob:{seconds}秒同步一次数据.....");

                if (Settings.IsAuthenticated)
                {
                    _globalService?.GetAPPFeatures(calToken: cancelToken);
                    _globalService?.SynchronizationPermission(cancelToken);
                    _settingService?.GetCompanySettingAsync(cancelToken);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return Task.FromResult(true);

            //发送通知
            //await this.services.Notifications.Send(
            //    this.GetType(),
            //    false,
            //    "数据同步工作已完成",
            //    $"{jobInfo.Identifier} 完成"
            //);

        }

        public void Start() { }
        //public void Start()
        //    => this.services.Notifications.Register(this.GetType(), true, "主数据同步作业");
    }
}
