namespace Wesley.ChartJS.Models
{
    public class ChartConfig
    {
        public string type { get; set; }
        public ChartData data { get; set; }
        public ChartOptions options { get; set; } = new ChartOptions();
    }
}
