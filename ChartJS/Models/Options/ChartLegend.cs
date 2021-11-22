namespace Wesley.ChartJS.Models
{
    public class ChartLegend
    {
        public bool? display { get; set; }
        public string position { get; set; }
        public string align { get; set; }
        public int? maxHeight { get; set; }
        public int? maxWidth { get; set; }
        public bool? fullSize { get; set; }
        public bool? reverse { get; set; }
        public bool? rtl { get; set; }
        public string textDirection { get; set; }
    }

    public class ChartPlugins
    {
        public ChartLegend legend { get; set; }
    }

    public class Scales
    {
        public XAxes[] xAxes { get; set; }
    }

    public class XAxes
    {
        public Ticks ticks { get; set; }
        public string position { get; set; } = "top";

    }
    public class Ticks
    {
        public bool? beginAtZero { get; set; } = true;
    }
}
