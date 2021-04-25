using Wesley.Client.Models.Report;
using Wesley.Client.Pages.Archive;
using Wesley.Client.Services;
using Wesley.Easycharts;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class VisitingRatePageViewModel : ViewModelBase
    {
        public readonly IReportingService _reportingService;
        [Reactive] public Chart ChartData { get; set; } = null;
        [Reactive] public CustomerVistAnalysis Data { get; set; } = new CustomerVistAnalysis();

        public VisitingRatePageViewModel(INavigationService navigationService,
            IReportingService reportingService,
              IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "客户拜访分析";
            _reportingService = reportingService;


            this.WhenAnyValue(x => x.Filter.BusinessUserId)
            .Where(x => x > 0)
            .Subscribe(x =>
            {
                ((ICommand)Load)?.Execute(null);
            }).DisposeWith(DestroyWith);


            this.Load = ReactiveCommand.Create(() => Task.Run(async () =>
            {
                var result = await _reportingService.GetCustomerVistAnalysisAsync(Filter.BusinessUserId, this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    Data = result;
                    ChartData = CreateRadarChart(this.Data);
                }

            }));

            //历史记录选择
            this.HistoryCommand = ReactiveCommand.Create<object>(async e =>
            {
                int.TryParse(e.ToString(), out int type);
                switch (type)
                {
                    //今日拜访
                    case 1:
                        Filter.StartTime = DateTime.Now;
                        Filter.EndTime = DateTime.Now;
                        break;
                    //昨天拜访
                    case 2:
                        Filter.StartTime = DateTime.Now.AddDays(-1);
                        Filter.EndTime = DateTime.Now;
                        break;
                    //前天拜访
                    case 3:
                        Filter.StartTime = DateTime.Now.AddDays(-2);
                        Filter.EndTime = DateTime.Now;
                        break;
                    //上周拜访
                    case 4:
                        Filter.StartTime = DateTime.Now.AddDays(0 - Convert.ToInt16(DateTime.Now.DayOfWeek) - 6);
                        Filter.EndTime = DateTime.Now.AddDays(6 - Convert.ToInt16(DateTime.Now.DayOfWeek) - 6);
                        break;
                    //本周拜访
                    case 5:
                        Filter.StartTime = DateTime.Now.AddDays(0 - Convert.ToInt16(DateTime.Now.DayOfWeek) + 1);
                        Filter.EndTime = DateTime.Now.AddDays(6 - Convert.ToInt16(DateTime.Now.DayOfWeek) + 1);
                        break;
                    //上月拜访
                    case 6:
                        Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(-1);
                        Filter.EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1);
                        break;
                    //本月拜访
                    case 7:
                        Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01"));
                        Filter.EndTime = DateTime.Now;
                        break;
                    //本年拜访
                    case 8:
                        Filter.StartTime = new DateTime(DateTime.Now.Year, 1, 1);
                        Filter.EndTime = DateTime.Now;
                        break;
                }

                await this.NavigateAsync($"{nameof(VisitRecordsPage)}", ("Filter", Filter));
            });

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public Chart CreateRadarChart(CustomerVistAnalysis data)
        {
            var entries = new List<ChartEntry>
            {
                new ChartEntry(data?.Today?.VistCount??0)
                {
                    Label = "今日拜访",
                    ValueLabel = (data?.Today?.VistCount??0).ToString(),
                    Color = ChartDataProvider.Colors[0]
                },

                new ChartEntry(data?.Yesterday?.VistCount??0)
                {
                    Label = "昨天拜访",
                    ValueLabel = (data?.Yesterday?.VistCount??0).ToString(),
                    Color = ChartDataProvider.Colors[1]
                },

                new ChartEntry(data?.BeforeYesterday?.VistCount??0)
                {
                    Label = "前天拜访",
                    ValueLabel = (data?.BeforeYesterday?.VistCount??0).ToString(),
                    Color = ChartDataProvider.Colors[2]
                },

                new ChartEntry(data?.LastWeek?.VistCount??0)
                {
                    Label = "上周拜访",
                    ValueLabel = (data?.LastWeek?.VistCount??0).ToString(),
                    Color = ChartDataProvider.Colors[3]
                },

                new ChartEntry(data?.ThisWeek?.VistCount??0)
                {
                    Label = "本周拜访",
                    ValueLabel = (data?.ThisWeek?.VistCount??0).ToString(),
                    Color = ChartDataProvider.Colors[4]
                },

                new ChartEntry(data?.LastMonth?.VistCount??0)
                {
                    Label = "上月拜访",
                    ValueLabel = (data?.LastMonth?.VistCount??0).ToString(),
                    Color = ChartDataProvider.Colors[5]
                },

                new ChartEntry(data?.ThisMonth?.VistCount??0)
                {
                    Label = "本月拜访",
                    ValueLabel = (data?.ThisMonth?.VistCount??0).ToString(),
                    Color = ChartDataProvider.Colors[6]
                },

                new ChartEntry(data?.ThisYear?.VistCount??0)
                {
                    Label = "本年拜访",
                    ValueLabel = (data?.ThisYear?.VistCount??0).ToString(),
                    Color = ChartDataProvider.Colors[8]
                }
            };
            return ChartDataProvider.CreateRadarChart(entries);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            if (ChartData == null)
                ((ICommand)Load)?.Execute(null);
        }
    }
}
