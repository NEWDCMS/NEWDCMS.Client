using Wesley.Client.Models.Census;
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
          IDialogService dialogService) : base(navigationService,
            productService,
            reportingService,
            dialogService)
        {
            Title = "客户拜访排行";

            this.PageType = Enums.ChartPageEnum.CustomerVisitRank_Template;

            _terminalService = terminalService;
            _userService = userService;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DestroyWith);
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    var result = await _terminalService.GetBusinessVisitRankingAsync(Filter.BusinessUserId, Filter.StartTime, Filter.EndTime, this.ForceRefresh, calToken: cts.Token);

                    if (result != null)
                    {
                        RefreshData(result.ToList());
                    }

#if DEBUG
                    //模拟
                    var random = new Random();
                    var series = new List<BusinessVisitRank>();

                    series.Add(new BusinessVisitRank
                    {
                        BusinessUserId = random.Next(10, 1000),
                        BusinessUserName = "刘冬冬" + random.Next(1, 10),
                        VisitedCount = random.Next(0, 100),
                        CustomerCount = random.Next(10, 100),
                        NoVisitedCount = random.Next(20, 100),
                        ExceptVisitCount = random.Next(0, 100)
                    });
                    series.Add(new BusinessVisitRank
                    {
                        BusinessUserId = random.Next(10, 1000),
                        BusinessUserName = "刘冬冬" + random.Next(1, 10),
                        VisitedCount = random.Next(0, 100),
                        CustomerCount = random.Next(10, 100),
                        NoVisitedCount = random.Next(20, 100),
                        ExceptVisitCount = random.Next(0, 100)
                    });
                    series.Add(new BusinessVisitRank
                    {
                        BusinessUserId = random.Next(10, 1000),
                        BusinessUserName = "刘冬冬" + random.Next(1, 10),
                        VisitedCount = random.Next(0, 100),
                        CustomerCount = random.Next(10, 100),
                        NoVisitedCount = random.Next(20, 100),
                        ExceptVisitCount = random.Next(0, 100)
                    });
                    series.Add(new BusinessVisitRank
                    {
                        BusinessUserId = random.Next(10, 1000),
                        BusinessUserName = "刘冬冬" + random.Next(1, 10),
                        VisitedCount = random.Next(0, 100),
                        CustomerCount = random.Next(10, 100),
                        NoVisitedCount = random.Next(20, 100),
                        ExceptVisitCount = random.Next(0, 100)
                    });
                    series.Add(new BusinessVisitRank
                    {
                        BusinessUserId = random.Next(10, 1000),
                        BusinessUserName = "刘冬冬" + random.Next(1, 10),
                        VisitedCount = random.Next(0, 100),
                        CustomerCount = random.Next(10, 100),
                        NoVisitedCount = random.Next(20, 100),
                        ExceptVisitCount = random.Next(0, 100)
                    });

                    RefreshData(series);
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
            }, 8, 9, 10, 13, 14);

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }


        private void RefreshData(List<BusinessVisitRank> series)
        {
            if (series == null)
            {
                return;
            }

            RankSeries = new ObservableCollection<BusinessVisitRank>(series);
            SubTotal = series.Select(s => s.VisitedCount).Sum();
            Total = series.Select(s => s.CustomerCount).Sum();

            var entries = new List<ChartEntry>();
            int i = 0;
            foreach (var t in RankSeries.Take(10))
            {
                entries.Add(new ChartEntry(t?.VisitedCount ?? 0)
                {
                    Label = t.BusinessUserName,
                    ValueLabel = (t?.VisitedCount ?? 0).ToString(),
                    Color = ChartDataProvider.Colors[i]
                });
                i++;
            }
            ChartData = ChartDataProvider.CreateHorizontalBarChart(entries);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
