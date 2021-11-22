using Wesley.ChartJS.Models;
using Wesley.Client.Models.Report;
using Wesley.Client.Models.Users;
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
    public class SalesRankingPageViewModel : ViewModelBaseChart<BusinessRanking>
    {

        public readonly IUserService _userService;

        [Reactive] public decimal? SaleAmount { get; set; }
        [Reactive] public decimal? SaleReturnAmount { get; set; }
        [Reactive] public decimal? NetAmount { get; set; }


        public SalesRankingPageViewModel(INavigationService navigationService,
               IProductService productService,
               IUserService userService,
               IReportingService reportingService,
               IDialogService dialogService

               ) : base(navigationService, productService, reportingService, dialogService)
        {

            Title = "业务员销售排行";
            this.PageType = Enums.ChartPageEnum.SalesRanking_Template;
            _userService = userService;

            this.WhenAnyValue(x => x.RankSeries)
                .Subscribe(x => { this.IsNull = x.Count == 0; })
                .DisposeWith(DeactivateWith);

            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    int? businessUserId = Filter.BusinessUserId;
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;

                    var businessUsers = new List<UserModel>();

                    //初始化 
                    var result = await _reportingService.GetBusinessRankingAsync(businessUserId,
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

        public void Refresh(List<BusinessRanking> analysis)
        {
            RankSeries = new ObservableCollection<BusinessRanking>(analysis);
            SaleAmount = analysis.Select(s => s.SaleAmount).Sum();
            SaleReturnAmount = analysis.Select(s => s.SaleReturnAmount).Sum();
            NetAmount = analysis.Select(s => s.NetAmount).Sum();

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
                    data = ChartDataProvider.GetSalesRanking(ranks)
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
