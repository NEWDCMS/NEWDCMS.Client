using Wesley.Client.Models;
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
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

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


            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "我的报表";

            _terminalService = terminalService;
            _reportingService = reportingService;


            this.Load = ChartsLoader.Load(async () =>
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
                                                    ChartData = chartData
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
                                                    ChartData = chartData
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
                                                    ChartData = chartData
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
                                                    ChartData = chartData
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
                                                    ChartData = chartData
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
                                                    ChartData = chartData
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
                                                    ChartData = chartData
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
                                                    ChartData = chartData
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
                                                    ChartData = chartData
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
                                                    ChartData = chartData
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
                                                    ChartData = chartData
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
                                                    ChartData = chartData
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
                                                    ChartData = chartData
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
            });

            this.AddCommand = ReactiveCommand.Create<object>(async e => await this.NavigateAsync("AddReportPage"));

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }


        /// <summary>
        /// 客户排行榜
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<Chart> CustomerRankingPage_Chart(Module app, string title)
        {
            Chart chartData = null;
            try
            {
                var result = await _reportingService.GetCustomerRankingAsync(0, 0, Settings.UserId, DateTime.Now.AddDays(-100), DateTime.Now, calToken: cts.Token);
                if (result != null)
                {
                    var series = result.ToList();

                    //#if DEBUG
                    //                    //模拟
                    //                    var random = new Random();

                    //                    series.Add(new CustomerRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "每一天便利店" + random.Next(1, 10),
                    //                        VisitSum = random.Next(0, 100),
                    //                        SaleAmount = random.Next(10, 1000),
                    //                        SaleReturnAmount = random.Next(20, 1000),
                    //                        NetAmount = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new CustomerRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "每一天便利店" + random.Next(1, 10),
                    //                        VisitSum = random.Next(0, 100),
                    //                        SaleAmount = random.Next(10, 1000),
                    //                        SaleReturnAmount = random.Next(20, 1000),
                    //                        NetAmount = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new CustomerRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "每一天便利店" + random.Next(1, 10),
                    //                        VisitSum = random.Next(0, 100),
                    //                        SaleAmount = random.Next(10, 1000),
                    //                        SaleReturnAmount = random.Next(20, 1000),
                    //                        NetAmount = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new CustomerRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "每一天便利店" + random.Next(1, 10),
                    //                        VisitSum = random.Next(0, 100),
                    //                        SaleAmount = random.Next(10, 1000),
                    //                        SaleReturnAmount = random.Next(20, 1000),
                    //                        NetAmount = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new CustomerRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "每一天便利店" + random.Next(1, 10),
                    //                        VisitSum = random.Next(0, 100),
                    //                        SaleAmount = random.Next(10, 1000),
                    //                        SaleReturnAmount = random.Next(20, 1000),
                    //                        NetAmount = random.Next(0, 1000)
                    //                    });
                    //#endif

                    //v3.0
                    var entries = new List<ChartEntry>();
                    int i = 0;
                    foreach (var t in series.Take(10))
                    {
                        entries.Add(new ChartEntry((float)(t?.NetAmount ?? 0))
                        {
                            Label = RemoveChar(t.TerminalName.Trim()),
                            ValueLabel = (t?.NetAmount ?? 0).ToString("#,##0.00"),
                            Color = ChartDataProvider.Colors[i],
                        });
                        i++;
                    }
                    chartData = ChartDataProvider.CreateBarChart(entries);

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
            direction = direction.Replace(" ", "");
            direction = System.Text.RegularExpressions.Regex.Replace(direction, @"\s+", string.Empty);
            return direction;
        }

        /// <summary>
        /// 业务销售排行
        /// </summary>
        /// <param name="app"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<Chart> SalesRankingPage_Chart(Module app, string title)
        {
            Chart chartData = null;
            try
            {
                var result = await _reportingService.GetBusinessRankingAsync(0, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    var series = result.ToList();

                    //#if DEBUG
                    //                    //模拟
                    //                    var random = new Random();
                    //                    series.Add(new BusinessRanking
                    //                    {
                    //                        BusinessUserId = random.Next(10, 1000),
                    //                        BusinessUserName = "马晓彤" + random.Next(1, 10),
                    //                        SaleAmount = random.Next(100, 10000),
                    //                        SaleReturnAmount = random.Next(20, 10000),
                    //                        NetAmount = random.Next(10, 1000),
                    //                        Profit = random.Next(0, 10)
                    //                    });
                    //                    series.Add(new BusinessRanking
                    //                    {
                    //                        BusinessUserId = random.Next(10, 1000),
                    //                        BusinessUserName = "马晓彤" + random.Next(1, 10),
                    //                        SaleAmount = random.Next(100, 10000),
                    //                        SaleReturnAmount = random.Next(20, 10000),
                    //                        NetAmount = random.Next(10, 1000),
                    //                        Profit = random.Next(0, 10)
                    //                    });
                    //                    series.Add(new BusinessRanking
                    //                    {
                    //                        BusinessUserId = random.Next(10, 1000),
                    //                        BusinessUserName = "马晓彤" + random.Next(1, 10),
                    //                        SaleAmount = random.Next(100, 10000),
                    //                        SaleReturnAmount = random.Next(20, 10000),
                    //                        NetAmount = random.Next(10, 1000),
                    //                        Profit = random.Next(0, 10)
                    //                    });
                    //                    series.Add(new BusinessRanking
                    //                    {
                    //                        BusinessUserId = random.Next(10, 1000),
                    //                        BusinessUserName = "马晓彤" + random.Next(1, 10),
                    //                        SaleAmount = random.Next(100, 10000),
                    //                        SaleReturnAmount = random.Next(20, 10000),
                    //                        NetAmount = random.Next(10, 1000),
                    //                        Profit = random.Next(0, 10)
                    //                    });
                    //                    series.Add(new BusinessRanking
                    //                    {
                    //                        BusinessUserId = random.Next(10, 1000),
                    //                        BusinessUserName = "马晓彤" + random.Next(1, 10),
                    //                        SaleAmount = random.Next(100, 10000),
                    //                        SaleReturnAmount = random.Next(20, 10000),
                    //                        NetAmount = random.Next(10, 1000),
                    //                        Profit = random.Next(0, 10)
                    //                    });
                    //#endif

                    //v3.0
                    var entries = new List<ChartEntry>();
                    int i = 0;
                    foreach (var t in series.Take(10))
                    {
                        entries.Add(new ChartEntry((float)(t?.NetAmount ?? 0))
                        {
                            Label = RemoveChar(t.BusinessUserName.Trim()),
                            ValueLabel = (t?.NetAmount ?? 0).ToString("#,##0.00"),
                            Color = ChartDataProvider.Colors[i]
                        });
                        i++;
                    }
                    chartData = ChartDataProvider.CreateBarChart(entries);

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
        private async Task<Chart> BrandRankingPage_Chart(Module app, string title)
        {
            Chart chartData = null;
            try
            {
                var result = await _reportingService.GetBrandRankingAsync(new[] { 0 }, Settings.UserId, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    var series = result.ToList();


                    //#if DEBUG
                    //                    //模拟
                    //                    var random = new Random();

                    //                    series.Add(new BrandRanking
                    //                    {
                    //                        BrandId = random.Next(10, 100),
                    //                        BrandName = "雪花啤酒" + random.Next(1, 10),
                    //                        Profit = random.Next(20, 100),
                    //                        SaleAmount = random.Next(100, 1000),
                    //                        SaleReturnAmount = random.Next(100, 10000),
                    //                        NetAmount = random.Next(100, 10000),
                    //                        Percentage = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new BrandRanking
                    //                    {
                    //                        BrandId = random.Next(10, 100),
                    //                        BrandName = "雪花啤酒" + random.Next(1, 10),
                    //                        Profit = random.Next(20, 100),
                    //                        SaleAmount = random.Next(100, 1000),
                    //                        SaleReturnAmount = random.Next(100, 10000),
                    //                        NetAmount = random.Next(100, 10000),
                    //                        Percentage = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new BrandRanking
                    //                    {
                    //                        BrandId = random.Next(10, 100),
                    //                        BrandName = "雪花啤酒" + random.Next(1, 10),
                    //                        Profit = random.Next(20, 100),
                    //                        SaleAmount = random.Next(100, 1000),
                    //                        SaleReturnAmount = random.Next(100, 10000),
                    //                        NetAmount = random.Next(100, 10000),
                    //                        Percentage = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new BrandRanking
                    //                    {
                    //                        BrandId = random.Next(10, 100),
                    //                        BrandName = "雪花啤酒" + random.Next(1, 10),
                    //                        Profit = random.Next(20, 100),
                    //                        SaleAmount = random.Next(100, 1000),
                    //                        SaleReturnAmount = random.Next(100, 10000),
                    //                        NetAmount = random.Next(100, 10000),
                    //                        Percentage = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new BrandRanking
                    //                    {
                    //                        BrandId = random.Next(10, 100),
                    //                        BrandName = "雪花啤酒" + random.Next(1, 10),
                    //                        Profit = random.Next(20, 100),
                    //                        SaleAmount = random.Next(100, 1000),
                    //                        SaleReturnAmount = random.Next(100, 10000),
                    //                        NetAmount = random.Next(100, 10000),
                    //                        Percentage = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new BrandRanking
                    //                    {
                    //                        BrandId = random.Next(10, 100),
                    //                        BrandName = "雪花啤酒" + random.Next(1, 10),
                    //                        Profit = random.Next(20, 100),
                    //                        SaleAmount = random.Next(100, 1000),
                    //                        SaleReturnAmount = random.Next(100, 10000),
                    //                        NetAmount = random.Next(100, 10000),
                    //                        Percentage = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new BrandRanking
                    //                    {
                    //                        BrandId = random.Next(10, 100),
                    //                        BrandName = "雪花啤酒" + random.Next(1, 10),
                    //                        Profit = random.Next(20, 100),
                    //                        SaleAmount = random.Next(100, 1000),
                    //                        SaleReturnAmount = random.Next(100, 10000),
                    //                        NetAmount = random.Next(100, 10000),
                    //                        Percentage = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new BrandRanking
                    //                    {
                    //                        BrandId = random.Next(10, 100),
                    //                        BrandName = "雪花啤酒" + random.Next(1, 10),
                    //                        Profit = random.Next(20, 100),
                    //                        SaleAmount = random.Next(100, 1000),
                    //                        SaleReturnAmount = random.Next(100, 10000),
                    //                        NetAmount = random.Next(100, 10000),
                    //                        Percentage = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new BrandRanking
                    //                    {
                    //                        BrandId = random.Next(10, 100),
                    //                        BrandName = "雪花啤酒" + random.Next(1, 10),
                    //                        Profit = random.Next(20, 100),
                    //                        SaleAmount = random.Next(100, 1000),
                    //                        SaleReturnAmount = random.Next(100, 10000),
                    //                        NetAmount = random.Next(100, 10000),
                    //                        Percentage = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new BrandRanking
                    //                    {
                    //                        BrandId = random.Next(10, 100),
                    //                        BrandName = "雪花啤酒" + random.Next(1, 10),
                    //                        Profit = random.Next(20, 100),
                    //                        SaleAmount = random.Next(100, 1000),
                    //                        SaleReturnAmount = random.Next(100, 10000),
                    //                        NetAmount = random.Next(100, 10000),
                    //                        Percentage = random.Next(0, 100)
                    //                    });
                    //#endif


                    //v3.0
                    var entries = new List<ChartEntry>();
                    int i = 0;
                    foreach (var t in series.Take(10))
                    {
                        entries.Add(new ChartEntry((float)(t?.NetAmount ?? 0))
                        {
                            Label = RemoveChar(t.BrandName.Trim()),
                            ValueLabel = (t?.NetAmount ?? 0).ToString("#,##0.00"),
                            Color = ChartDataProvider.Colors[i]
                        });
                        i++;
                    }
                    chartData = ChartDataProvider.CreateBarChart(entries);
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
        private async Task<Chart> HotSalesRankingPage_Chart(Module app, string title)
        {
            Chart chartData = null;
            try
            {
                var result = await _reportingService.GetHotSaleRankingAsync(0, 0, 0, 0, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    var series = result.ToList();


                    //#if DEBUG
                    //                    //模拟
                    //                    var random = new Random();

                    //                    series.Add(new HotSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "马尔斯绿" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new HotSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "马尔斯绿" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new HotSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "马尔斯绿" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new HotSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "马尔斯绿" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new HotSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "马尔斯绿" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //#endif

                    //v3.0
                    var entries = new List<ChartEntry>();
                    int i = 0;
                    foreach (var t in series.Take(10))
                    {
                        entries.Add(new ChartEntry((float)(t?.TotalSumNetQuantity ?? 0))
                        {
                            Label = RemoveChar(t.ProductName.Trim()),
                            ValueLabel = (t?.TotalSumNetQuantity ?? 0).ToString("#,##0.00"),
                            Color = ChartDataProvider.Colors[i]
                        });
                        i++;
                    }
                    chartData = ChartDataProvider.CreateHorizontalBarChart(entries);

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
        private async Task<Chart> SaleTrendChatPage_Chart(Module app, string title)
        {
            Chart chartData = null;
            try
            {
                var result = await _reportingService.GetSaleTrendingAsync("day", this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    var series = result.OrderBy(s => s.SaleDate).ToList();

                    //#if DEBUG
                    //                    //模拟
                    //                    var random = new Random();

                    //                    series.Add(new SaleTrending
                    //                    {
                    //                        DateType = "day",
                    //                        SaleDate = DateTime.Now.AddDays(random.Next(0, 10)),
                    //                        SaleAmount = random.Next(100, 10000),
                    //                        SaleReturnAmount = random.Next(20, 10000),
                    //                        NetAmount = random.Next(10, 1000)
                    //                    });
                    //                    series.Add(new SaleTrending
                    //                    {
                    //                        DateType = "day",
                    //                        SaleDate = DateTime.Now.AddDays(random.Next(0, 10)),
                    //                        SaleAmount = random.Next(100, 10000),
                    //                        SaleReturnAmount = random.Next(20, 10000),
                    //                        NetAmount = random.Next(10, 1000)
                    //                    });
                    //                    series.Add(new SaleTrending
                    //                    {
                    //                        DateType = "day",
                    //                        SaleDate = DateTime.Now.AddDays(random.Next(0, 10)),
                    //                        SaleAmount = random.Next(100, 10000),
                    //                        SaleReturnAmount = random.Next(20, 10000),
                    //                        NetAmount = random.Next(10, 1000)
                    //                    });
                    //                    series.Add(new SaleTrending
                    //                    {
                    //                        DateType = "day",
                    //                        SaleDate = DateTime.Now.AddDays(random.Next(0, 10)),
                    //                        SaleAmount = random.Next(100, 10000),
                    //                        SaleReturnAmount = random.Next(20, 10000),
                    //                        NetAmount = random.Next(10, 1000)
                    //                    });
                    //                    series.Add(new SaleTrending
                    //                    {
                    //                        DateType = "day",
                    //                        SaleDate = DateTime.Now.AddDays(random.Next(0, 10)),
                    //                        SaleAmount = random.Next(100, 10000),
                    //                        SaleReturnAmount = random.Next(20, 10000),
                    //                        NetAmount = random.Next(10, 1000)
                    //                    });
                    //#endif

                    //v3.0
                    var entries = new List<ChartEntry>();
                    int i = 0;
                    foreach (var t in series.Take(10))
                    {
                        entries.Add(new ChartEntry((float)(t?.NetAmount ?? 0))
                        {
                            Label = RemoveChar(t.SaleDateName.Trim()),
                            ValueLabel = (t?.NetAmount ?? 0).ToString("#,##0.00"),
                            Color = ChartDataProvider.Colors[i]
                        });
                        i++;
                    }
                    chartData = ChartDataProvider.CreateLineChart(entries);
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
        private async Task<Chart> UnsalablePage_Chart(Module app, string title)
        {
            Chart chartData = null;
            try
            {
                var result = await _reportingService.GetUnSaleRankingAsync(Settings.UserId, 0, 0, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    var series = result.ToList();

                    //#if DEBUG
                    //                    //模拟
                    //                    var random = new Random();

                    //                    series.Add(new UnSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "王老吉" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new UnSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "王老吉" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new UnSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "王老吉" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new UnSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "王老吉" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new UnSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "王老吉" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //#endif

                    //v3.0
                    var entries = new List<ChartEntry>();
                    int i = 0;
                    foreach (var t in series.Take(10))
                    {
                        //t.ProductName = t.ProductName.Length > 10 ? t.ProductName.Trim().Substring(0, 10) : t.ProductName.Trim();
                        entries.Add(new ChartEntry((float)(t?.TotalSumNetQuantity ?? 0))
                        {
                            Label = RemoveChar(t.ProductName.Trim()),
                            ValueLabel = (t?.TotalSumNetQuantity ?? 0).ToString("#,##0.00"),
                            Color = ChartDataProvider.Colors[i]
                        });
                        i++;
                    }
                    chartData = ChartDataProvider.CreateLineChart(entries);
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
        private async Task<Chart> VisitingRatePage_Chart(Module app, string title)
        {
            Chart chartData = null;
            try
            {
                var result = await _reportingService.GetCustomerVistAnalysisAsync(Settings.UserId, this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    var cdata = result;

                    ////统计图表
                    float up = 100;
                    if (cdata.TotalCustomer - cdata.Today.VistCount != 0)
                    {
                        if (cdata.TotalCustomer == 0) cdata.TotalCustomer = 1;
                        up = (float)((cdata.TotalCustomer - cdata.Today.VistCount) / cdata.TotalCustomer) * 100;
                    }

                    //v3.0
                    var entries = new List<ChartEntry>
                    {
                        new ChartEntry(cdata.TotalCustomer)
                        {
                            Label = RemoveChar("总客户数"),
                            ValueLabel = (cdata.TotalCustomer).ToString(),
                            Color = ChartDataProvider.Colors[0]
                        },
                        new ChartEntry((float)(cdata?.Today?.Percentage ?? 0))
                        {
                            Label = RemoveChar("拜访数"),
                            ValueLabel = (cdata?.Today?.Percentage ?? 0).ToString(),
                            Color = ChartDataProvider.Colors[1]
                        },
                        new ChartEntry((float)(up == 0 ? 100 : up))
                        {
                            Label = RemoveChar("未拜访数"),
                            ValueLabel = (up == 0 ? 100 : up).ToString(),
                            Color = ChartDataProvider.Colors[2]
                        }
                    };
                    chartData = ChartDataProvider.CreateDonutChart(entries);
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
        private async Task<Chart> SalesProfitRankingPage_Chart(Module app, string title)
        {
            Chart chartData = null;
            try
            {
                var result = await _reportingService.GetCostProfitRankingAsync(0, 0, 0, 0, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    var series = result.ToList();


                    //#if DEBUG
                    //                    //模拟
                    //                    var random = new Random();

                    //                    series.Add(new CostProfitRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "康师傅绿茶" + random.Next(1, 10),
                    //                        TotalSumNetQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(10, 10000),
                    //                        TotalSumProfit = random.Next(0, 10)
                    //                    });
                    //                    series.Add(new CostProfitRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "康师傅绿茶" + random.Next(1, 10),
                    //                        TotalSumNetQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(10, 10000),
                    //                        TotalSumProfit = random.Next(0, 10)
                    //                    });
                    //                    series.Add(new CostProfitRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "康师傅绿茶" + random.Next(1, 10),
                    //                        TotalSumNetQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(10, 10000),
                    //                        TotalSumProfit = random.Next(0, 10)
                    //                    });
                    //                    series.Add(new CostProfitRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "康师傅绿茶" + random.Next(1, 10),
                    //                        TotalSumNetQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(10, 10000),
                    //                        TotalSumProfit = random.Next(0, 10)
                    //                    });
                    //                    series.Add(new CostProfitRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "康师傅绿茶" + random.Next(1, 10),
                    //                        TotalSumNetQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(10, 10000),
                    //                        TotalSumProfit = random.Next(0, 10)
                    //                    });
                    //#endif

                    //v3.0
                    var entries = new List<ChartEntry>();
                    int i = 0;
                    foreach (var t in series.Take(10))
                    {
                        entries.Add(new ChartEntry((float)(t?.TotalSumProfit ?? 0))
                        {
                            Label = RemoveChar(t.ProductName.Trim()),
                            ValueLabel = (t?.TotalSumProfit ?? 0).ToString("#,##0.00"),
                            Color = ChartDataProvider.Colors[i]
                        });
                        i++;
                    }
                    chartData = ChartDataProvider.CreateHorizontalBarChart(entries);

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
        private async Task<Chart> SalesRatePage_Chart(Module app, string title)
        {
            Chart chartData = null;
            try
            {
                var result = await _reportingService.GetSaleAnalysisAsync(0, 0, 0, 0, this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    var cdata = result;

                    //统计图表
                    var total = cdata.Today?.SaleAmount ?? 0 + cdata.Yesterday?.SaleAmount ?? 0 + cdata.BeforeYesterday?.SaleAmount ?? 0 + cdata.LastWeek?.SaleAmount ?? 0 + cdata.ThisWeek?.SaleAmount ?? 0 + cdata.LastMonth?.SaleAmount ?? 0 + cdata.ThisMonth?.SaleAmount ?? 0 + cdata.ThisYear?.SaleAmount ?? 0;

                    float p1 = 0;
                    if (total > 0)
                        p1 = (float)((cdata?.Today?.SaleAmount) / total) * 100;

                    float p2 = 0;
                    if (total > 0)
                        p2 = (float)((cdata?.Yesterday?.SaleAmount) / total) * 100;

                    float p3 = 0;
                    if (total > 0)
                        p3 = (float)((cdata?.BeforeYesterday?.SaleAmount) / total) * 100;

                    float p4 = 0;
                    if (total > 0)
                        p4 = (float)((cdata?.LastWeek?.SaleAmount) / total) * 100;

                    float p5 = 0;
                    if (total > 0)
                        p5 = (float)((cdata?.ThisWeek?.SaleAmount) / total) * 100;

                    float p6 = 0;
                    if (total > 0)
                        p6 = (float)((cdata?.LastMonth?.SaleAmount) / total) * 100;

                    float p7 = 0;
                    if (total > 0)
                        p7 = (float)((cdata?.ThisMonth?.SaleAmount) / total) * 100;

                    float p8 = 0;
                    if (total > 0)
                        p8 = (float)((cdata?.ThisYear?.SaleAmount) / total) * 100;

                    //v3.0
                    var entries = new List<ChartEntry>
                    {
                        new ChartEntry(p1){Label = "今日",ValueLabel = p1.ToString("#,##0.00"),Color = ChartDataProvider.Colors[0]},
                        new ChartEntry(p2){Label = "昨天",ValueLabel = p2.ToString("#,##0.00"),Color = ChartDataProvider.Colors[1]},
                        new ChartEntry(p3){Label = "前天",ValueLabel = p3.ToString("#,##0.00"),Color = ChartDataProvider.Colors[2]},
                        new ChartEntry(p4){Label = "上周",ValueLabel = p4.ToString("#,##0.00"),Color = ChartDataProvider.Colors[3]},
                        new ChartEntry(p5){Label = "本周",ValueLabel = p5.ToString("#,##0.00"),Color = ChartDataProvider.Colors[4]},
                        new ChartEntry(p6){Label = "上月",ValueLabel = p6.ToString("#,##0.00"),Color = ChartDataProvider.Colors[5]},
                        new ChartEntry(p7){Label = "本月",ValueLabel = p7.ToString("#,##0.00"),Color = ChartDataProvider.Colors[6]},
                        new ChartEntry(p8){Label = "本年",ValueLabel = p8.ToString("#,##0.00"),Color = ChartDataProvider.Colors[7]}
                    };
                    chartData = ChartDataProvider.CreateRadarChart(entries);
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
        private async Task<Chart> NewCustomersPage_Chart(Module app, string title)
        {
            Chart chartData = null;
            try
            {

                var result = await _reportingService.GetNewCustomerAnalysisAsync(Filter.BusinessUserId, this.ForceRefresh, calToken: cts.Token);
                if (result != null && result.ChartDatas != null)
                {
                    var series = result.ChartDatas.ToList();
                    if (series.Count == 0)
                        return null;

                    //v3.0
                    var entries = new List<ChartEntry>();
                    int i = 0;
                    foreach (var t in series.Take(10))
                    {
                        entries.Add(new ChartEntry((float)(t.Value))
                        {
                            Label = RemoveChar(t.Key.Trim()),
                            ValueLabel = (t.Value).ToString("#,##0.00"),
                            Color = ChartDataProvider.Colors[i]
                        });
                        i++;
                    }
                    chartData = ChartDataProvider.CreateLineChart(entries);
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
        private async Task<Chart> CustomerVisitRankPage_Chart(Module app, string title)
        {
            Chart chartData = null;
            try
            {
                var result = await _terminalService.GetBusinessVisitRankingAsync(0, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    var series = result.ToList();
                    //v3.0
                    var entries = new List<ChartEntry>();
                    int i = 0;
                    foreach (var t in series.Take(10))
                    {
                        entries.Add(new ChartEntry(t?.VisitedCount ?? 0)
                        {
                            Label = RemoveChar(t.BusinessUserName.Trim()),
                            ValueLabel = (t?.VisitedCount ?? 0).ToString("#,##0.00"),
                            Color = ChartDataProvider.Colors[i]
                        });
                        i++;
                    }
                    chartData = ChartDataProvider.CreateHorizontalBarChart(entries);

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
        private async Task<Chart> CustomerActivityPage_Chart(Module app, string title)
        {
            Chart chartData = null;
            try
            {
                var result = await _terminalService.GetCustomerActivityRankingAsync(Settings.UserId, 0, this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    var series = result.ToList();

                    //#if DEBUG
                    //                    //模拟
                    //                    var random = new Random();

                    //                    series.Add(new CustomerActivityRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                    //                        VisitDaySum = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new CustomerActivityRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                    //                        VisitDaySum = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new CustomerActivityRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                    //                        VisitDaySum = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new CustomerActivityRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                    //                        VisitDaySum = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new CustomerActivityRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                    //                        VisitDaySum = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new CustomerActivityRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                    //                        VisitDaySum = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new CustomerActivityRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                    //                        VisitDaySum = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new CustomerActivityRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                    //                        VisitDaySum = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new CustomerActivityRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                    //                        VisitDaySum = random.Next(0, 100)
                    //                    });
                    //                    series.Add(new CustomerActivityRanking
                    //                    {
                    //                        TerminalId = random.Next(10, 1000),
                    //                        TerminalName = "小郡肝串串" + random.Next(1, 10),
                    //                        VisitDaySum = random.Next(0, 100)
                    //                    });
                    //#endif



                    //v3.0
                    var entries = new List<ChartEntry>();
                    int i = 0;
                    foreach (var t in series.Take(10))
                    {
                        entries.Add(new ChartEntry(t?.VisitDaySum ?? 0)
                        {
                            Label = RemoveChar(t.TerminalName.Trim()),
                            ValueLabel = (t?.VisitDaySum ?? 0).ToString("#,##0.00"),
                            Color = ChartDataProvider.Colors[i]
                        });
                        i++;
                    }
                    chartData = ChartDataProvider.CreateHorizontalBarChart(entries);

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
        private async Task<Chart> HotOrderRankingPage_Chart(Module app, string title)
        {
            Chart chartData = null;
            try
            {
                var result = await _reportingService.GetHotOrderRankingAsync(0, Settings.UserId, 0, 0, DateTime.Now.AddDays(-15), DateTime.Now, this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    var series = result.ToList();

                    //#if DEBUG
                    //                    //模拟
                    //                    var random = new Random();

                    //                    series.Add(new HotSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "马尔斯绿" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new HotSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "马尔斯绿" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new HotSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "马尔斯绿" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new HotSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "马尔斯绿" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //                    series.Add(new HotSaleRanking
                    //                    {
                    //                        ProductId = random.Next(10, 1000),
                    //                        ProductName = "马尔斯绿" + random.Next(1, 10),
                    //                        TotalSumSaleAmount = random.Next(0, 100),
                    //                        TotalSumSaleQuantity = random.Next(10, 100),
                    //                        TotalSumReturnAmount = random.Next(20, 100),
                    //                        TotalSumReturnQuantity = random.Next(0, 100),
                    //                        TotalSumNetAmount = random.Next(0, 1000),
                    //                        TotalSumNetQuantity = random.Next(0, 1000)
                    //                    });
                    //#endif

                    //v3.0
                    var entries = new List<ChartEntry>();
                    int i = 0;
                    foreach (var t in series.Take(10))
                    {
                        entries.Add(new ChartEntry((float)(t?.TotalSumNetAmount ?? 0))
                        {
                            Label = RemoveChar(t.ProductName.Trim()),
                            ValueLabel = (t?.TotalSumNetAmount ?? 0).ToString("#,##0.00"),
                            Color = ChartDataProvider.Colors[i]
                        });
                        i++;
                    }
                    chartData = ChartDataProvider.CreateHorizontalBarChart(entries);

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

        public override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
