using System.Collections.Generic;

namespace Wesley.ChartJS.Models
{
    public class ChartData
    {
        public IEnumerable<IChartDataset> datasets { get; set; }
        public IEnumerable<string> labels { get; set; }
    }
}
