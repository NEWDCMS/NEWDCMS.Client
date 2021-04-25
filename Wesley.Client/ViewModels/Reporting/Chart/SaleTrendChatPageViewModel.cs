using Wesley.Client.Models.Report;
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
    public class SaleTrendChatPageViewModel : ViewModelBaseChart<SaleTrending>
    {
        [Reactive] public string DateType { get; internal set; }
        [Reactive] public decimal? TotalAmount { get; internal set; }


        public SaleTrendChatPageViewModel(INavigationService navigationService,
           IProductService productService,
           IReportingService reportingService,
             IDialogService dialogService) : base(navigationService,
               productService,
               reportingService,


               dialogService)
        {
            Title = "销量走势图";
            this.PageType = Enums.ChartPageEnum.SaleTrendChat_Template;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DestroyWith);
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    //初始化 
                    var result = await _reportingService.GetSaleTrendingAsync(string.IsNullOrEmpty(DateType) ? "day" : DateType, this.ForceRefresh, calToken: cts.Token);
                    if (result != null)
                    {
                        RefreshData(result.ToList());
                    }


#if DEBUG
                    //模拟
                    var random = new Random();
                    var series = new List<SaleTrending>();

                    series.Add(new SaleTrending
                    {
                        DateType = "day",
                        SaleDate = DateTime.Now.AddDays(random.Next(0, 10)),
                        SaleAmount = random.Next(100, 10000),
                        SaleReturnAmount = random.Next(20, 10000),
                        NetAmount = random.Next(10, 1000)
                    });
                    series.Add(new SaleTrending
                    {
                        DateType = "day",
                        SaleDate = DateTime.Now.AddDays(random.Next(0, 10)),
                        SaleAmount = random.Next(100, 10000),
                        SaleReturnAmount = random.Next(20, 10000),
                        NetAmount = random.Next(10, 1000)
                    });
                    series.Add(new SaleTrending
                    {
                        DateType = "day",
                        SaleDate = DateTime.Now.AddDays(random.Next(0, 10)),
                        SaleAmount = random.Next(100, 10000),
                        SaleReturnAmount = random.Next(20, 10000),
                        NetAmount = random.Next(10, 1000)
                    });
                    series.Add(new SaleTrending
                    {
                        DateType = "day",
                        SaleDate = DateTime.Now.AddDays(random.Next(0, 10)),
                        SaleAmount = random.Next(100, 10000),
                        SaleReturnAmount = random.Next(20, 10000),
                        NetAmount = random.Next(10, 1000)
                    });
                    series.Add(new SaleTrending
                    {
                        DateType = "day",
                        SaleDate = DateTime.Now.AddDays(random.Next(0, 10)),
                        SaleAmount = random.Next(100, 10000),
                        SaleReturnAmount = random.Next(20, 10000),
                        NetAmount = random.Next(10, 1000)
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
               switch (x)
               {
                   case Enums.MenuEnum.TODAY:
                       {
                           DateType = "DAY";
                       }
                       break;
                   case Enums.MenuEnum.MONTH:
                       {
                           DateType = "MONTH";
                       }
                       break;
                   case Enums.MenuEnum.THISWEEBK:
                       {
                           DateType = "WEEK";
                       }
                       break;
               }
           }, 8, 10, 13);

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }


        private void RefreshData(List<SaleTrending> series)
        {
            int i = 0;
            Random random = new Random();
            if (series == null || series.Count == 0)
            {
                for (i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
                {
                    series.Add(new SaleTrending() { DateType = "DAY", NetAmount = random.Next(200), SaleAmount = random.Next(200), SaleDateName = DateTime.Now.AddDays(i).ToString("yyyy-MM-dd"), SaleReturnAmount = random.Next(200), SaleDate = DateTime.Now.AddDays(i) });
                }
            }

            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            series.ForEach(d =>
            {
                if ("DAY" == d.DateType.ToUpper())
                {
                    d.SaleDateName = d.SaleDate.ToString("yyyy/MM/dd");
                }
                else if ("WEEK" == d.DateType.ToUpper())
                {
                    d.SaleDateName = weekdays[Convert.ToInt32(d.SaleDate.DayOfWeek)];
                }
                else if ("MONTH" == d.DateType.ToUpper())
                {
                    d.SaleDateName = d.SaleDate.ToString("yyyy-MM");
                }
            });

            RankSeries = new ObservableCollection<SaleTrending>(series);
            TotalAmount = RankSeries.Select(s => s.NetAmount).Sum();


            var entries = new List<ChartEntry>();
            int j = 0;
            foreach (var t in RankSeries.Take(10))
            {
                entries.Add(new ChartEntry((float)(t?.NetAmount ?? 0))
                {
                    Label = t.SaleDateName,
                    ValueLabel = (t?.NetAmount ?? 0).ToString(),
                    Color = ChartDataProvider.Colors[j]
                });
                j++;
            }
            ChartData = ChartDataProvider.CreateLineChart(entries);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
