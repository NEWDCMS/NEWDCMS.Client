using Xamarin.Forms;
namespace Wesley.Client.BaiduMaps
{
    public interface IProjection
    {
        Point ToScreen(Coordinate p);
        Coordinate ToCoordinate(Point p);
    }
}

