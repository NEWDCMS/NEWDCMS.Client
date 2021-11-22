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
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{
    public class VisitingRatePageViewModel : ViewModelBase
    {
        public readonly IReportingService _reportingService;

        [Reactive] public CustomerVistAnalysis Data { get; set; } = new CustomerVistAnalysis();
        [Reactive] public ChartViewConfig BarConfig { get; set; }

        public VisitingRatePageViewModel(INavigationService navigationService,
            IReportingService reportingService,
              IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "客户拜访分析";
            _reportingService = reportingService;


            this.WhenAnyValue(x => x.Filter.BusinessUserId)
            .Where(x => x > 0)
            .Subscribe(x =>
            {
                ((ICommand)Load)?.Execute(null);
            }).DisposeWith(DeactivateWith);


            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                var analysis = await _reportingService.GetCustomerVistAnalysisAsync(Filter.BusinessUserId, this.ForceRefresh, new System.Threading.CancellationToken());
                if (analysis != null)
                {
                    Data = analysis;

                    var data = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = Wesley.ChartJS.ChartTypes.Bar,
                            data = GetChartData(analysis)
                        }
                    };
                    BarConfig = data;
                }
                return this.Data;
            }));

            //历史记录选择
            this.HistoryCommand = ReactiveCommand.Create<object>(async e =>
            {
                int tag = 0;
                int.TryParse(e.ToString(), out int type);
                switch (type)
                {
                    //今日拜访
                    case 1:
                        //Filter.StartTime = DateTime.Now;
                        //Filter.EndTime = DateTime.Now;
                        tag = 1;
                        break;
                    //昨天拜访
                    case 3:
                        //Filter.StartTime = DateTime.Now.AddDays(-1);
                        //Filter.EndTime = DateTime.Now;
                        tag = 3;
                        break;
                    //前天拜访
                    case 4:
                        //Filter.StartTime = DateTime.Now.AddDays(-2);
                        //Filter.EndTime = DateTime.Now;
                        tag = 4;
                        break;
                    //上周拜访
                    case 5:
                        //Filter.StartTime = DateTime.Now.AddDays(0 - Convert.ToInt16(DateTime.Now.DayOfWeek) - 6);
                        //Filter.EndTime = DateTime.Now.AddDays(6 - Convert.ToInt16(DateTime.Now.DayOfWeek) - 6);
                        tag = 5;
                        break;
                    //本周拜访
                    case 6:
                        //Filter.StartTime = DateTime.Now.AddDays(0 - Convert.ToInt16(DateTime.Now.DayOfWeek) + 1);
                        //Filter.EndTime = DateTime.Now.AddDays(6 - Convert.ToInt16(DateTime.Now.DayOfWeek) + 1);
                        tag = 6;
                        break;
                    //上月拜访
                    case 7:
                        //Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(-1);
                        //Filter.EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1);
                        tag = 7;
                        break;
                    //本月拜访
                    case 8:
                        //Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01"));
                        //Filter.EndTime = DateTime.Now;
                        tag = 8;
                        break;
                    //本年拜访
                    case 9:
                        //Filter.StartTime = new DateTime(DateTime.Now.Year, 1, 1);
                        //Filter.EndTime = DateTime.Now;
                        tag = 9;
                        break;
                }
                //VisitReportPage
                //await this.NavigateAsync($"{nameof(VisitRecordsPage)}", ("Filter", Filter));
                await this.NavigateAsync("VisitReportPage", ("Tag", tag));
            });

            this.BindBusyCommand(Load);

        }

        private ChartData GetChartData(CustomerVistAnalysis analysis)
        {
            var labels = new[] { "今日拜访", "昨天拜访", "前天拜访", "上周拜访", "本周拜访", "上月拜访", "本年拜访" }.ToList();
            var dataSets = new List<ChartNumberDataset>();

            var colors = RandomChartBuilder.GetDefaultColors();

            var datas = new int[]
            {
                analysis.Today?.VistCount ?? 0,
                analysis.Yesterday?.VistCount ?? 0,
                analysis.BeforeYesterday?.VistCount ?? 0,
                analysis.LastWeek?.VistCount ?? 0,
                analysis.ThisWeek?.VistCount ?? 0,
                analysis.LastMonth?.VistCount ?? 0,
                analysis.ThisYear?.VistCount ?? 0,
            };



            dataSets.Add(new ChartNumberDataset
            {
                type = Wesley.ChartJS.ChartTypes.Bar,
                label = "客户拜访分析",
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
