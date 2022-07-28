namespace Wesley.Client.BaiduMaps
{
    public interface ICalculateUtils
    {
        double CalculateDistance(Coordinate p1, Coordinate p2);
        void Converter(double lat, double lng);
    }
}

