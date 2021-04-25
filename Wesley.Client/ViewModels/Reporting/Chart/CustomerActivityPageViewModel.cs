using Wesley.Client.Models.Report;
using Wesley.Client.Services;
using Wesley.Easycharts;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
namespace Wesley.Client.ViewModels
{
    public class CustomerActivityPageViewModel : ViewModelBaseChart<CustomerActivityRanking>
    {

        private readonly ITerminalService _terminalService;
        public CustomerActivityPageViewModel(INavigationService navigationService,
           IProductService productService,
           ITerminalService terminalService,
           IReportingService reportingService,


           IDialogService dialogService) : base(navigationService,
               productService,
               reportingService,
               dialogService)
        {

            Title = "客户活跃度";
            this.PageType = Enums.ChartPageEnum.CustomerActivity_Template;

            _terminalService = terminalService;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DestroyWith);
            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    var result = await _terminalService.GetCustomerActivityRankingAsync(Settings.UserId, Filter.TerminalId, this.ForceRefresh, calToken: cts.Token);
                    if (result != null)
                    {
                        Refresh(result.ToList());
                    }

#if DEBUG
                    //模拟
                    var random = new Random();
                    var series = new List<CustomerActivityRanking>();

                    series.Add(new CustomerActivityRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                        VisitDaySum = random.Next(0, 100)
                    });
                    series.Add(new CustomerActivityRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                        VisitDaySum = random.Next(0, 100)
                    });
                    series.Add(new CustomerActivityRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                        VisitDaySum = random.Next(0, 100)
                    });
                    series.Add(new CustomerActivityRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                        VisitDaySum = random.Next(0, 100)
                    });
                    series.Add(new CustomerActivityRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                        VisitDaySum = random.Next(0, 100)
                    });
                    series.Add(new CustomerActivityRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                        VisitDaySum = random.Next(0, 100)
                    });
                    series.Add(new CustomerActivityRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                        VisitDaySum = random.Next(0, 100)
                    });
                    series.Add(new CustomerActivityRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                        VisitDaySum = random.Next(0, 100)
                    });
                    series.Add(new CustomerActivityRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                        VisitDaySum = random.Next(0, 100)
                    });
                    series.Add(new CustomerActivityRanking
                    {
                        TerminalId = random.Next(10, 1000),
                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                        VisitDaySum = random.Next(0, 100)
                    });
                    Refresh(series);
#endif
                }
                catch (Exception ex)
                {

                    Crashes.TrackError(ex);
                }
            });

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }


        public void Refresh(List<CustomerActivityRanking> series)
        {

            RankSeries = new ObservableCollection<CustomerActivityRanking>(series);

            var entries = new List<ChartEntry>();
            int i = 0;
            foreach (var t in RankSeries.Take(10))
            {
                entries.Add(new ChartEntry(t?.VisitDaySum ?? 0)
                {
                    Label = t.TerminalName,
                    ValueLabel = (t?.VisitDaySum ?? 0).ToString(),
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
