using Android.App;
using Android.Content;
using Android.OS;
using DCMS.Client.BaiduMaps;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace DCMS.Client.Droid
{
    [Service]
    public class LocationService : Service
    {
        CancellationTokenSource _cts;
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _cts = new CancellationTokenSource();

            Task.Run(async () => 
            {
                Location locShared = null;
                try
                {
                    locShared = new Location();
                    await locShared.Run(_cts.Token);
                }
                catch (Android.OS.OperationCanceledException)
                {
                }
                finally
                {
                    if (_cts.IsCancellationRequested)
                    {
                        var message = new StopServiceMessage();
                        Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(message, "ServiceStopped")
                        );
                    }
                }
            }, _cts.Token);

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Token.ThrowIfCancellationRequested();
                _cts.Cancel();
            }
            base.OnDestroy();
        }
    }

    public class Location
    {
        readonly bool stopping = false;
        public Location()
        {
            try
            {
                //
            }
            catch (Java.Lang.Exception ex)
            {
                Android.Util.Log.Error("上报", ex.Message);
            }
        }

        public async Task Run(CancellationToken token)
        {
            await Task.Run(async () => 
            {
                while (!stopping)
                {
                    token.ThrowIfCancellationRequested();
                    try
                    {
                        await Task.Delay(2000);
                        System.Diagnostics.Debug.Print($"间隔2秒GPS定位一次.....");

                        var request = new GeolocationRequest(GeolocationAccuracy.Best);
                        var location = await Geolocation.GetLocationAsync(request);
                        if (location != null)
                        {
                            GlobalSettings.Latitude = location.Latitude;
                            GlobalSettings.Longitude = location.Longitude;
                            System.Diagnostics.Debug.Print($"{location.Latitude} {location.Longitude}");
                        }
                    }
                    catch (Exception)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var errormessage = new LocationErrorMessage();
                            MessagingCenter.Send(errormessage, "LocationError");
                        });
                    }
                }
            }, token);
        }
    }
}

