using Wesley.Client.Models.Report;
using Wesley.Client.Services;
using Wesley.Easycharts;

using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class NewOrderPageViewModel : ViewModelBaseCutom
    {
        private readonly IReportingService _reportingService;
        [Reactive] public Chart ChartData { get; set; } = null;
        [Reactive] public NewOrderAnalysis Data { get; set; } = new NewOrderAnalysis();
        public ReactiveCommand<object, Unit> ViewDetailCommand { get; }


        public NewOrderPageViewModel(INavigationService navigationService,
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
            Title = "今日新增订单";

            _reportingService = reportingService;

            this.WhenAnyValue(x => x.Data)
                .Subscribe(x =>
                {
                    this.IsNull = (x == null);
                }).DisposeWith(DestroyWith);

            this.WhenAnyValue(x => x.Filter.BusinessUserId)
                .Where(x => x > 0)
                .Subscribe(x =>
                {
                    ((ICommand)Load)?.Execute(null);
                }).DisposeWith(DestroyWith);


            this.Load = ReactiveCommand.Create(() => Task.Run(async () =>
            {
                var result = await _reportingService.GetNewOrderAnalysisAsync(Filter.BusinessUserId, this.ForceRefresh, calToken: cts.Token);
                if (result != null && result.ChartDatas != null)
                {
                    Data = result;
                    ChartData = CreateLineChart(result.ChartDatas);
                }
            }));

            //历史记录选择
            this.ViewDetailCommand = ReactiveCommand.Create<object>(async e =>
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
            });


            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public Chart CreateLineChart(Dictionary<string, double> datas)
        {
            var entries = new List<ChartEntry>();
            int i = 0;
            foreach (var item in datas)
            {
                entries.Add(new ChartEntry((float)(item.Value))
                {
                    Label = item.Key,
                    ValueLabel = item.Value.ToString(),
                    Color = ChartDataProvider.Colors[i]
                });
                i++;
            }
            return ChartDataProvider.CreateLineChart(entries);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            if (ChartData == null)
                ((ICommand)Load)?.Execute(null);
        }
    }
}
