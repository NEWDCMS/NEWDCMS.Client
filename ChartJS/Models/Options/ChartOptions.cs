using Plugin.Models;

namespace Wesley.ChartJS.Models
{
    public class ChartOptions
    {
        public string indexAxis { get; set; }
        public bool? responsive { get; set; }
        public bool? maintainAspectRatio { get; set; } = false;
        public double? aspectRatio { get; set; }
        public double? resizeDelay { get; set; }
        public double? devicePixelRatio { get; set; }
        public string locale { get; set; }
        public ChartInteraction interaction { get; set; }
        public ChartAnimation animation { get; set; }
        public ChartLayout layout { get; set; }
        public ChartLegend legend { get; set; }
        public Scales scales { get; set; }
        public double borderWidth { get; set; } = 1;
        public ChartPlugins plugins { get; set; }
    }
}
