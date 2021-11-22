using Wesley.ChartJS.Models;
using Wesley.Client.Models;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Reactive.Disposables;


namespace Wesley.Client.ViewModels
{
    public class MyReportingPageViewModel : ViewModelBase
    {

        private readonly IReportingService _reportingService;
        private readonly ITerminalService _terminalService;

        [Reactive] public IList<MyReportingModel> ChartDatas { get; set; } = new ObservableCollection<MyReportingModel>();

        public MyReportingPageViewModel(INavigationService navigationService,
            ITerminalService terminalService,
            IReportingService reportingService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "我的报表";

            _terminalService = terminalService;
            _reportingService = reportingService;


            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                var charts = new List<MyReportingModel>();
                var series = GlobalSettings.ReportsDatas;
                var pending = new List<Task>();
                if (series != null)
                {
                    foreach (var chart in series)
                    {
                        switch (chart.Navigation)
                        {
                            case "CustomerRankingPage":
                                {
                                    //GetCustomerRankingAsync
                                    var task = await Task.Factory.StartNew((async () =>
                                    {
                                        try
                                        {
                                            var chartData = await CustomerRankingPage_Chart(chart, chart.Name);
                                            if (chartData != null)
                                            {
                                                charts.Add(new MyReportingModel
                                                {
                                                    Title = chart.Name,
                                                    ChartConfig = chartData
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Crashes.TrackError(ex);
                                        }
                                    }));

                                    pending.Add(task);

                                }
                                break;
                            case "SalesRankingPage":
                                {
                                    //GetBusinessRankingAsync
                                    var task = await Task.Factory.StartNew((async () =>
                                    {
                                        try
                                        {
                                            var chartData = await SalesRankingPage_Chart(chart, chart.Name);
                                            if (chartData != null)
                                            {
                                                charts.Add(new MyReportingModel
                                                {
                                                    Title = chart.Name,
                                                    ChartConfig = chartData
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Crashes.TrackError(ex);
                                        }
                                    }));

                                    pending.Add(task);
                                }
                                break;
                            case "BrandRankingPage":
                                {
                                    //GetBrandRankingAsync
                                    var task = await Task.Factory.StartNew((async () =>
                                    {
                                        try
                                        {
                                            var chartData = await BrandRankingPage_Chart(chart, chart.Name);
                                            if (chartData != null)
                                            {
                                                charts.Add(new MyReportingModel
                                                {
                                                    Title = chart.Name,
                                                    ChartConfig = chartData
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Crashes.TrackError(ex);
                                        }
                                    }));

                                    pending.Add(task);
                                }
                                break;
                            case "HotSalesRankingPage":
                                {
                                    //GetHotSaleRankingAsync
                                    var task = await Task.Factory.StartNew((async () =>
                                    {
                                        try
                                        {
                                            var chartData = await HotSalesRankingPage_Chart(chart, chart.Name);
                                            if (chartData != null)
                                            {
                                                charts.Add(new MyReportingModel
                                                {
                                                    Title = chart.Name,
                                                    ChartConfig = chartData
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Crashes.TrackError(ex);
                                        }
                                    }));

                                    pending.Add(task);
                                }
                                break;
                            case "SaleTrendChatPage":
                                {
                                    //GetSaleTrendingAsync
                                    var task = await Task.Factory.StartNew((async () =>
                                    {
                                        try
                                        {
                                            var chartData = await SaleTrendChatPage_Chart(chart, chart.Name);
                                            if (chartData != null)
                                            {
                                                charts.Add(new MyReportingModel
                                                {
                                                    Title = chart.Name,
                                                    ChartConfig = chartData
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Crashes.TrackError(ex);
                                        }
                                    }));

                                    pending.Add(task);
                                }
                                break;
                            case "UnsalablePage":
                                {
                                    //GetUnSaleRankingAsync
                                    var task = await Task.Factory.StartNew((async () =>
                                    {
                                        try
                                        {
                                            var chartData = await UnsalablePage_Chart(chart, chart.Name);
                                            if (chartData != null)
                                            {
                                                charts.Add(new MyReportingModel
                                                {
                                                    Title = chart.Name,
                                                    ChartConfig = chartData
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Crashes.TrackError(ex);
                                        }
                                    }));

                                    pending.Add(task);
                                }
                                break;
                            case "VisitingRatePage":
                                {
                                    //GetCustomerVistAnalysisAsync
                                    var task = await Task.Factory.StartNew((async () =>
                                    {
                                        try
                                        {
                                            var chartData = await VisitingRatePage_Chart(chart, chart.Name);
                                            if (chartData != null)
                                            {
                                                charts.Add(new MyReportingModel
                                                {
                                                    Title = chart.Name,
                                                    ChartConfig = chartData
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Crashes.TrackError(ex);
                                        }
                                    }));

                                    pending.Add(task);
                                }
                                break;
                            case "SalesProfitRankingPage":
                                {
                                    //SalesProfitRankingPage_Chart
                                    var task = await Task.Factory.StartNew((async () =>
                                    {
                                        try
                                        {
                                            var chartData = await SalesProfitRankingPage_Chart(chart, chart.Name);
                                            if (chartData != null)
                                            {
                                                charts.Add(new MyReportingModel
                                                {
                                                    Title = chart.Name,
                                                    ChartConfig = chartData
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Crashes.TrackError(ex);
                                        }
                                    }));

                                    pending.Add(task);
                                }
                                break;
                            case "SalesRatePage":
                                {
                                    //SalesRatePage_Chart
                                    var task = await Task.Factory.StartNew((async () =>
                                    {
                                        try
                                        {
                                            var chartData = await SalesRatePage_Chart(chart, chart.Name);
                                            if (chartData != null)
                                            {
                                                charts.Add(new MyReportingModel
                                                {
                                                    Title = chart.Name,
                                                    ChartConfig = chartData
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Crashes.TrackError(ex);
                                        }
                                    }));

                                    pending.Add(task);
                                }
                                break;
                            case "NewCustomersPage":
                                {
                                    //GetNewCustomerAnalysisAsync
                                    var task = await Task.Factory.StartNew((async () =>
                                    {
                                        try
                                        {
                                            var chartData = await NewCustomersPage_Chart(chart, chart.Name);
                                            if (chartData != null)
                                            {
                                                charts.Add(new MyReportingModel
                                                {
                                                    Title = chart.Name,
                                                    ChartConfig = chartData
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Crashes.TrackError(ex);
                                        }
                                    }));

                                    pending.Add(task);
                                }
                                break;
                            case "CustomerVisitRankPage":
                                {
                                    //GetBusinessVisitRankingAsync
                                    var task = await Task.Factory.StartNew((async () =>
                                    {
                                        try
                                        {
                                            var chartData = await CustomerVisitRankPage_Chart(chart, chart.Name);
                                            if (chartData != null)
                                            {
                                                charts.Add(new MyReportingModel
                                                {
                                                    Title = chart.Name,
                                                    ChartConfig = chartData
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Crashes.TrackError(ex);
                                        }
                                    }));

                                    pending.Add(task);
                                }
                                break;
                            case "CustomerActivityPage":
                                {
                                    //GetCustomerActivityRankingAsync
                                    var task = await Task.Factory.StartNew((async () =>
                                    {
                                        try
                                        {
                                            var chartData = await CustomerActivityPage_Chart(chart, chart.Name);
                                            if (chartData != null)
                                            {
                                                charts.Add(new MyReportingModel
                                                {
                                                    Title = chart.Name,
                                                    ChartConfig = chartData
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Crashes.TrackError(ex);
                                        }
                                    }));

                                    pending.Add(task);
                                }
                                break;
                            case "HotOrderRankingPage":
                                {
                                    //GetHotOrderRankingAsync
                                    var task = await Task.Factory.StartNew((async () =>
                                    {
                                        try
                                        {
                                            var chartData = await HotOrderRankingPage_Chart(chart, chart.Name);
                                            if (chartData != null)
                                            {
                                                charts.Add(new MyReportingModel
                                                {
                                                    Title = chart.Name,
                                                    ChartConfig = chartData
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Crashes.TrackError(ex);
                                        }
                                    }));

                                    pending.Add(task);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                //等待
                return await Task.WhenAll(pending.ToArray()).ContinueWith((s) =>
                {
                    this.ChartDatas = new ObservableCollection<MyReportingModel>(charts);
                    return charts;
                });
            }));

            this.AddCommand = ReactiveCommand.Create<object>(async e => await this.NavigateAsync("AddReportPage"));

            this.BindBusyCommand(Load);


            this.Load.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
            this.AddCommand.ThrownExceptions.Subscribe(ex => { Debug.Print(ex.StackTrace); }).DisposeWith(this.DeactivateWith);
        }


        /// <summary>
        /// 客户排行榜
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<ChartViewConfig> CustomerRankingPage_Chart(Module app, string title)
        {
            ChartViewConfig chartData = null;
            try
            {
                var result = await _reportingService.GetCustomerRankingAsync(0,
                    0,
                    Settings.UserId,
                    DateTime.Now.AddDays(-100),
                    DateTime.Now,
                    calToken: new System.Threading.CancellationToken());

                if (result != null && result.Any())
                {
                    var series = result.ToList();
                    if (series.Count > 50)
                        series = result.Take(50).ToList();

                    chartData = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = Wesley.ChartJS.ChartTypes.Bar,
                            data = ChartDataProvider.GetCustomerRanking(result.ToList())
                        }
                    };
                }
                return chartData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return chartData;
            }
        }

        private string RemoveChar(string direction)
        {
            if (string.IsNullOrEmpty(direction))
            {
                direction = direction.Replace(" ", "");
                direction = System.Text.RegularExpressions.Regex.Replace(direction, @"\s+", string.Empty);

            }
            return direction;
        }

        /// <summary>
        /// 业务销售排行
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<ChartViewConfig> SalesRankingPage_Chart(Module app, string title)
        {
            ChartViewConfig chartData = null;
            try
            {
                var result = await _reportingService.GetBusinessRankingAsync(0, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, new System.Threading.CancellationToken());
                if (result != null && result.Any())
                {
                    var series = result.ToList();
                    if (series.Count > 50)
                        series = result.Take(50).ToList();

                    if (series != null && series.Any())
                    {
                        chartData = new ChartViewConfig()
                        {
                            BackgroundColor = Color.White,
                            ChartConfig = new ChartConfig
                            {
                                type = Wesley.ChartJS.ChartTypes.Bar,
                                data = ChartDataProvider.GetSalesRanking(result.ToList())
                            }
                        };
                    }
                }
                return chartData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return chartData;
            }
        }

        /// <summary>
        /// 品牌销量汇总
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<ChartViewConfig> BrandRankingPage_Chart(Module app, string title)
        {
            ChartViewConfig chartData = null;
            try
            {
                var result = await _reportingService.GetBrandRankingAsync(new[] { 0 }, Settings.UserId, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, new System.Threading.CancellationToken());
                if (result != null && result.Any())
                {
                    var series = result.ToList();
                    if (series.Count > 50)
                        series = result.Take(50).ToList();

                    chartData = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = Wesley.ChartJS.ChartTypes.Bar,
                            data = ChartDataProvider.GetBrandRanking(series)
                        }
                    };
                }

                return chartData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return chartData;
            }
        }

        /// <summary>
        /// 热销排行榜
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<ChartViewConfig> HotSalesRankingPage_Chart(Module app, string title)
        {
            ChartViewConfig chartData = null;
            try
            {
                var result = await _reportingService.GetHotSaleRankingAsync(0, 0, 0, 0, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, new System.Threading.CancellationToken());
                if (result != null && result.Any())
                {
                    var series = result.ToList();
                    if (series.Count > 50)
                        series = result.Take(50).ToList();

                    chartData = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = Wesley.ChartJS.ChartTypes.Bar,
                            data = ChartDataProvider.GetHotSalesRanking(series)
                        }
                    };

                }
                return chartData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return chartData;
            }
        }

        /// <summary>
        /// 销量走势图
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<ChartViewConfig> SaleTrendChatPage_Chart(Module app, string title)
        {
            ChartViewConfig chartData = null;
            try
            {
                var result = await _reportingService.GetSaleTrendingAsync("day", this.ForceRefresh, new System.Threading.CancellationToken());
                if (result != null && result.Any())
                {
                    var series = result.ToList();
                    if (series.Count > 50)
                        series = result.Take(50).ToList();

                    chartData = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = Wesley.ChartJS.ChartTypes.Line,
                            data = ChartDataProvider.GetSaleTrendChat(series),
                        }
                    };
                }
                return chartData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return chartData;
            }
        }

        /// <summary>
        /// 库存滞销报表
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<ChartViewConfig> UnsalablePage_Chart(Module app, string title)
        {
            ChartViewConfig chartData = null;
            try
            {
                var result = await _reportingService.GetUnSaleRankingAsync(Settings.UserId, 0, 0, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, new System.Threading.CancellationToken());
                if (result != null && result.Any())
                {
                    var series = result.ToList();
                    if (series.Count > 50)
                        series = result.Take(50).ToList();

                    chartData = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = Wesley.ChartJS.ChartTypes.Line,
                            data = ChartDataProvider.GetUnsalable(series),
                        }
                    };
                }

                return chartData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return chartData;
            }
        }

        /// <summary>
        /// 客户拜访分析
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<ChartViewConfig> VisitingRatePage_Chart(Module app, string title)
        {
            ChartViewConfig chartData = null;
            try
            {
                var result = await _reportingService.GetCustomerVistAnalysisAsync(Settings.UserId, this.ForceRefresh, new System.Threading.CancellationToken());
                if (result != null)
                {
                    chartData = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = Wesley.ChartJS.ChartTypes.Pie,
                            data = ChartDataProvider.GetVisitingRate(result)
                        }
                    };

                }
                return chartData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return chartData;
            }
        }

        /// <summary>
        /// 销售利润排行
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<ChartViewConfig> SalesProfitRankingPage_Chart(Module app, string title)
        {
            ChartViewConfig chartData = null;
            try
            {
                var result = await _reportingService.GetCostProfitRankingAsync(0, 0, 0, 0, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, new System.Threading.CancellationToken());
                if (result != null && result.Any())
                {
                    var series = result.ToList();
                    if (series.Count > 50)
                        series = result.Take(50).ToList();

                    chartData = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = Wesley.ChartJS.ChartTypes.Bar,
                            data = ChartDataProvider.GetSalesProfitRanking(series)
                        }
                    };

                }
                return chartData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return chartData;
            }
        }

        /// <summary>
        /// 销售额分析
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<ChartViewConfig> SalesRatePage_Chart(Module app, string title)
        {
            ChartViewConfig chartData = null;
            try
            {
                var result = await _reportingService.GetSaleAnalysisAsync(0, 0, 0, 0, this.ForceRefresh, new System.Threading.CancellationToken());
                if (result != null)
                {
                    var cdata = result;
                }
                return chartData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return chartData;
            }
        }

        /// <summary>
        /// 新增客户分析
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<ChartViewConfig> NewCustomersPage_Chart(Module app, string title)
        {
            ChartViewConfig chartData = null;
            try
            {

                var result = await _reportingService.GetNewCustomerAnalysisAsync(Filter.BusinessUserId, this.ForceRefresh, new System.Threading.CancellationToken());
                if (result != null && result.ChartDatas != null)
                {
                    var series = result.ChartDatas.ToList();
                    if (series.Count == 0)
                        return null;

                    chartData = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = Wesley.ChartJS.ChartTypes.Line,
                            data = ChartDataProvider.GetNewCustomers(result)
                        }
                    };

                }
                return chartData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return chartData;
            }
        }

        /// <summary>
        /// 客户拜访排行
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<ChartViewConfig> CustomerVisitRankPage_Chart(Module app, string title)
        {
            ChartViewConfig chartData = null;
            try
            {
                var result = await _terminalService.GetBusinessVisitRankingAsync(0, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, new System.Threading.CancellationToken());
                if (result != null && result.Any())
                {
                    var series = result.ToList();
                    if (series.Count > 50)
                        series = result.Take(50).ToList();

                    chartData = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = Wesley.ChartJS.ChartTypes.Bar,
                            data = ChartDataProvider.GetCustomerVisitRank(series)
                        }
                    };
                }
                return chartData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return chartData;
            }
        }

        /// <summary>
        /// 客户活跃度
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<ChartViewConfig> CustomerActivityPage_Chart(Module app, string title)
        {
            ChartViewConfig chartData = null;
            try
            {
                var result = await _terminalService.GetCustomerActivityRankingAsync(Settings.UserId, 0, this.ForceRefresh, new System.Threading.CancellationToken());
                if (result != null && result.Any())
                {
                    var series = result.ToList();
                    if (series.Count > 50)
                        series = result.Take(50).ToList();

                    chartData = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = Wesley.ChartJS.ChartTypes.Line,
                            data = ChartDataProvider.GetCustomerActivity(series),
                        }
                    };
                }
                return chartData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return chartData;
            }
        }

        /// <summary>
        /// 热定排行榜
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<ChartViewConfig> HotOrderRankingPage_Chart(Module app, string title)
        {
            ChartViewConfig chartData = null;
            try
            {
                var result = await _reportingService.GetHotOrderRankingAsync(0, Settings.UserId, 0, 0, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, new System.Threading.CancellationToken());
                if (result != null && result.Any())
                {
                    var series = result.ToList();
                    if (series.Count > 50)
                        series = result.Take(50).ToList();

                    chartData = new ChartViewConfig()
                    {
                        BackgroundColor = Color.White,
                        ChartConfig = new ChartConfig
                        {
                            type = Wesley.ChartJS.ChartTypes.Bar,
                            data = ChartDataProvider.GetHotOrderRanking(series)
                        }
                    };
                }
                return chartData;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return chartData;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            ((ICommand)Load)?.Execute(null);
        }
    }
}
