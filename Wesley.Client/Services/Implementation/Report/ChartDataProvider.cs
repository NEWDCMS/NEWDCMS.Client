using Wesley.ChartJS.Models;
using Wesley.Client.Models.Census;
using Wesley.Client.Models.Report;
using Wesley.Infrastructure.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Wesley.Client.Services
{
    /// <summary>
    /// 用于图表数据提供
    /// </summary>
    public static class ChartDataProvider
    {
        /// <summary>
        /// 客户排行榜
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public static ChartData GetCustomerRanking(List<CustomerRanking> analysis)
        {
            var labels = analysis.Select(s => s.TerminalName).ToArray();
            var dataSets = new List<ChartDecimalDataset>();
            var colors = RandomChartBuilder.GetDefaultColors();
            var datas = analysis.Select(s => s.NetAmount ?? 0).ToArray();

            dataSets.Add(new ChartDecimalDataset
            {
                type = Wesley.ChartJS.ChartTypes.Bar,
                label = "客户排行榜",
                data = datas,
                backgroundColor = datas.Select((d, i) =>
                {
                    var color = colors[i % colors.Count];
                    return $"rgb({color.Item1},{color.Item2},{color.Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }

        /// <summary>
        /// 品牌销量汇总
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public static ChartData GetBrandRanking(List<BrandRanking> analysis)
        {
            var labels = analysis.Select(s => s.BrandName).ToList();
            var dataSets = new List<ChartDecimalDataset>();
            var colors = RandomChartBuilder.GetDefaultColors();
            var datas = analysis.Select(s => s.NetAmount ?? 0).ToArray();

            dataSets.Add(new ChartDecimalDataset
            {
                type = Wesley.ChartJS.ChartTypes.Bar,
                label = "品牌销量汇总",
                data = datas,
                backgroundColor = datas.Select((d, i) =>
                {
                    var color = colors[i % colors.Count];
                    return $"rgb({color.Item1},{color.Item2},{color.Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }

        /// <summary>
        /// 客户活跃度
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public static ChartData GetCustomerActivity(List<CustomerActivityRanking> analysis)
        {
            var labels = analysis.Select(s => s.TerminalName).ToList();
            var dataSets = new List<ChartNumberDataset>();
            var colors = RandomChartBuilder.GetDefaultColors();
            var datas = analysis.Select(s => s.VisitDaySum ?? 0).ToArray();

            dataSets.Add(new ChartNumberDataset
            {
                type = Wesley.ChartJS.ChartTypes.Line,
                label = "客户活跃度",
                data = datas,
                tension = 0.4,
                backgroundColor = datas.Select((d, i) =>
                {
                    var color = colors[i % colors.Count];
                    return $"rgb({color.Item1},{color.Item2},{color.Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }

        /// <summary>
        /// 客户拜访排行
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public static ChartData GetCustomerVisitRank(List<BusinessVisitRank> analysis)
        {
            var labels = analysis.Select(s => s.BusinessUserName).ToList();
            var dataSets = new List<ChartNumberDataset>();
            var colors = RandomChartBuilder.GetDefaultColors();
            var datas = analysis.Select(s => s.VisitedCount ?? 0).ToArray();

            dataSets.Add(new ChartNumberDataset
            {
                type = Wesley.ChartJS.ChartTypes.Bar,
                label = "客户拜访排行",
                data = datas,
                backgroundColor = datas.Select((d, i) =>
                {
                    var color = colors[i % colors.Count];
                    return $"rgb({color.Item1},{color.Item2},{color.Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }


        /// <summary>
        /// 热订排行榜
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public static ChartData GetHotOrderRanking(List<HotSaleRanking> analysis)
        {
            var labels = analysis.Select(s => s.ProductName).ToList();
            var dataSets = new List<ChartDecimalDataset>();
            var colors = RandomChartBuilder.GetDefaultColors();
            var datas = analysis.Select(s => s.TotalSumNetQuantity ?? 0).ToArray();

            dataSets.Add(new ChartDecimalDataset
            {
                type = Wesley.ChartJS.ChartTypes.Bar,
                label = "热订排行榜",
                data = datas,
                backgroundColor = datas.Select((d, i) =>
                {
                    var color = colors[i % colors.Count];
                    return $"rgb({color.Item1},{color.Item2},{color.Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }

        /// <summary>
        /// 热销排行榜
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public static ChartData GetHotSalesRanking(List<HotSaleRanking> analysis, bool showLable = true)
        {
            var labels = analysis.Select(s => s.ProductName).ToList();
            var dataSets = new List<ChartDecimalDataset>();
            var colors = RandomChartBuilder.GetDefaultColors();
            var datas = analysis.Select(s => s.TotalSumNetQuantity ?? 0).ToArray();

            dataSets.Add(new ChartDecimalDataset
            {
                type = Wesley.ChartJS.ChartTypes.Bar,
                label = "热销排行榜",
                data = datas,
                backgroundColor = datas.Select((d, i) =>
                {
                    var color = colors[i % colors.Count];
                    return $"rgb({color.Item1},{color.Item2},{color.Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }


        /// <summary>
        /// 销售利润排行
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public static ChartData GetSalesProfitRanking(List<CostProfitRanking> analysis)
        {
            var labels = analysis.Select(s => s.ProductName).ToList();
            var dataSets = new List<ChartNumberDataset>();
            var colors = RandomChartBuilder.GetDefaultColors();
            var datas = analysis.Select(s => s.TotalSumNetQuantity ?? 0).ToArray();

            dataSets.Add(new ChartNumberDataset
            {
                type = Wesley.ChartJS.ChartTypes.Bar,
                label = "销售利润排行",
                data = datas,
                backgroundColor = datas.Select((d, i) =>
                {
                    var color = colors[i % colors.Count];
                    return $"rgb({color.Item1},{color.Item2},{color.Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }


        /// <summary>
        /// 业务员销售排行
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public static ChartData GetSalesRanking(List<BusinessRanking> analysis)
        {
            var labels = analysis.Select(s => s.BusinessUserName).ToArray();
            var dataSets = new List<ChartDecimalDataset>();
            var colors = RandomChartBuilder.GetDefaultColors();
            var datas = analysis.Select(s => s.NetAmount ?? 0).ToArray();

            dataSets.Add(new ChartDecimalDataset
            {
                type = Wesley.ChartJS.ChartTypes.Bar,
                label = "销售排行",
                data = datas,
                backgroundColor = datas.Select((d, i) =>
                {
                    var color = colors[i % colors.Count];
                    return $"rgb({color.Item1},{color.Item2},{color.Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }


        /// <summary>
        /// 销量走势图
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public static ChartData GetSaleTrendChat(List<SaleTrending> analysis)
        {
            var labels = analysis.Select(s => s.SaleDateName).ToList();
            var dataSets = new List<ChartDecimalDataset>();
            var colors = RandomChartBuilder.GetDefaultColors();
            var datas = analysis.Select(s => s.NetAmount ?? 0).ToArray();

            dataSets.Add(new ChartDecimalDataset
            {
                type = Wesley.ChartJS.ChartTypes.Line,
                label = "销量走势图",
                data = datas,
                tension = 0.4,
                backgroundColor = datas.Select((d, i) =>
                {
                    var color = colors[i % colors.Count];
                    return $"rgb({color.Item1},{color.Item2},{color.Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }


        /// <summary>
        /// 库存滞销排行榜
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public static ChartData GetUnsalable(List<UnSaleRanking> analysis)
        {
            var labels = analysis.Select(s => s.ProductName).ToList();
            var dataSets = new List<ChartDecimalDataset>();
            var colors = RandomChartBuilder.GetDefaultColors();
            var datas = analysis.Select(s => s.TotalSumNetQuantity ?? 0).ToArray();

            dataSets.Add(new ChartDecimalDataset
            {
                type = Wesley.ChartJS.ChartTypes.Line,
                label = "库存滞销排行榜",
                data = datas,
                tension = 0.4,
                backgroundColor = datas.Select((d, i) =>
                {
                    var color = colors[i % colors.Count];
                    return $"rgb({color.Item1},{color.Item2},{color.Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }


        public static ChartData GetNewCustomers(NewCustomerAnalysis analysis)
        {
            var labels = analysis.ChartDatas.Keys.Select(s => s).ToList();
            var dataSets = new List<ChartNumberDataset>();

            var colors = RandomChartBuilder.GetDefaultColors();

            var datas = analysis.ChartDatas.Values.Select(s => System.Convert.ToInt32(s)).ToList();

            dataSets.Add(new ChartNumberDataset
            {
                type = Wesley.ChartJS.ChartTypes.Line,
                label = "新增客户",
                data = datas,
                tension = 0.4,
                backgroundColor = datas.Select((d, i) =>
                {
                    var color = colors[i % colors.Count];
                    return $"rgb({color.Item1},{color.Item2},{color.Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }
        public static ChartData GetVisitingRate(CustomerVistAnalysis analysis)
        {
            float up = 100;
            if (analysis.TotalCustomer - analysis.Today.VistCount != 0)
            {
                if (analysis.TotalCustomer == 0) analysis.TotalCustomer = 1;
                up = (float)((analysis.TotalCustomer - analysis.Today.VistCount) / analysis.TotalCustomer) * 100;
            }

            var labels = new string[] { "总客户数", "拜访数", "拜访数", };
            var dataSets = new List<ChartFloatDataset>();

            var colors = RandomChartBuilder.GetDefaultColors();

            var datas = new float[] { analysis.TotalCustomer, (float)(analysis?.Today?.Percentage), (up == 0 ? 100 : up) };

            dataSets.Add(new ChartFloatDataset
            {
                type = Wesley.ChartJS.ChartTypes.Pie,
                label = "客户拜访分析",
                data = datas,
                tension = 0.4,
                backgroundColor = datas.Select((d, i) =>
                {
                    var color = colors[i % colors.Count];
                    return $"rgb({color.Item1},{color.Item2},{color.Item3})";
                })
            });

            return new ChartData()
            {
                datasets = dataSets,
                labels = labels
            };
        }

    }
}
