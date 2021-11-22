using Wesley.Client.Models.Report;
using Wesley.Client.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class OrderQuantityAnalysisPageViewModel : ViewModelBase
    {

        public readonly IReportingService _reportingService;

        [Reactive] public OrderQuantityAnalysis ChartData { get; set; } = new OrderQuantityAnalysis();

        [Reactive] public decimal TodayNetAmount { get; set; }
        [Reactive] public decimal NetAmountBalance { get; set; }
        [Reactive] public decimal LastWeekSameNetAmount { get; set; }


        public OrderQuantityAnalysisPageViewModel(INavigationService navigationService,
            IReportingService reportingService,
              IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "订单额分析";

            _reportingService = reportingService;

            this.Load = ReactiveCommand.Create(() => Task.Run(async () =>
            {
                int? businessUserId = Filter.BusinessUserId;
                int? brandId = Filter.BrandId;
                int? productId = Filter.ProductId;
                int? categoryId = Filter.CatagoryId;

#if DEBUG
                await Task.Delay(0);
                //模拟
                var random = new Random();
                var data = new OrderQuantityAnalysis()
                {
                    Today = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                    LastWeekSame = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                    Yesterday = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                    BeforeYesterday = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                    LastWeek = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                    ThisWeek = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                    LastMonth = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                    ThisMonth = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                    ThisQuarter = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                    ThisYear = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) }
                };

                TodayNetAmount = data.Today.NetAmount;
                NetAmountBalance = data.LastWeekSame.NetAmount - data.Today.NetAmount;
                LastWeekSameNetAmount = data.LastWeekSame.NetAmount;

                ChartData = data;
#else
                //初始化 
                var data = await _reportingService.GetOrderQuantityAnalysisAsync(businessUserId, brandId, productId, categoryId,calToken: new System.Threading.CancellationToken());
                if (data != null)
                {
                    data.NetAmountBalance = data.LastWeekSame.NetAmount - data.Today.NetAmount;
                    TodayNetAmount = data.Today.NetAmount;
                    NetAmountBalance = data.LastWeekSame.NetAmount - data.Today.NetAmount;
                    LastWeekSameNetAmount = data.LastWeekSame.NetAmount;

                    ChartData = data;
                }
#endif

            }));

            this.BindBusyCommand(Load);

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
