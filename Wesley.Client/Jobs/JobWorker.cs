using Shiny;
using Shiny.Jobs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wesley.Client.Services;


namespace Wesley.Client.Jobs
{
    public class JobWorker : IJob, IShinyStartupTask
    {
        readonly CoreDelegateServices services;
        public JobWorker(CoreDelegateServices services) => this.services = services;

        public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            await this.services.Notifications.Send(
                this.GetType(),
                true,
                "数据同步开始",
                $"{jobInfo.Identifier} Started"
            );

            //var seconds = jobInfo.Parameters.Get("SecondsToRun", 10);
            //await Task.Delay(TimeSpan.FromSeconds(seconds), cancelToken);

            var _globalService = App.Resolve<IGlobalService>();
            var _settingService = App.Resolve<ISettingService>();

            await _globalService?.GetAPPFeatures(calToken: cancelToken);

            await _globalService?.SynchronizationPermission(cancelToken);

            await _settingService?.GetCompanySettingAsync(cancelToken);

            await this.services.Notifications.Send(
                this.GetType(),
                false,
                "数据同步完成",
                $"{jobInfo.Identifier} Finished"
            );
        }

        public void Start()
            => this.services.Notifications.Register(this.GetType(), true, "Jobs");
    }
}
