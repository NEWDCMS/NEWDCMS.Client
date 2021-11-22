using Wesley.ChartJS.Models;
using Wesley.Client.Models.Report;
using Wesley.Client.Services;
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

namespace Wesley.Client.ViewModels
{
    public class HotOrderRankingPageViewModel : ViewModelBaseChart<HotSaleRanking>
    {
        [Reactive] public decimal TotalSumReturnAmount { get; set; }
        [Reactive] public decimal TotalSumNetAmount { get; set; }

        public HotOrderRankingPageViewModel(INavigationService navigationService,
               IProductService productService,
               IReportingService reportingService,
                 IDialogService dialogService
            ) : base(navigationService,
                   productService,
                   reportingService,
                   dialogService)
        {

            Title = "热订排行榜";
            this.PageType = Enums.ChartPageEnum.HotOrderRanking_Template;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DeactivateWith);
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    var rankings = new List<HotSaleRanking>();

                    int? terminalId = 0;
                    int? businessUserId = 0;
                    int? brandId = 0;
                    int? categoryId = 0;
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;

                    //初始化 
                    var result = await _reportingService.GetHotOrderRankingAsync(terminalId, businessUserId, brandId, categoryId, startTime, endTime, this.ForceRefresh, new System.Threading.CancellationToken());
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
                    type = Wesley.ChartJS.ChartTypes.Bar,
                    data = ChartDataProvider.GetHotOrderRanking(ranks)
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
