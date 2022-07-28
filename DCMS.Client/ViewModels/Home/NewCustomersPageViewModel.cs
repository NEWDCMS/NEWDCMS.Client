using DCMS.ChartJS.Models;
using DCMS.Client.Models.Report;
using DCMS.Client.Pages.Market;
using DCMS.Client.Services;
using DCMS.Infrastructure.Helpers;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DCMS.Client.ViewModels
{
    public class NewCustomersPageViewModel : ViewModelBaseCutom
    {
        private readonly IReportingService _reportingService;

        [Reactive] public NewCustomerAnalysis Data { get; set; } = new NewCustomerAnalysis();
        [Reactive] public ChartViewConfig LineConfig { get; set; }

        public NewCustomersPageViewModel(INavigationService navigationService,
                   IProductService productService,
                   IUserService userService,
                   ITerminalService terminalService,
                   IWareHousesService wareHousesService,
                   IAccountingService accountingService,
                   IReportingService reportingService,
                   IDialogService dialogService
                   ) : base(navigationService,
                       productService,
                       terminalService,
                       userService,
                       wareHousesService,
                       accountingService,
                       dialogService)
        {
            Title = "今日新增客户";

            _reportingService = reportingService;

            this.WhenAnyValue(x => x.Data)
                .Subscribe(x => { this.IsNull = (x == null); })
                .DisposeWith(DeactivateWith);

            this.WhenAnyValue(x => x.Filter.BusinessUserId)
            .Where(x => x > 0)
            .Subscribe(x =>
            {
                ((ICommand)Load)?.Execute(null);
            }).DisposeWith(DeactivateWith);

            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                var analysis = await _reportingService.GetNewCustomerAnalysisAsync(Filter.BusinessUserId,
                     this.ForceRefresh,
                     new System.Threading.CancellationToken());

                if (analysis != null)
                {
                    this.Data = analysis;

                    var data = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = DCMS.ChartJS.ChartTypes.Line,
                            data = GetChartData(analysis)
                        }
                    };
                    LineConfig = data;
                }
            }));



            //历史记录选择
            this.HistoryCommand = ReactiveCommand.Create<object>(async e => await this.NavigateAsync($"{nameof(CustomerArchivesPage)}", null));

            this.BindBusyCommand(Load);

        }

        private ChartData GetChartData(NewCustomerAnalysis analysis)
        {
            var labels = analysis.ChartDatas.Keys.Select(s => s).ToList();
            var dataSets = new List<ChartNumberDataset>();

            var colors = RandomChartBuilder.GetDefaultColors();

            var datas = analysis.ChartDatas.Values.Select(s => Convert.ToInt32(s)).ToList();

            dataSets.Add(new ChartNumberDataset
            {
                type = DCMS.ChartJS.ChartTypes.Line,
                label = "新增客户",
                data = datas,
                tension = 0.4,
                backgroundColor = datas.Select((d, i) =>
                {
                    var color = colors[i % colors.Count];
                    return $"rgb({color.Item1},{color.Item2},{color.Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
