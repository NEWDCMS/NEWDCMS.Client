using DCMS.ChartJS.Models;
using DCMS.Client.Enums;
using DCMS.Client.Models;
using DCMS.Client.Models.Census;
using DCMS.Client.Models.Sales;
using DCMS.Client.Pages;
using DCMS.Client.Services;
using DCMS.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DCMS.Client.ViewModels
{
    public class VisitReportPageViewModel : ViewModelBaseChart<VisitStore>
    {

        public VisitReportPageViewModel(INavigationService navigationService,
            IProductService productService,
            IReportingService reportingService,
            IDialogService dialogService
            ) : base(navigationService,
                productService,
                reportingService,
                dialogService)
        {
            Title = "业务拜访统计";

            this.Load = ReactiveCommand.CreateFromTask<int>((t) => Task.Run(() =>
            {
                try
                {
                    //今日 1  昨天 3 前天 4 本月 8
                    var tag = t;
                    if (tag == 0) tag = 8;
                    _reportingService.Rx_GetBusinessAnalysis(tag, new System.Threading.CancellationToken())
                    .Subscribe((results) =>
                    {
                        if (results != null && results?.Code >= 0)
                        {
                            var analysis = results?.Data;
                            var data = new ChartViewConfig()
                            {
                                BackgroundColor = Color.White,
                                ChartConfig = new ChartConfig
                                {
                                    type = DCMS.ChartJS.ChartTypes.Bar,
                                    data = GetChartData(analysis),
                                    options = new ChartOptions
                                    {
                                        indexAxis = "y",
                                        scales = new Scales
                                        {
                                            xAxes = new XAxes[]
                                               {
                                             new XAxes
                                             {
                                                 ticks = new Ticks
                                                 {
                                                     beginAtZero = true
                                                 },
                                                 position = "top"
                                             }
                                               }
                                        }
                                    }
                                }
                            };
                            ChartConfig = data;
                        }
                    });

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }));

            //绑定页面菜单
            //BindFilterDateMenus(true);
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                { MenuEnum.TODAY, (m,vm) => {
                ((ICommand)Load)?.Execute(1);
                } },
                { MenuEnum.YESTDAY, (m,vm) => {
                ((ICommand)Load)?.Execute(3);
                } },
                { MenuEnum.THISWEEBK, (m,vm) => {
                ((ICommand)Load)?.Execute(6);
                } },
                { MenuEnum.LASTMONTH, (m,vm) => {
                ((ICommand)Load)?.Execute(7);
                } },
                { MenuEnum.MONTH, (m,vm) => {
                ((ICommand)Load)?.Execute(8);
                } },
                { MenuEnum.YEAR, (m,vm) => {
                ((ICommand)Load)?.Execute(9);
                } }
            });
            this.BindBusyCommand(Load);
        }


        private ChartData GetChartData(BusinessAnalysis analysis)
        {
            var labels = analysis.UserNames;
            var dataSets = new List<ChartNumberDataset>();

            var colors = RandomChartBuilder.GetDefaultColors();

            dataSets.Add(new ChartNumberDataset
            {
                type = DCMS.ChartJS.ChartTypes.Bar,
                label = "拜访量",
                data = analysis.VistCounts,
                tension = 0.4,
                indexAxis = "y",
                backgroundColor = analysis.VistCounts.Select((d, i) =>
                {
                    //var color = colors[i % colors.Count];
                    return $"rgb({colors[0].Item1},{colors[0].Item2},{colors[0].Item3})";
                }),

            });
            dataSets.Add(new ChartNumberDataset
            {
                type = DCMS.ChartJS.ChartTypes.Bar,
                label = "销单数",
                data = analysis.SaleCounts,
                tension = 0.4,
                indexAxis = "y",
                backgroundColor = analysis.VistCounts.Select((d, i) =>
                {
                    //var color = colors[i % colors.Count];
                    return $"rgb({colors[3].Item1},{colors[3].Item2},{colors[3].Item3})";
                })
            });

            //dataSets.Add(new ChartNumberDataset
            //{
            //    type = DCMS.ChartJS.ChartTypes.Bar,
            //    label = "销订数",
            //    data = analysis.OrderCounts,
            //    tension = 0.4,
            //    backgroundColor = analysis.VistCounts.Select((d, i) =>
            //    {
            //        return $"rgb({colors[5].Item1},{colors[5].Item2},{colors[5].Item3})";
            //    })
            //});

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            //过滤器
            if (parameters.ContainsKey("Tag"))
            {
                parameters.TryGetValue("Tag", out int tag);
                ((ICommand)Load)?.Execute(tag);
            }
            else
            {
                ((ICommand)Load)?.Execute(8);
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            _popupMenu?.Show(8, 9, 12, 13, 10, 11);
        }
    }
}
