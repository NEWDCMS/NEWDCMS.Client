using Wesley.Client.Models.Report;
using Wesley.Client.Services;
using Wesley.Easycharts;

using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
namespace Wesley.Client.ViewModels
{
    public class SalesRatePageViewModel : ViewModelBase
    {
        public readonly IReportingService _reportingService;

        [Reactive] public Chart ChartData { get; set; } = null;
        [Reactive] public SaleAnalysis Data { get; set; } = new SaleAnalysis();
        [Reactive] public string DescriptionChart { get; set; } = "今日销售净额0";


        public SalesRatePageViewModel(INavigationService navigationService,
            IReportingService reportingService,


            IDialogService dialogService) : base(navigationService,
                dialogService)
        {
            Title = "今日销售净额";
            _reportingService = reportingService;

            this.WhenAnyValue(x => x.Data)
                .Subscribe(x => { this.IsNull = (x == null); })
                .DisposeWith(DestroyWith);

            //this.WhenAny(
            //        x => x.NotificationTitle,
            //        x => x.NotificationMessage,
            //        x => x.ScheduledTime,
            //        (title, msg, sch) =>
            //            !title.GetValue().IsEmpty() &&
            //            !msg.GetValue().IsEmpty() &&
            //            sch.GetValue() > DateTime.Now
            //    )

            this.WhenAnyValue(
                x => x.Filter.BusinessUserId,
                x => x.Filter.BrandId,
                x => x.Filter.ProductId,
                x => x.Filter.CatagoryId)
            .Where(a => a.Item1 > 0 || a.Item2 > 0 || a.Item3 > 0 || a.Item4 > 0)
            .Subscribe(x =>
            {
                ((ICommand)Load)?.Execute(null);

            }).DisposeWith(DestroyWith);

            this.Load = ReactiveCommand.Create(() => Task.Run(async () =>
            {
                var businessUserId = Filter.BusinessUserId == 0 ? Settings.UserId : Filter.BusinessUserId;
                var result = await _reportingService.GetSaleAnalysisAsync(businessUserId, Filter.BrandId, Filter.ProductId, Filter.CatagoryId, this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    Data = result;
                    RefreshData(this.Data);
                    ChartData = CreateDonutChart(this.Data);
                }

                //#if DEBUG
                //                //模拟
                //                var random = new Random();
                //                Data = new SaleAnalysis()
                //                {
                //                    Today = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                //                    LastWeekSame = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                //                    Yesterday = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                //                    BeforeYesterday = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                //                    LastWeek = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                //                    ThisWeek = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                //                    LastMonth = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                //                    ThisMonth = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                //                    ThisQuarter = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) },
                //                    ThisYear = new Sale { NetAmount = random.Next(100, 1000), SaleAmount = random.Next(100, 1000), SaleReturnAmount = random.Next(100, 1000) }
                //                };
                //                ChartData = CreateDonutChart(this.Data);
                //                RefreshData(this.Data);
                //#endif

            }));

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public Chart CreateDonutChart(SaleAnalysis data)
        {
            var entries = new List<ChartEntry>
            {
                new ChartEntry((float)(data?.Today?.NetAmount??0))
                {
                    Label = "今日净额",
                    ValueLabel = (data?.Today?.NetAmount??0).ToString("#,##0.00"),
                    Color = ChartDataProvider.Colors[0]
                },

                new ChartEntry((float)(data?.Yesterday?.NetAmount??0))
                {
                    Label = "昨天净额",
                    ValueLabel = (data?.Yesterday?.NetAmount??0).ToString("#,##0.00"),
                    Color = ChartDataProvider.Colors[1]
                },

                new ChartEntry((float)(data?.BeforeYesterday?.NetAmount??0))
                {
                    Label = "前天净额",
                    ValueLabel = (data?.BeforeYesterday?.NetAmount??0).ToString("#,##0.00"),
                    Color = ChartDataProvider.Colors[2]
                },

                new ChartEntry((float)(data?.LastWeek?.NetAmount??0))
                {
                    Label = "上周净额",
                    ValueLabel = (data?.LastWeek?.NetAmount??0).ToString("#,##0.00"),
                    Color = ChartDataProvider.Colors[3]
                },

                new ChartEntry((float)(data?.ThisWeek?.NetAmount??0))
                {
                    Label = "本周净额",
                    ValueLabel = (data?.ThisWeek?.NetAmount??0).ToString("#,##0.00"),
                    Color = ChartDataProvider.Colors[4]
                },

                new ChartEntry((float)(data?.LastMonth?.NetAmount??0))
                {
                    Label = "上月净额",
                    ValueLabel = (data?.LastMonth?.NetAmount??0).ToString("#,##0.00"),
                    Color = ChartDataProvider.Colors[5]
                },

                new ChartEntry((float)(data?.ThisMonth?.NetAmount??0))
                {
                    Label = "本月净额",
                    ValueLabel = (data?.ThisMonth?.NetAmount??0).ToString("#,##0.00"),
                    Color = ChartDataProvider.Colors[6]
                },

                new ChartEntry((float)(data?.ThisQuarter?.NetAmount??0))
                {
                    Label = "本季净额",
                    ValueLabel = (data?.ThisQuarter?.NetAmount??0).ToString("#,##0.00"),
                    Color = ChartDataProvider.Colors[7]
                },

                new ChartEntry((float)(data?.ThisYear?.NetAmount??0))
                {
                    Label = "本年净额",
                    ValueLabel = (data?.ThisYear?.NetAmount??0).ToString("#,##0.00"),
                    Color = ChartDataProvider.Colors[8]
                }
            };
            return ChartDataProvider.CreateDonutChart(entries);
        }

        private void RefreshData(SaleAnalysis data)
        {
            if ((data?.Today?.NetAmount ?? 0) > (data?.LastWeekSame?.NetAmount ?? 0))
            {
                DescriptionChart = $"今日销售净额 {(data?.LastWeekSame?.NetAmount ?? 0)},比上周同期增加了 {((data?.Today?.NetAmount ?? 0) - (data?.LastWeekSame?.NetAmount ?? 0))}";
            }
            else if ((data?.Today?.NetAmount ?? 0) < (data?.LastWeekSame?.NetAmount ?? 0))
            {
                DescriptionChart = $"今日销售净额 {(data?.LastWeekSame?.NetAmount ?? 0)},比上周同期减少了 {Math.Abs((data?.Today?.NetAmount ?? 0) - (data?.LastWeekSame?.NetAmount ?? 0))}";
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            if (ChartData == null)
                ((ICommand)Load)?.Execute(null);
        }
    }
}
