using DCMS.ChartJS.Models;
using DCMS.Client.Models.Report;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
namespace DCMS.Client.ViewModels
{
    public class HotSalesRankingPageViewModel : ViewModelBaseChart<HotSaleRanking>
    {
        [Reactive] public decimal TotalSumReturnAmount { get; set; }
        [Reactive] public decimal TotalSumNetAmount { get; set; }

        [Reactive] public HotSaleRanking Selecter { get; set; }


        public HotSalesRankingPageViewModel(INavigationService navigationService,
          IProductService productService,
          IReportingService reportingService,
            IDialogService dialogService
            ) : base(navigationService,
              productService,
              reportingService,
              dialogService)
        {
            Title = "热销排行榜";

            this.PageType = Enums.ChartPageEnum.HotSalesRanking_Template;

            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
            .Skip(1)
            .Where(x => x != null)
            .SubOnMainThread(async item =>
            {
                if (item != null)
                {
                    var start = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01 00:00:00"));
                    await this.NavigateAsync("SaleDetailPage", ("ProductId", item.ProductId), ("TotalSumNetAmount", item.TotalSumNetAmount), ("TotalSumReturnAmount", item.TotalSumReturnAmount), ("ProductName", item.ProductName), ("Reference", PageName), ("BusinessUserId", Filter.BusinessUserId), ("StartTime", Filter.StartTime ?? start), ("EndTime", Filter.EndTime ?? DateTime.Now));
                }

                this.Selecter = null;
            })
            .DisposeWith(DeactivateWith);


            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    var rankings = new List<HotSaleRanking>();
                    int? terminalId = Filter.TerminalId;
                    int? businessUserId = Filter.BusinessUserId;
                    int? brandId = Filter.BrandId;
                    int? categoryId = Filter.CatagoryId;
                    DateTime? startTime = Filter.StartTime ?? DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01 00:00:00"));
                    DateTime? endTime = Filter.EndTime ?? DateTime.Now;

                    //初始化 
                    var result = await _reportingService.GetHotSaleRankingAsync(terminalId,
                        businessUserId,
                        brandId,
                        categoryId,
                        startTime.Value,
                        endTime.Value,
                        this.ForceRefresh,
                        new System.Threading.CancellationToken());

                    if (result != null)
                    {
                        RefreshData(result.ToList());
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }));

            //绑定页面菜单
            BindFilterDateMenus(true);

            this.BindBusyCommand(Load);

        }

        private void RefreshData(List<HotSaleRanking> analysis)
        {
            RankSeries = new ObservableCollection<HotSaleRanking>(analysis);
            TotalSumReturnAmount = analysis.Select(s => s.TotalSumReturnAmount ?? 0).Sum();
            TotalSumNetAmount = analysis.Select(s => s.TotalSumNetAmount ?? 0).Sum();

            var ranks = analysis.ToList();
            if (ranks.Count > 10)
            {
                ranks = ranks.Take(10).ToList();
            }

            var data = new ChartViewConfig()
            {
                BackgroundColor = Color.White,
                ChartConfig = new ChartConfig
                {
                    type = DCMS.ChartJS.ChartTypes.Bar,
                    data = ChartDataProvider.GetHotSalesRanking(ranks)
                }
            };
            ChartConfig = data;
        }


        public override void OnAppearing()
        {
            base.OnAppearing();

            _popupMenu?.Show(8, 10, 13, 14);

            ((ICommand)Load)?.Execute(null);
        }
    }
}
