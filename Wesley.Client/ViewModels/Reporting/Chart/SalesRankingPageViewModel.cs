using Wesley.Client.Models.Report;
using Wesley.Client.Models.Users;
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
                 IDialogService dialogService) : base(navigationService,
                   productService,
                   reportingService,


                   dialogService)
        {
            Title = "业务员销售排行";
            this.PageType = Enums.ChartPageEnum.SalesRanking_Template;


            _userService = userService;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DestroyWith);
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    int? businessUserId = Filter.BusinessUserId;
                    DateTime? startTime = Filter.StartTime;
                    DateTime? endTime = Filter.EndTime;

                    var businessUsers = new List<UserModel>();
                    //初始化 
                    var result = await _reportingService.GetBusinessRankingAsync(businessUserId, startTime, endTime, this.ForceRefresh, calToken: cts.Token);
                    if (result != null)
                    {
                        Refresh(result.ToList());
                    }


#if DEBUG
                    //模拟
                    var random = new Random();
                    var series = new List<BusinessRanking>();

                    series.Add(new BusinessRanking
                    {
                        BusinessUserId = random.Next(10, 1000),
                        BusinessUserName = "马晓彤" + random.Next(1, 10),
                        SaleAmount = random.Next(100, 10000),
                        SaleReturnAmount = random.Next(20, 10000),
                        NetAmount = random.Next(10, 1000),
                        Profit = random.Next(0, 10)
                    });
                    series.Add(new BusinessRanking
                    {
                        BusinessUserId = random.Next(10, 1000),
                        BusinessUserName = "马晓彤" + random.Next(1, 10),
                        SaleAmount = random.Next(100, 10000),
                        SaleReturnAmount = random.Next(20, 10000),
                        NetAmount = random.Next(10, 1000),
                        Profit = random.Next(0, 10)
                    });
                    series.Add(new BusinessRanking
                    {
                        BusinessUserId = random.Next(10, 1000),
                        BusinessUserName = "马晓彤" + random.Next(1, 10),
                        SaleAmount = random.Next(100, 10000),
                        SaleReturnAmount = random.Next(20, 10000),
                        NetAmount = random.Next(10, 1000),
                        Profit = random.Next(0, 10)
                    });
                    series.Add(new BusinessRanking
                    {
                        BusinessUserId = random.Next(10, 1000),
                        BusinessUserName = "马晓彤" + random.Next(1, 10),
                        SaleAmount = random.Next(100, 10000),
                        SaleReturnAmount = random.Next(20, 10000),
                        NetAmount = random.Next(10, 1000),
                        Profit = random.Next(0, 10)
                    });
                    series.Add(new BusinessRanking
                    {
                        BusinessUserId = random.Next(10, 1000),
                        BusinessUserName = "马晓彤" + random.Next(1, 10),
                        SaleAmount = random.Next(100, 10000),
                        SaleReturnAmount = random.Next(20, 10000),
                        NetAmount = random.Next(10, 1000),
                        Profit = random.Next(0, 10)
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

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }



        public void Refresh(List<BusinessRanking> series)
        {
            RankSeries = new ObservableCollection<BusinessRanking>(series);
            SaleAmount = series.Select(s => s.SaleAmount).Sum();
            SaleReturnAmount = series.Select(s => s.SaleReturnAmount).Sum();
            NetAmount = series.Select(s => s.NetAmount).Sum();

            var entries = new List<ChartEntry>();
            int i = 0;
            foreach (var t in RankSeries.Take(10))
            {
                entries.Add(new ChartEntry((float)(t?.NetAmount ?? 0))
                {
                    Label = t.BusinessUserName,
                    ValueLabel = (t?.NetAmount ?? 0).ToString(),
                    Color = ChartDataProvider.Colors[i]
                });
                i++;
            }
            ChartData = ChartDataProvider.CreatePointChart(entries);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
