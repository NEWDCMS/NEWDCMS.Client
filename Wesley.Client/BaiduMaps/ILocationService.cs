using Wesley.Client.Models.Census;
using System;
using System.Threading.Tasks;

namespace Wesley.Client.BaiduMaps
{
    public class LocationUpdatedEventArgs : EventArgs
    {
        public Coordinate Coordinate { get; set; }
        public double Direction { get; set; }
        public double Altitude { get; set; }
        public double Accuracy { get; set; }
        public int Satellites { get; set; }
        public string Type { get; set; }
        public TrackingModel GpsEvent { get; set; }
    }

    //LocationSyncEvents
    public class LocationFailedEventArgs : EventArgs
    {
        public string Message { get; }
        public LocationFailedEventArgs(string message)
        {
            Message = message;
        }
    }

    public interface IBaiduLocationService
    {
        Task UpdateCenter(Wesley.Client.BaiduMaps.Map map, Action action);
        Task UpdateCenter(Wesley.Client.BaiduMaps.Map map);
        bool IsStarted();
        void OnDestroy();
        void Start();
        void Stop();
        void Converter(double lat, double lng);
        void Converter(Wesley.Client.BaiduMaps.Map map, double lat, double lng);
        //static event EventHandler<LocationFailedEventArgs> Failed;
        //static event EventHandler<LocationUpdatedEventArgs> LocationUpdated;
    }

}

