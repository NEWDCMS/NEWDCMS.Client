using Wesley.Client.Models.Report;
using Wesley.Client.Services;
using Wesley.Easycharts;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
namespace Wesley.Client.ViewModels
{
    public class CustomerRankingPageViewModel : ViewModelBaseChart<CustomerRanking>
    {

        [Reactive] public decimal? Total { get; set; }
        [Reactive] public decimal? SubTotal { get; set; }
        public CustomerRankingPageViewModel(INavigationService navigationService,
           IProductService productService,
           IReportingService reportingService,
             IDialogService dialogService) : base(navigationService,
               productService,
               reportingService,


               dialogService)
        {
            Title = "客户排行榜";

            this.PageType = Enums.ChartPageEnum.CustomerRanking_Template;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DestroyWith);
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    int? terminalId = Filter.TerminalId;
                    int? districtId = Filter.DistrictId;
                    int? businessUserId = Filter.BusinessUserId;
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;

                    var result = await _reportingService.GetCustomerRankingAsync(terminalId, districtId, businessUserId, startTime, endTime, this.ForceRefresh, calToken: cts.Token);
                    if (result != null)
                    {
                        Refresh(result.ToList());
                    }

#if DEBUG
                    //模拟
                    var random = new Random();
                    var series = new List<CustomerRanking>();

                    series.Add(new CustomerRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "每一天便利店" + random.Next(1, 10),
                        VisitSum = random.Next(0, 100),
                        SaleAmount = random.Next(10, 1000),
                        SaleReturnAmount = random.Next(20, 1000),
                        NetAmount = random.Next(0, 1000)
                    });
                    series.Add(new CustomerRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "每一天便利店" + random.Next(1, 10),
                        VisitSum = random.Next(0, 100),
                        SaleAmount = random.Next(10, 1000),
                        SaleReturnAmount = random.Next(20, 1000),
                        NetAmount = random.Next(0, 1000)
                    });
                    series.Add(new CustomerRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "每一天便利店" + random.Next(1, 10),
                        VisitSum = random.Next(0, 100),
                        SaleAmount = random.Next(10, 1000),
                        SaleReturnAmount = random.Next(20, 1000),
                        NetAmount = random.Next(0, 1000)
                    });
                    series.Add(new CustomerRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "每一天便利店" + random.Next(1, 10),
                        VisitSum = random.Next(0, 100),
                        SaleAmount = random.Next(10, 1000),
                        SaleReturnAmount = random.Next(20, 1000),
                        NetAmount = random.Next(0, 1000)
                    });
                    series.Add(new CustomerRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "每一天便利店" + random.Next(1, 10),
                        VisitSum = random.Next(0, 100),
                        SaleAmount = random.Next(10, 1000),
                        SaleReturnAmount = random.Next(20, 1000),
                        NetAmount = random.Next(0, 1000)
                    });
                    Refresh(series);
#endif
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

            }));

            //菜单选择
            this.SetMenus((x) =>
            {
                this.HitFilterDate(x, () => { ((ICommand)Load)?.Execute(null); });
            }, 8, 10, 14);

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }


        public void Refresh(List<CustomerRanking> series)
        {

            RankSeries = new ObservableCollection<CustomerRanking>(series);
            SubTotal = series.Select(s => s.VisitSum).Sum();
            Total = series.Select(s => s.NetAmount).Sum();

            var entries = new List<ChartEntry>();
            int i = 0;
            foreach (var t in RankSeries.Take(10))
            {
                entries.Add(new ChartEntry((float)(t?.NetAmount ?? 0))
                {
                    Label = t.TerminalName,
                    ValueLabel = (t?.NetAmount ?? 0).ToString("#,##0.00"),
                    Color = ChartDataProvider.Colors[i]
                });
                i++;
            }
            ChartData = ChartDataProvider.CreateHorizontalBarChart(entries);
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
