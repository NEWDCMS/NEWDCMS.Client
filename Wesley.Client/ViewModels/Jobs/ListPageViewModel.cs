using DCMS.Client.Services;
using Microsoft.Extensions.Logging;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny.Jobs;
using Shiny.Net;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace DCMS.Client.ViewModels
{
    public class ListPageViewModel : ViewModelBase
    {
        private readonly IJobManager _jobManager;
        public ICommand LoadJobs { get; }
        public ICommand CancelAllJobs { get; }
        public ICommand RunAllJobs { get; }
        public ICommand Create { get; }
        [Reactive] public List<CommandItem> Jobs { get; private set; }


        public ListPageViewModel(INavigationService navigationService,
            IJobManager jobManager,
            IDialogService dialogService,
            ILogger<ViewModelBase> logger, IConnectivity connectivity) : base(navigationService, dialogService, logger, connectivity)
        {
            this._jobManager = jobManager;

            //this.Create = _navigationService.NavigateCommand("CreateJob");

            this.LoadJobs = ReactiveCommand.CreateFromTask(async () =>
            {
                var jobs = await jobManager.GetJobs();
                this.Jobs = jobs
                    .Select(x => new CommandItem
                    {
                        Text = x.Type.Name,
                        Detail = $"上次运行：{x.LastRunUtc?.ToLocalTime().ToString("G") ?? "永久运行"}",
                        PrimaryCommand = ReactiveCommand.CreateFromTask(() => jobManager.Run(x.Identifier)),
                        SecondaryCommand = ReactiveCommand.CreateFromTask(async () =>
                        {
                            await jobManager.Cancel(x.Identifier);
                            this.LoadJobs.Execute(null);
                        })
                    })
                    .ToList();
            });
            this.BindBusyCommand(this.LoadJobs);

            this.RunAllJobs = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!await this.AssertJobs())
                    return;

                if (this._jobManager.IsRunning)
                {
                    _dialogService.ShortAlert("作业管理器已在运行");
                }
                else
                {
                    await this._jobManager.RunAll();
                    _dialogService.ShortAlert("作业批处理已启动");
                }
            });

            this.CancelAllJobs = ReactiveCommand.CreateFromTask(async _ =>
            {
                if (!await this.AssertJobs())
                    return;

                var confirm = await _dialogService.ShowConfirmAsync("是否确实要取消所有作业?");
                if (confirm)
                {
                    await this._jobManager.CancelAll();
                    this.LoadJobs.Execute(null);
                }
            });
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            this.LoadJobs.Execute(null);

            //this._jobManager
            //    .JobStarted
            //    .Subscribe(x =>
            //    {
            //        Device.BeginInvokeOnMainThread(() =>
            //        {
            //            _dialogService.ShortAlert($"作业 {x.Identifier} 已经开启");
            //            this.LoadJobs.Execute(null);
            //        });
            //    })
            //    .DisposedBy(this.DeactivateWith);

            //this._jobManager
            //    .JobFinished
            //    .Subscribe(x =>
            //    {
            //        Device.BeginInvokeOnMainThread(() =>
            //        {
            //            _dialogService.ShortAlert($"作业 {x.Job?.Identifier} 已经完成");
            //            this.LoadJobs.Execute(null);
            //        });
            //    })
            //    .DisposedBy(this.DeactivateWith);
        }

        private async Task<bool> AssertJobs()
        {
            var jobs = await this._jobManager.GetJobs();
            if (!jobs.Any())
            {
                _dialogService.ShortAlert("没有可运行Job");
                return false;
            }

            return true;
        }
    }
}
