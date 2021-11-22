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
    public class BrandRankingPageViewModel : ViewModelBaseChart<BrandRanking>
    {
        [Reactive] public decimal? TotalAmount { get; set; }
        [Reactive] public double? TotalPercentage { get; set; }

        public BrandRankingPageViewModel(INavigationService navigationService,
            IProductService productService,
            IReportingService reportingService,
              IDialogService dialogService
            ) : base(navigationService,
                productService,
                reportingService,
                dialogService)
        {
            Title = "品牌销量汇总";

            this.PageType = Enums.ChartPageEnum.BrandRanking_Template;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DeactivateWith);
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    int[] brandIds = new int[] { Filter.BrandId };
                    int? businessUserId = Filter.BusinessUserId == 0 ? 0 : Filter.BusinessUserId;
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;

                    //初始化 
                    var result = await _reportingService.GetBrandRankingAsync(brandIds, businessUserId, startTime, endTime, this.ForceRefresh, new System.Threading.CancellationToken());
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


        public void Refresh(List<BrandRanking> analysis)
        {
            if (analysis == null || analysis.Count == 0)
            {
                return;
            }

            analysis.ForEach(b =>
            {
                b.Percentage *= 100;
            });

            RankSeries = new ObservableCollection<BrandRanking>(analysis.OrderByDescending(b => b.Percentage));
            TotalAmount = analysis.Select(s => s.NetAmount ?? 0).Sum();
            TotalPercentage = analysis.Select(s => s.Percentage ?? 0).Sum();

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
                    data = ChartDataProvider.GetBrandRanking(ranks)
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
