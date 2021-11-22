namespace Wesley.ChartJS.Models
{
    public interface IChartDataset
    {
        string type { get; set; }
        string label { get; set; }
        int? order { get; set; }
    }
}
