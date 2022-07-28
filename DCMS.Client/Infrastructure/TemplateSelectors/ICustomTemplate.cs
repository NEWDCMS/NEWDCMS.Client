using Wesley.Client.Enums;
using Newtonsoft.Json;

namespace Wesley.Client.Services
{
    /// <summary>
    /// 消息模板
    /// </summary>
    public class ICustomTemplate : Base
    {
        [JsonIgnore]
        public int Type { get; set; }
        [JsonIgnore]
        public CustomTemplate TemplateType { get; set; }
    }

    /// <summary>
    /// Chat模板
    /// </summary>
    public class IChartTemplate : Base
    {
        [JsonIgnore]
        public int Type { get; set; }
        [JsonIgnore]
        public ChartTemplate TemplateType { get; set; }
    }

    /// <summary>
    /// 收款对账单据模板
    /// </summary>
    public class IBillTemplate : Base
    {
        public BillTypeEnum BType { get; set; }
    }

    /// <summary>
    /// 表标头模板
    /// </summary>
    public interface IReportTemplate
    {
        public ChartPageEnum PageType { get; set; }
    }
}
