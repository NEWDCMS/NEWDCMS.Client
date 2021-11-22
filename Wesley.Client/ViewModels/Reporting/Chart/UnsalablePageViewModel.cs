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
    public class UnsalablePageViewModel : ViewModelBaseChart<UnSaleRanking>
    {
        [Reactive] public decimal? TotalSumReturnAmount { get; set; }
        [Reactive] public decimal? TotalSumNetAmount { get; set; }


        public UnsalablePageViewModel(INavigationService navigationService,
            IProductService productService,
            IReportingService reportingService,
              IDialogService dialogService
            ) : base(navigationService,
                productService,
                reportingService,
                dialogService)
        {
            Title = "库存滞销排行榜";

            this.PageType = Enums.ChartPageEnum.Unsalable_Template;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DeactivateWith);
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {

                    int? businessUserId = Filter.BusinessUserId;
                    int? brandId = Filter.BrandId;
                    int? categoryId = Filter.CatagoryId;
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;

                    //初始化 
                    var result = await _reportingService.GetUnSaleRankingAsync(businessUserId, brandId, categoryId, startTime, endTime, this.ForceRefresh, new System.Threading.CancellationToken());
                    if (result != null)
                    {
                        Refresh(result.ToList());
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

        public void Refresh(List<UnSaleRanking> series)
        {
            RankSeries = new ObservableCollection<UnSaleRanking>(series);
            TotalSumReturnAmount = series.Select(s => s.TotalSumReturnAmount).Sum();
            TotalSumNetAmount = series.Select(s => s.TotalSumNetAmount).Sum();


            var ranks = series.ToList();
            if (ranks.Count > 10)
            {
                ranks = ranks.Take(10).ToList();
            }

            var data = new ChartViewConfig()
            {
                BackgroundColor = Color.White,
                ChartConfig = new ChartConfig
                {
                    type = Wesley.ChartJS.ChartTypes.Line,
                    data = ChartDataProvider.GetUnsalable(ranks),
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
