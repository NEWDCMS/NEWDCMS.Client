using Wesley.ChartJS.Models;
using Wesley.Client.Models.Census;
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
    public class CustomerVisitRankPageViewModel : ViewModelBaseChart<BusinessVisitRank>
    {

        private readonly ITerminalService _terminalService;
        public readonly IUserService _userService;

        [Reactive] public decimal? Total { get; set; }
        [Reactive] public decimal? SubTotal { get; set; }

        public CustomerVisitRankPageViewModel(INavigationService navigationService,
        IProductService productService,
        IReportingService reportingService,
        ITerminalService terminalService,
        IUserService userService,
          IDialogService dialogService
            ) : base(navigationService,
            productService,
            reportingService,
            dialogService)
        {
            Title = "客户拜访排行";

            this.PageType = Enums.ChartPageEnum.CustomerVisitRank_Template;

            _terminalService = terminalService;
            _userService = userService;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DeactivateWith);
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    var result = await _terminalService.GetBusinessVisitRankingAsync(Filter.BusinessUserId, Filter.StartTime, Filter.EndTime, this.ForceRefresh, new System.Threading.CancellationToken());

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


        private void RefreshData(List<BusinessVisitRank> analysis)
        {
            if (analysis == null)
            {
                return;
            }

            RankSeries = new ObservableCollection<BusinessVisitRank>(analysis);
            SubTotal = analysis.Select(s => s.VisitedCount).Sum();
            Total = analysis.Select(s => s.CustomerCount).Sum();

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
                    data = ChartDataProvider.GetCustomerVisitRank(ranks)
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
