using System.Collections.Generic;

namespace Wesley.ChartJS.Models
{
    public class ChartNumberDataset : IChartDataset
    {
        public string type { get; set; }
        public string label { get; set; }
        public int? order { get; set; }
        public double? tension { get; set; } = 0.4;
        public IEnumerable<int> data { get; set; }
        public string indexAxis { get; set; }
        public IEnumerable<string> backgroundColor { get; set; }
    }

    public class ChartDecimalDataset : IChartDataset
    {
        public string type { get; set; }
        public string label { get; set; }
        public int? order { get; set; }
        public double? tension { get; set; } = 0.4;
        public IEnumerable<decimal> data { get; set; }
        public IEnumerable<string> backgroundColor { get; set; }
    }

    public class ChartFloatDataset : IChartDataset
    {
        public string type { get; set; }
        public string label { get; set; }
        public int? order { get; set; }
        public double? tension { get; set; } = 0.4;
        public IEnumerable<float> data { get; set; }
        public IEnumerable<string> backgroundColor { get; set; }
    }
}
