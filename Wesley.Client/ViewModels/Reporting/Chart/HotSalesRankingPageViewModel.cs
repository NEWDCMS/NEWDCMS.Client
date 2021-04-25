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
    public class HotSalesRankingPageViewModel : ViewModelBaseChart<HotSaleRanking>
    {
        [Reactive] public decimal TotalSumReturnAmount { get; set; }
        [Reactive] public decimal TotalSumNetAmount { get; set; }

        public HotSalesRankingPageViewModel(INavigationService navigationService,
          IProductService productService,
          IReportingService reportingService,
            IDialogService dialogService) : base(navigationService,
              productService,
              reportingService,


              dialogService)
        {
            Title = "热销排行榜";
            this.PageType = Enums.ChartPageEnum.HotSalesRanking_Template;


            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DestroyWith);

            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    var rankings = new List<HotSaleRanking>();
                    int? terminalId = Filter.TerminalId;
                    int? businessUserId = Filter.BusinessUserId;
                    int? brandId = Filter.BrandId;
                    int? categoryId = Filter.CatagoryId;
                    DateTime? startTime = Filter.StartTime ?? DateTime.Now;
                    DateTime? endTime = Filter.EndTime ?? DateTime.Now;

                    //初始化 
                    var result = await _reportingService.GetHotSaleRankingAsync(terminalId, businessUserId, brandId, categoryId, startTime.Value, endTime.Value, this.ForceRefresh, calToken: cts.Token);
                    if (result != null)
                    {
                        RefreshData(result.ToList());
                    }

#if DEBUG
                    //模拟
                    var random = new Random();
                    var series = new List<HotSaleRanking>();

                    series.Add(new HotSaleRanking
                    {
                        ProductId = random.Next(10, 1000),
                        ProductName = "马尔斯绿" + random.Next(1, 10),
                        TotalSumSaleAmount = random.Next(0, 100),
                        TotalSumSaleQuantity = random.Next(10, 100),
                        TotalSumReturnAmount = random.Next(20, 100),
                        TotalSumReturnQuantity = random.Next(0, 100),
                        TotalSumNetAmount = random.Next(0, 1000),
                        TotalSumNetQuantity = random.Next(0, 1000)
                    });
                    series.Add(new HotSaleRanking
                    {
                        ProductId = random.Next(10, 1000),
                        ProductName = "马尔斯绿" + random.Next(1, 10),
                        TotalSumSaleAmount = random.Next(0, 100),
                        TotalSumSaleQuantity = random.Next(10, 100),
                        TotalSumReturnAmount = random.Next(20, 100),
                        TotalSumReturnQuantity = random.Next(0, 100),
                        TotalSumNetAmount = random.Next(0, 1000),
                        TotalSumNetQuantity = random.Next(0, 1000)
                    });
                    series.Add(new HotSaleRanking
                    {
                        ProductId = random.Next(10, 1000),
                        ProductName = "马尔斯绿" + random.Next(1, 10),
                        TotalSumSaleAmount = random.Next(0, 100),
                        TotalSumSaleQuantity = random.Next(10, 100),
                        TotalSumReturnAmount = random.Next(20, 100),
                        TotalSumReturnQuantity = random.Next(0, 100),
                        TotalSumNetAmount = random.Next(0, 1000),
                        TotalSumNetQuantity = random.Next(0, 1000)
                    });
                    series.Add(new HotSaleRanking
                    {
                        ProductId = random.Next(10, 1000),
                        ProductName = "马尔斯绿" + random.Next(1, 10),
                        TotalSumSaleAmount = random.Next(0, 100),
                        TotalSumSaleQuantity = random.Next(10, 100),
                        TotalSumReturnAmount = random.Next(20, 100),
                        TotalSumReturnQuantity = random.Next(0, 100),
                        TotalSumNetAmount = random.Next(0, 1000),
                        TotalSumNetQuantity = random.Next(0, 1000)
                    });
                    series.Add(new HotSaleRanking
                    {
                        ProductId = random.Next(10, 1000),
                        ProductName = "马尔斯绿" + random.Next(1, 10),
                        TotalSumSaleAmount = random.Next(0, 100),
                        TotalSumSaleQuantity = random.Next(10, 100),
                        TotalSumReturnAmount = random.Next(20, 100),
                        TotalSumReturnQuantity = random.Next(0, 100),
                        TotalSumNetAmount = random.Next(0, 1000),
                        TotalSumNetQuantity = random.Next(0, 1000)
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
            }, 8, 10, 14);

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }


        private void RefreshData(List<HotSaleRanking> series)
        {
            RankSeries = new ObservableCollection<HotSaleRanking>(series);

            TotalSumReturnAmount = series.Select(s => s.TotalSumReturnAmount ?? 0).Sum();
            TotalSumNetAmount = series.Select(s => s.TotalSumNetAmount ?? 0).Sum();

            var entries = new List<ChartEntry>();
            int i = 0;
            foreach (var t in RankSeries.Take(10))
            {
                entries.Add(new ChartEntry((float)(t?.TotalSumNetQuantity ?? 0))
                {
                    Label = t.ProductName,
                    ValueLabel = (t?.TotalSumNetQuantity ?? 0).ToString(),
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
