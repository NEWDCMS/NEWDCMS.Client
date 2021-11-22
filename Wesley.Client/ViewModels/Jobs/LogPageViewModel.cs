using DCMS.Client.Models;
using DCMS.Client.Services;
using Microsoft.Extensions.Logging;
using Prism.Navigation;
using ReactiveUI;
//using Shiny;
using Shiny.Infrastructure;
using Shiny.Jobs;
using Shiny.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DCMS.Client.ViewModels
{
    public class LogPageViewModel : AbstractLogViewModel<CommandItem>
    {
        private readonly IJobManager jobManager;
        private readonly LocalDatabase conn;
        private readonly ISerializer serializer;


        public LogPageViewModel(INavigationService navigationService,
            IJobManager jobManager,
            IDialogService dialogService,
            ISerializer serializer,
            LocalDatabase conn,
            ILogger<ViewModelBase> logger, IConnectivity connectivity) : base(navigationService, dialogService, logger, connectivity)
        {
            this.jobManager = jobManager;
            this.serializer = serializer;
            this.conn = conn;
        }


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            this.jobManager.JobStarted.Subscribe(_ => ((ICommand)Load).Execute(null))
                .DisposedBy(this.DeactivateWith);

            this.jobManager.JobFinished.Subscribe(_ => ((ICommand)Load).Execute(null))
                .DisposedBy(this.DeactivateWith);
        }


        protected override async Task<IEnumerable<CommandItem>> LoadLogs()
        {
            var logs = await this.conn
                .JobLogs
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();

            return logs.Select(x =>
            {
                var title = $"{x.JobIdentifier} ({x.JobType})";
                var msg = x.Started ? "Started" : "Finished";
                if (x.Error != null)
                    msg = $"ERROR - {x.Error}";

                msg += $" @ {x.Timestamp:G}";

                return new CommandItem
                {
                    Text = title,
                    Detail = msg,
                    PrimaryCommand = ReactiveCommand.CreateFromTask(async () =>
                    {
                        var job = await this.jobManager.GetJob(x.JobIdentifier);

                        var sb = new StringBuilder()
                            .AppendLine(msg)
                            .AppendLine($"Battery: {job.BatteryNotLow}")
                            .AppendLine($"Internet: {job.RequiredInternetAccess}")
                            .AppendLine($"Charging: {job.DeviceCharging}")
                            .AppendLine($"Repeat: {job.Repeat}");

                        if (!x.Parameters.IsEmpty())
                        {
                            var parameters = this.serializer.Deserialize<Dictionary<string, object>>(x.Parameters);
                            foreach (var p in parameters)
                                sb.AppendLine().Append($"{p.Key}: {p.Value}");
                        }
                        _dialogService.ShortAlert(sb.ToString());
                    })
                };
            });
        }

        protected override Task ClearLogs() => this.conn.DeleteAllAsync<JobLog>();
    }
}
