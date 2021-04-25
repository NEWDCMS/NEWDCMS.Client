using Wesley.Client.Enums;
using Wesley.Client.Services;
using Xamarin.Forms;
namespace Wesley.Client.Selector
{
    /// <summary>
    /// 自定义消息模板选择器
    /// </summary>
    public class CustomDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var template = new DataTemplate();
            var type = (ICustomTemplate)item;
            if (type != null)
            {
                switch ((int)type.TemplateType)
                {
                    case 0:
                        template = MessageTemplate;
                        break;
                    case 1:
                        template = NewsTemplate;
                        break;
                    default:
                        template = ValidTemplate;
                        break;
                }
            }
            return template;
        }
        public DataTemplate ValidTemplate { get; set; }

        public DataTemplate InvalidTemplate { get; set; }

        public DataTemplate MessageTemplate { get; set; }

        public DataTemplate NewsTemplate { get; set; }

        public DataTemplate MovieTemplate { get; set; }

        ///
    }

    /// <summary>
    /// 自定义图表模板选择器
    /// </summary>
    public class CustomChartDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var template = new DataTemplate();
            var type = (IChartTemplate)item;
            if (type != null)
            {
                switch ((int)type.TemplateType)
                {
                    case 0:
                        template = BarChartTemplate;
                        break;
                    case 1:
                        template = LineChartTemplate;
                        break;
                    case 2:
                        template = PieChartTemplate;
                        break;
                    case 3:
                        template = RadarChartTemplate;
                        break;
                    case 4:
                        template = ScatterChartTemplate;
                        break;
                }
            }
            return template;
        }

        /// <summary>
        /// BarChart
        /// </summary>
        public DataTemplate BarChartTemplate { get; set; }
        /// <summary>
        /// LineChart
        /// </summary>
        public DataTemplate LineChartTemplate { get; set; }
        /// <summary>
        /// PieChart
        /// </summary>
        public DataTemplate PieChartTemplate { get; set; }
        /// <summary>
        /// RadarChart
        /// </summary>
        public DataTemplate RadarChartTemplate { get; set; }
        /// <summary>
        /// ScatterChart
        /// </summary>
        public DataTemplate ScatterChartTemplate { get; set; }
    }

    /// <summary>
    /// 自定义单据模板选择器
    /// </summary>
    public class CustomBillTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var template = new DataTemplate();
            var type = (IBillTemplate)item;
            if (type != null)
            {
                switch (type.BType)
                {
                    case BillTypeEnum.SaleBill:
                        template = SaleBillTemplate;
                        break;
                    case BillTypeEnum.ReturnBill:
                        template = ReturnBillTemplate;
                        break;
                    case BillTypeEnum.CashReceiptBill:
                        template = CashReceiptBillTemplate;
                        break;
                    case BillTypeEnum.AdvanceReceiptBill:
                        template = AdvanceReceiptBillTemplate;
                        break;
                    case BillTypeEnum.CostExpenditureBill:
                        template = CostExpenditureBillTemplate;
                        break;
                    default:
                        template = ValidTemplate;
                        break;
                }
            }
            return template;
        }

        public DataTemplate SaleBillTemplate { get; set; }
        public DataTemplate ReturnBillTemplate { get; set; }
        public DataTemplate CashReceiptBillTemplate { get; set; }
        public DataTemplate AdvanceReceiptBillTemplate { get; set; }
        public DataTemplate CostExpenditureBillTemplate { get; set; }
        public DataTemplate ValidTemplate { get; set; }

    }


    /// <summary>
    /// 自定义报表头模板选择器
    /// </summary>
    public class CustomReportHeaderTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var template = new DataTemplate();
            var type = (IReportTemplate)item;
            if (type != null)
            {
                switch (type.PageType)
                {
                    case ChartPageEnum.BrandRanking_Template:
                        template = BrandRanking_Template;
                        break;
                    case ChartPageEnum.CustomerActivity_Template:
                        template = CustomerActivity_Template;
                        break;
                    case ChartPageEnum.CustomerRanking_Template:
                        template = CustomerRanking_Template;
                        break;
                    case ChartPageEnum.CustomerVisitRank_Template:
                        template = CustomerVisitRank_Template;
                        break;
                    case ChartPageEnum.HotOrderRanking_Template:
                        template = HotOrderRanking_Template;
                        break;
                    case ChartPageEnum.HotSalesRanking_Template:
                        template = HotSalesRanking_Template;
                        break;
                    case ChartPageEnum.SalesProfitRanking_Template:
                        template = SalesProfitRanking_Template;
                        break;
                    case ChartPageEnum.SalesRanking_Template:
                        template = SalesRanking_Template;
                        break;
                    case ChartPageEnum.SaleTrendChat_Template:
                        template = SaleTrendChat_Template;
                        break;
                    case ChartPageEnum.Unsalable_Template:
                        template = Unsalable_Template;
                        break;
                    default:
                        break;
                }
            }
            return template;
        }

        public DataTemplate BrandRanking_Template { get; set; }
        public DataTemplate CustomerActivity_Template { get; set; }
        public DataTemplate CustomerRanking_Template { get; set; }
        public DataTemplate CustomerVisitRank_Template { get; set; }
        public DataTemplate HotOrderRanking_Template { get; set; }
        public DataTemplate HotSalesRanking_Template { get; set; }
        public DataTemplate SalesProfitRanking_Template { get; set; }
        public DataTemplate SalesRanking_Template { get; set; }
        public DataTemplate SaleTrendChat_Template { get; set; }
        public DataTemplate Unsalable_Template { get; set; }

    }
}
