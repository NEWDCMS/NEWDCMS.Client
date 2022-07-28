using DCMS.ChartJS.Models;
using DCMS.Client.Models.Report;
using DCMS.Client.Services;
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
using Xamarin.Forms;

namespace DCMS.Client.ViewModels
{

    public class SalesProfitRankingPageViewModel : ViewModelBaseChart<CostProfitRanking>
    {
        [Reactive] public decimal? TotalSumNetQuantity { get; set; }
        [Reactive] public decimal? TotalSumNetAmount { get; set; }
        [Reactive] public decimal? TotalSumProfit { get; set; }

        public SalesProfitRankingPageViewModel(INavigationService navigationService,
          IProductService productService,
          IReportingService reportingService,
            IDialogService dialogService
            ) : base(navigationService,
              productService,
              reportingService,
              dialogService)
        {
            Title = "销售利润排行";
            this.PageType = Enums.ChartPageEnum.SalesProfitRanking_Template;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DeactivateWith);
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
                    var result = await _reportingService.GetCostProfitRankingAsync(terminalId, businessUserId, brandId, categoryId, startTime, endTime, this.ForceRefresh, new System.Threading.CancellationToken());

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

        private void RefreshData(List<CostProfitRanking> analysis)
        {

            RankSeries = new ObservableCollection<CostProfitRanking>(analysis);
            TotalSumNetQuantity = analysis.Select(s => s.TotalSumNetQuantity).Sum();
            TotalSumNetAmount = analysis.Select(s => s.TotalSumNetAmount).Sum();
            TotalSumProfit = analysis.Select(s => s.TotalSumProfit).Sum();

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
                    data = ChartDataProvider.GetSalesProfitRanking(ranks)
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
