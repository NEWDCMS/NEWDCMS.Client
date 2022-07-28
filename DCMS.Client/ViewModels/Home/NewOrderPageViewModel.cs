using DCMS.ChartJS.Models;
using DCMS.Client.Models.Report;
using DCMS.Client.Services;
using DCMS.Infrastructure.Helpers;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DCMS.Client.ViewModels
{
    public class NewOrderPageViewModel : ViewModelBaseCutom
    {
        private readonly IReportingService _reportingService;
        [Reactive] public NewOrderAnalysis Data { get; set; } = new NewOrderAnalysis();
        public ReactiveCommand<object, Unit> ViewDetailCommand { get; }
        [Reactive] public ChartViewConfig LineConfig { get; set; }

        public NewOrderPageViewModel(INavigationService navigationService,
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
            Title = "今日新增订单";

            _reportingService = reportingService;

            this.WhenAnyValue(x => x.Data)
                .Subscribe(x =>
                {
                    this.IsNull = (x == null);
                }).DisposeWith(DeactivateWith);

            this.WhenAnyValue(x => x.Filter.BusinessUserId)
                .Where(x => x > 0)
                .Subscribe(x =>
                {
                    ((ICommand)Load)?.Execute(null);
                }).DisposeWith(DeactivateWith);


            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                var analysis = await _reportingService.GetNewOrderAnalysisAsync(Filter.BusinessUserId, this.ForceRefresh, new System.Threading.CancellationToken());

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
            this.ViewDetailCommand = ReactiveCommand.Create<object>(async e =>
            {
                if (e != null)
                {
                    int.TryParse(e.ToString(), out int type);
                    switch (type)
                    {
                        //今日订单
                        case 1:
                            Filter.StartTime = DateTime.Now;
                            Filter.EndTime = DateTime.Now;
                            break;
                        //昨天订单
                        case 2:
                            Filter.StartTime = DateTime.Now.AddDays(-1);
                            Filter.EndTime = DateTime.Now;
                            break;
                        //前天订单
                        case 3:
                            Filter.StartTime = DateTime.Now.AddDays(-2);
                            Filter.EndTime = DateTime.Now;
                            break;
                        //上周订单
                        case 4:
                            Filter.StartTime = DateTime.Now.AddDays(0 - Convert.ToInt16(DateTime.Now.DayOfWeek) - 6);
                            Filter.EndTime = DateTime.Now.AddDays(6 - Convert.ToInt16(DateTime.Now.DayOfWeek) - 6);
                            break;
                        //本周订单
                        case 5:
                            Filter.StartTime = DateTime.Now.AddDays(0 - Convert.ToInt16(DateTime.Now.DayOfWeek) + 1);
                            Filter.EndTime = DateTime.Now.AddDays(6 - Convert.ToInt16(DateTime.Now.DayOfWeek) + 1);
                            break;
                        //上月订单
                        case 6:
                            Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(-1);
                            Filter.EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1);
                            break;
                        //本月订单
                        case 7:
                            Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01"));
                            Filter.EndTime = DateTime.Now;
                            break;
                        //本年订单
                        case 8:
                            Filter.StartTime = new DateTime(DateTime.Now.Year, 1, 1);
                            Filter.EndTime = DateTime.Now;
                            break;
                    }
                    await this.NavigateAsync("SaleOrderSummeryPage", ("Filter", Filter), ("Reference", this.PageName));
                }
            });

            this.BindBusyCommand(Load);
        }

        private ChartData GetChartData(NewOrderAnalysis analysis)
        {
            var labels = analysis.ChartDatas.Keys.Select(s => s).ToList();
            var dataSets = new List<ChartNumberDataset>();

            var colors = RandomChartBuilder.GetDefaultColors();

            var datas = analysis.ChartDatas.Values.Select(s => Convert.ToInt32(s)).ToList();

            dataSets.Add(new ChartNumberDataset
            {
                type = DCMS.ChartJS.ChartTypes.Line,
                label = "新增订单",
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
