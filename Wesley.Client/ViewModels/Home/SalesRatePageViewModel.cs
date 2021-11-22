using Wesley.ChartJS.Models;
using Wesley.Client.Models.Report;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Wesley.ChartJS.Models;
using Wesley.Client.Models.Report;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{
    public class SalesRatePageViewModel : ViewModelBaseCutom
    {
        public readonly IReportingService _reportingService;
        [Reactive] public SaleAnalysis Data { get; set; } = new SaleAnalysis();
        [Reactive] public ChartViewConfig DoughnutConfig { get; set; }
        public ReactiveCommand<object, Unit> ViewDetailCommand { get; }


        public SalesRatePageViewModel(INavigationService navigationService,
                    IProductService productService,
                    IUserService userService,
                    ITerminalService terminalService,
                    IWareHousesService wareHousesService,
                    IAccountingService accountingService,
                    IReportingService reportingService,
                    IDialogService dialogService) : base(navigationService,
                        productService,
                        terminalService,
                        userService,
                        wareHousesService,
                        accountingService,
                        dialogService)
        {
            Title = "今日销售净额";
            _reportingService = reportingService;

            this.WhenAnyValue(x => x.Data)
                .Subscribe(x => { this.IsNull = (x == null); })
                .DisposeWith(DeactivateWith);


            this.WhenAnyValue(
                x => x.Filter.BusinessUserId,
                x => x.Filter.BrandId,
                x => x.Filter.ProductId,
                x => x.Filter.CatagoryId)
            .Where(a => a.Item1 > 0 || a.Item2 > 0 || a.Item3 > 0 || a.Item4 > 0)
            .Subscribe(x =>
            {
                ((ICommand)Load)?.Execute(null);

            }).DisposeWith(DeactivateWith);

            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                var businessUserId = Filter.BusinessUserId == 0 ? Settings.UserId : Filter.BusinessUserId;
                var analysis = await _reportingService.GetSaleAnalysisAsync(businessUserId,
                    Filter.BrandId,
                    Filter.ProductId,
                    Filter.CatagoryId,
                    true,
                    new System.Threading.CancellationToken());

                if (analysis != null)
                {
                    Data = analysis;

                    var data = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = Wesley.ChartJS.ChartTypes.Doughnut,
                            data = GetChartData(analysis)
                        }
                    };
                    DoughnutConfig = data;
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
                        //今日
                        case 1:
                            Filter.StartTime = DateTime.Now;
                            Filter.EndTime = DateTime.Now;
                            break;
                        //昨天
                        case 2:
                            Filter.StartTime = DateTime.Now.AddDays(-1);
                            Filter.EndTime = DateTime.Now;
                            break;
                        //前天
                        case 3:
                            Filter.StartTime = DateTime.Now.AddDays(-2);
                            Filter.EndTime = DateTime.Now;
                            break;
                        //上周
                        case 4:
                            Filter.StartTime = DateTime.Now.AddDays(0 - Convert.ToInt16(DateTime.Now.DayOfWeek) - 6);
                            Filter.EndTime = DateTime.Now.AddDays(6 - Convert.ToInt16(DateTime.Now.DayOfWeek) - 6);
                            break;
                        //本周
                        case 5:
                            Filter.StartTime = DateTime.Now.AddDays(0 - Convert.ToInt16(DateTime.Now.DayOfWeek) + 1);
                            Filter.EndTime = DateTime.Now.AddDays(6 - Convert.ToInt16(DateTime.Now.DayOfWeek) + 1);
                            break;
                        //上月
                        case 6:
                            Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(-1);
                            Filter.EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1);
                            break;
                        //本月
                        case 7:
                            Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01"));
                            Filter.EndTime = DateTime.Now;
                            break;
                        //本季
                        case 8:
                            var s = DateTime.Now.AddMonths(0 - (DateTime.Now.Month - 1) % 3).AddDays(1 - DateTime.Now.Day);
                            Filter.StartTime = s;  //本季度初  
                            Filter.EndTime = s.AddMonths(3).AddDays(-1);  //本季度末  
                            break;
                        //本年
                        case 9:
                            Filter.StartTime = new DateTime(DateTime.Now.Year, 1, 1);
                            Filter.EndTime = DateTime.Now;
                            break;
                    }
                    await this.NavigateAsync("SaleSummeryPage", ("Filter", Filter), ("Reference", this.PageName));
                }
            });

            this.BindBusyCommand(Load);
        }


        private ChartData GetChartData(SaleAnalysis analysis)
        {
            var labels = new[] { "今日净额", "昨天净额", "前天净额", "上周净额", "本周净额", "上月净额", "本季净额", "本年净额" }.ToList();
            var dataSets = new List<ChartDecimalDataset>();

            var colors = RandomChartBuilder.GetDefaultColors();

            var datas = new decimal[]
            {
                analysis.Today?.NetAmount ?? 0,
                analysis.Yesterday?.NetAmount ?? 0,
                analysis.BeforeYesterday?.NetAmount ?? 0,
                analysis.LastWeek?.NetAmount ?? 0,
                analysis.ThisWeek?.NetAmount ?? 0,
                analysis.LastMonth?.NetAmount ?? 0,
                analysis.ThisQuarter?.NetAmount ?? 0,
                analysis.ThisYear?.NetAmount ?? 0,
            };

            var descriptionChart = "今日销售净额";
            if ((analysis?.Today?.NetAmount ?? 0) > (analysis?.LastWeekSame?.NetAmount ?? 0))
            {
                descriptionChart = $"今日销售净额 {(analysis?.LastWeekSame?.NetAmount ?? 0)},比上周同期增加了 {((analysis?.Today?.NetAmount ?? 0) - (analysis?.LastWeekSame?.NetAmount ?? 0))}";
            }
            else if ((analysis?.Today?.NetAmount ?? 0) < (analysis?.LastWeekSame?.NetAmount ?? 0))
            {
                descriptionChart = $"今日销售净额 {(analysis?.LastWeekSame?.NetAmount ?? 0)},比上周同期减少了 {Math.Abs((analysis?.Today?.NetAmount ?? 0) - (analysis?.LastWeekSame?.NetAmount ?? 0))}";
            }

            dataSets.Add(new ChartDecimalDataset
            {
                type = Wesley.ChartJS.ChartTypes.Doughnut,
                label = descriptionChart,
                data = datas,
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
