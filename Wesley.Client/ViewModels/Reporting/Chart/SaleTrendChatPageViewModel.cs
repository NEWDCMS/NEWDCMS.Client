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
    public class SaleTrendChatPageViewModel : ViewModelBaseChart<SaleTrending>
    {
        [Reactive] public string DateType { get; internal set; }
        [Reactive] public decimal? TotalAmount { get; internal set; }


        public SaleTrendChatPageViewModel(INavigationService navigationService,
           IProductService productService,
           IReportingService reportingService,
             IDialogService dialogService
            ) : base(navigationService,
               productService,
               reportingService,
               dialogService)
        {
            Title = "销量走势图";
            this.PageType = Enums.ChartPageEnum.SaleTrendChat_Template;

            this.WhenAnyValue(x => x.RankSeries).Subscribe(x => { this.IsNull = x.Count == 0; }).DisposeWith(DeactivateWith);
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    //初始化 
                    var result = await _reportingService.GetSaleTrendingAsync(string.IsNullOrEmpty(DateType) ? "day" : DateType, this.ForceRefresh, new System.Threading.CancellationToken());
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
                    data = ChartDataProvider.GetSaleTrendChat(ranks),
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
