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
    public class BrandRankingPageViewModel : ViewModelBaseChart<BrandRanking>
    {
        [Reactive] public decimal? TotalAmount { get; set; }
        [Reactive] public double? TotalPercentage { get; set; }

        public BrandRankingPageViewModel(INavigationService navigationService,
            IProductService productService,
            IReportingService reportingService,
              IDialogService dialogService) : base(navigationService,
                productService,
                reportingService,
                dialogService)
        {
            Title = "品牌销量汇总";

            this.PageType = Enums.ChartPageEnum.BrandRanking_Template;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DestroyWith);
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    int[] brandIds = new int[] { Filter.BrandId };
                    int? businessUserId = Filter.BusinessUserId == 0 ? 0 : Filter.BusinessUserId;
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;

                    //初始化 
                    var result = await _reportingService.GetBrandRankingAsync(brandIds, businessUserId, startTime, endTime, this.ForceRefresh, calToken: cts.Token);
                    if (result != null)
                    {
                        Refresh(result.ToList());
                    }

#if DEBUG
                    //模拟
                    var random = new Random();
                    var series = new List<BrandRanking>();
                    series.Add(new BrandRanking
                    {
                        BrandId = random.Next(10, 100),
                        BrandName = "雪花啤酒" + random.Next(1, 10),
                        Profit = random.Next(20, 100),
                        SaleAmount = random.Next(100, 1000),
                        SaleReturnAmount = random.Next(100, 10000),
                        NetAmount = random.Next(100, 10000),
                        Percentage = random.Next(0, 100)
                    });
                    series.Add(new BrandRanking
                    {
                        BrandId = random.Next(10, 100),
                        BrandName = "雪花啤酒" + random.Next(1, 10),
                        Profit = random.Next(20, 100),
                        SaleAmount = random.Next(100, 1000),
                        SaleReturnAmount = random.Next(100, 10000),
                        NetAmount = random.Next(100, 10000),
                        Percentage = random.Next(0, 100)
                    });
                    series.Add(new BrandRanking
                    {
                        BrandId = random.Next(10, 100),
                        BrandName = "雪花啤酒" + random.Next(1, 10),
                        Profit = random.Next(20, 100),
                        SaleAmount = random.Next(100, 1000),
                        SaleReturnAmount = random.Next(100, 10000),
                        NetAmount = random.Next(100, 10000),
                        Percentage = random.Next(0, 100)
                    });
                    series.Add(new BrandRanking
                    {
                        BrandId = random.Next(10, 100),
                        BrandName = "雪花啤酒" + random.Next(1, 10),
                        Profit = random.Next(20, 100),
                        SaleAmount = random.Next(100, 1000),
                        SaleReturnAmount = random.Next(100, 10000),
                        NetAmount = random.Next(100, 10000),
                        Percentage = random.Next(0, 100)
                    });
                    series.Add(new BrandRanking
                    {
                        BrandId = random.Next(10, 100),
                        BrandName = "雪花啤酒" + random.Next(1, 10),
                        Profit = random.Next(20, 100),
                        SaleAmount = random.Next(100, 1000),
                        SaleReturnAmount = random.Next(100, 10000),
                        NetAmount = random.Next(100, 10000),
                        Percentage = random.Next(0, 100)
                    });
                    series.Add(new BrandRanking
                    {
                        BrandId = random.Next(10, 100),
                        BrandName = "雪花啤酒" + random.Next(1, 10),
                        Profit = random.Next(20, 100),
                        SaleAmount = random.Next(100, 1000),
                        SaleReturnAmount = random.Next(100, 10000),
                        NetAmount = random.Next(100, 10000),
                        Percentage = random.Next(0, 100)
                    });
                    series.Add(new BrandRanking
                    {
                        BrandId = random.Next(10, 100),
                        BrandName = "雪花啤酒" + random.Next(1, 10),
                        Profit = random.Next(20, 100),
                        SaleAmount = random.Next(100, 1000),
                        SaleReturnAmount = random.Next(100, 10000),
                        NetAmount = random.Next(100, 10000),
                        Percentage = random.Next(0, 100)
                    });
                    series.Add(new BrandRanking
                    {
                        BrandId = random.Next(10, 100),
                        BrandName = "雪花啤酒" + random.Next(1, 10),
                        Profit = random.Next(20, 100),
                        SaleAmount = random.Next(100, 1000),
                        SaleReturnAmount = random.Next(100, 10000),
                        NetAmount = random.Next(100, 10000),
                        Percentage = random.Next(0, 100)
                    });
                    series.Add(new BrandRanking
                    {
                        BrandId = random.Next(10, 100),
                        BrandName = "雪花啤酒" + random.Next(1, 10),
                        Profit = random.Next(20, 100),
                        SaleAmount = random.Next(100, 1000),
                        SaleReturnAmount = random.Next(100, 10000),
                        NetAmount = random.Next(100, 10000),
                        Percentage = random.Next(0, 100)
                    });
                    series.Add(new BrandRanking
                    {
                        BrandId = random.Next(10, 100),
                        BrandName = "雪花啤酒" + random.Next(1, 10),
                        Profit = random.Next(20, 100),
                        SaleAmount = random.Next(100, 1000),
                        SaleReturnAmount = random.Next(100, 10000),
                        NetAmount = random.Next(100, 10000),
                        Percentage = random.Next(0, 100)
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



        public void Refresh(List<BrandRanking> brands)
        {
            if (brands == null || brands.Count == 0)
            {
                return;
            }

            brands.ForEach(b =>
            {
                b.Percentage *= 100;
            });

            RankSeries = new ObservableCollection<BrandRanking>(brands.OrderByDescending(b => b.Percentage));

            TotalAmount = brands.Select(s => s.NetAmount ?? 0).Sum();
            TotalPercentage = brands.Select(s => s.Percentage ?? 0).Sum();

            var entries = new List<ChartEntry>();
            int i = 0;
            foreach (var t in brands.Take(10))
            {
                entries.Add(new ChartEntry((float)(t?.NetAmount ?? 0))
                {
                    Label = t.BrandName,
                    ValueLabel = (t?.NetAmount ?? 0).ToString("#,##0.00"),
                    Color = ChartDataProvider.Colors[i]
                });
                i++;
            }
            ChartData = ChartDataProvider.CreateBarChart(entries);
        }




        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
