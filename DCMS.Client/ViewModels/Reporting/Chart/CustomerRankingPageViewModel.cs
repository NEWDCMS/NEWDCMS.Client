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
    public class CustomerRankingPageViewModel : ViewModelBaseChart<CustomerRanking>
    {
        [Reactive] public decimal? Total { get; set; }
        [Reactive] public decimal? SubTotal { get; set; }


        public CustomerRankingPageViewModel(INavigationService navigationService,
           IProductService productService,
           IReportingService reportingService,
             IDialogService dialogService
            ) : base(navigationService,
               productService,
               reportingService,
               dialogService)
        {
            Title = "客户排行榜";

            this.PageType = Enums.ChartPageEnum.CustomerRanking_Template;

            this.WhenAnyValue(x => x.RankSeries)
                .Subscribe(x => { this.IsNull = x.Count == 0; })
                .DisposeWith(DeactivateWith);

            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    int? terminalId = Filter.TerminalId;
                    int? districtId = Filter.DistrictId;
                    int? businessUserId = Filter.BusinessUserId;
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;

                    var result = await _reportingService.GetCustomerRankingAsync(terminalId,
                        districtId,
                        businessUserId,
                        startTime,
                        endTime,
                        this.ForceRefresh,
                        new System.Threading.CancellationToken());

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

        public void Refresh(List<CustomerRanking> analysis)
        {
            RankSeries = new ObservableCollection<CustomerRanking>(analysis);
            SubTotal = analysis.Select(s => s.VisitSum).Sum();
            Total = analysis.Select(s => s.NetAmount).Sum();

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
                    data = ChartDataProvider.GetCustomerRanking(ranks)
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
