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

    public class SalesProfitRankingPageViewModel : ViewModelBaseChart<CostProfitRanking>
    {
        [Reactive] public decimal? TotalSumNetQuantity { get; set; }
        [Reactive] public decimal? TotalSumNetAmount { get; set; }
        [Reactive] public decimal? TotalSumProfit { get; set; }

        public SalesProfitRankingPageViewModel(INavigationService navigationService,
          IProductService productService,
          IReportingService reportingService,
            IDialogService dialogService) : base(navigationService,
              productService,
              reportingService,


              dialogService)
        {
            Title = "销售利润排行";
            this.PageType = Enums.ChartPageEnum.SalesProfitRanking_Template;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DestroyWith);
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {

                    var rankings = new List<CostProfitRanking>();

                    int? terminalId = Filter.TerminalId == 0 ? 0 : Filter.TerminalId;
                    int? businessUserId = Filter.BusinessUserId == 0 ? 0 : Filter.BusinessUserId;
                    int? brandId = Filter.BrandId == 0 ? 0 : Filter.BrandId;
                    int? categoryId = Filter.CatagoryId == 0 ? 0 : Filter.CatagoryId;
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;

                    //初始化 
                    var result = await _reportingService.GetCostProfitRankingAsync(terminalId, businessUserId, brandId, categoryId, startTime, endTime, this.ForceRefresh, calToken: cts.Token);

                    if (result != null)
                    {
                        RefreshData(result.ToList());
                    }

#if DEBUG
                    //模拟
                    var random = new Random();
                    var series = new List<CostProfitRanking>();

                    series.Add(new CostProfitRanking
                    {
                        ProductId = random.Next(10, 1000),
                        ProductName = "康师傅绿茶" + random.Next(1, 10),
                        TotalSumNetQuantity = random.Next(0, 100),
                        TotalSumNetAmount = random.Next(10, 10000),
                        TotalSumProfit = random.Next(0, 10)
                    });
                    series.Add(new CostProfitRanking
                    {
                        ProductId = random.Next(10, 1000),
                        ProductName = "康师傅绿茶" + random.Next(1, 10),
                        TotalSumNetQuantity = random.Next(0, 100),
                        TotalSumNetAmount = random.Next(10, 10000),
                        TotalSumProfit = random.Next(0, 10)
                    });
                    series.Add(new CostProfitRanking
                    {
                        ProductId = random.Next(10, 1000),
                        ProductName = "康师傅绿茶" + random.Next(1, 10),
                        TotalSumNetQuantity = random.Next(0, 100),
                        TotalSumNetAmount = random.Next(10, 10000),
                        TotalSumProfit = random.Next(0, 10)
                    });
                    series.Add(new CostProfitRanking
                    {
                        ProductId = random.Next(10, 1000),
                        ProductName = "康师傅绿茶" + random.Next(1, 10),
                        TotalSumNetQuantity = random.Next(0, 100),
                        TotalSumNetAmount = random.Next(10, 10000),
                        TotalSumProfit = random.Next(0, 10)
                    });
                    series.Add(new CostProfitRanking
                    {
                        ProductId = random.Next(10, 1000),
                        ProductName = "康师傅绿茶" + random.Next(1, 10),
                        TotalSumNetQuantity = random.Next(0, 100),
                        TotalSumNetAmount = random.Next(10, 10000),
                        TotalSumProfit = random.Next(0, 10)
                    });

                    RefreshData(series);
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
            }, 8, 9, 10, 14, 13);

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }


        private void RefreshData(IList<CostProfitRanking> series)
        {

            RankSeries = new ObservableCollection<CostProfitRanking>(series);
            TotalSumNetQuantity = series.Select(s => s.TotalSumNetQuantity).Sum();
            TotalSumNetAmount = series.Select(s => s.TotalSumNetAmount).Sum();
            TotalSumProfit = series.Select(s => s.TotalSumProfit).Sum();


            var entries = new List<ChartEntry>();
            int i = 0;
            foreach (var t in RankSeries.Take(10))
            {
                entries.Add(new ChartEntry((float)(t?.TotalSumProfit ?? 0))
                {
                    Label = t.ProductName,
                    ValueLabel = (t?.TotalSumNetQuantity ?? 0).ToString(),
                    Color = ChartDataProvider.Colors[i]
                });
                i++;
            }
            ChartData = ChartDataProvider.CreateBarChart(entries);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
