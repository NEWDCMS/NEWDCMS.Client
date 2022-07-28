using Android.Content;
using Android.Locations;
using Android.Views;
using Android.Widget;
using Com.Baidu.Location;
using Com.Baidu.Mapapi.Utils;
using DCMS.Client.BaiduMaps;
using DCMS.Client.Models.Census;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace DCMS.Client.Droid.Services
{
    public class BaiduLocationServiceImpl : IBaiduLocationService
    {
        private static LocationClient client = null;
        private static LocationClientOption mOption;
        private static MyLocationListener myLocationListener;
        private readonly object objLock = new Object();

        public BaiduLocationServiceImpl()
        {
            myLocationListener = new MyLocationListener();
            client = new LocationClient(MainActivity.Instance)
            {
                // 设置定位参数
                LocOption = GetDefaultLocationClientOption()
            };
        }

        public LocationClientOption GetDefaultLocationClientOption()
        {
            if (mOption == null)
            {
                mOption = new LocationClientOption();
                //高精度
                mOption.SetLocationMode(LocationClientOption.LocationMode.HightAccuracy);
                //附近地址
                mOption.SetIsNeedAddress(true);
                // 可选，默认0，即仅定位一次，设置发起连续定位请求的间隔需要大于等于1000ms才是有效的
                mOption.ScanSpan = 1000;
                // 可选，默认gcj02，设置返回的定位结果坐标系，如果配合百度地图使用，建议设置为bd09ll;
                mOption.CoorType = "bd09ll";
                // 可选，默认false，设置是否开启Gps定位
                mOption.OpenGps = true;
                // 可选，默认true，定位SDK内部是一个SERVICE，并放到了独立进程，设置是否在stop
                mOption.SetIgnoreKillProcess(true);
                // 可选，默认false，设置是否需要位置语义化结果，可以在BDLocation
                mOption.SetIsNeedLocationDescribe(true);
                // 可选，默认false，设置是否需要POI结果，可以在BDLocation
                mOption.SetIsNeedLocationPoiList(true);
            }
            return mOption;
        }

        public void Converter(BaiduMaps.Map map, double lat, double lng)
        {
            try
            {
                //初始化坐标转换工具类，指定源坐标类型和坐标数据
                CoordinateConverter converter = new CoordinateConverter()
                        .From(CoordinateConverter.CoordType.Gps)
                        .Coord(new Com.Baidu.Mapapi.Model.LatLng(lat, lng));

                //desLatLng 转换后的坐标
                var lct = converter.Convert();

                if (map != null && lct != null)
                {
                    if (lct.Latitude > 0 && lct.Longitude > 0)
                    {
                        map.Center = new Coordinate(lct.Latitude, lct.Longitude);
                        map.SendStatusChanged(lct.Latitude, lct.Longitude);
                    }
                }

                if (lct != null && lct.Latitude > 0 && lct.Longitude > 0)
                    GlobalSettings.UpdatePoi(lct.Latitude, lct.Longitude);
            }
            catch (Exception) { }
        }

        public void Converter(double lat, double lng)
        {
            //初始化坐标转换工具类，指定源坐标类型和坐标数据
            CoordinateConverter converter = new CoordinateConverter()
                    .From(CoordinateConverter.CoordType.Gps)
                    .Coord(new Com.Baidu.Mapapi.Model.LatLng(lat, lng));

            //desLatLng 转换后的坐标
            var lct = converter.Convert();
            if (lct != null && lct.Latitude > 0 && lct.Longitude > 0)
                GlobalSettings.UpdatePoi(lct.Latitude, lct.Longitude);
        }

        public bool IsStarted()
        {
            return client?.IsStarted ?? false;
        }

        public void Start()
        {
            lock (objLock)
            {
                if (client != null && !client.IsStarted)
                {
                    System.Diagnostics.Debug.Print("RegisterListener------------////-------------->");
                    RegisterListener(myLocationListener);
                    //开始服务
                    client.Start();
                }
            }
        }

        public void Stop()
        {
            lock (objLock)
            {
                if (client != null && client.IsStarted)
                {
                    client.Stop();
                }
            }
        }

        ~BaiduLocationServiceImpl() { OnDestroy(); }
        public void OnDestroy()
        {
            try
            {
                if (client != null)
                {
                    //停止Baidu定位服务
                    if (client.IsStarted)
                        client?.Stop();

                    //关闭前台定位服务
                    //client?.DisableLocInForeground(true);

                    //取消之前注册的 BDAbstractLocationListener 定位监听函数
                    UnregisterListener(myLocationListener);
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public bool RegisterListener(BDAbstractLocationListener listener)
        {
            bool isSuccess = false;
            if (listener != null)
            {
                client.RegisterLocationListener(listener);
                isSuccess = true;
            }
            return isSuccess;
        }

        public void UnregisterListener(BDAbstractLocationListener listener)
        {
            if (listener != null)
            {
                client.UnRegisterLocationListener(listener);
            }
        }

        private CancellationTokenSource Cts => new CancellationTokenSource();
        public async Task UpdateCenter(BaiduMaps.Map map, Action action)
        {
            await UpdateCenter(map);
            action?.Invoke();
        }


        public async Task UpdateCenter(BaiduMaps.Map map)
        {
            try
            {
                var locationManager = (LocationManager)MainActivity.Instance.GetSystemService(Context.LocationService);
                //判断GPS是否开启，没有开启，则开启
                if (!locationManager.IsProviderEnabled(LocationManager.GpsProvider))
                {
                    var toast = Toast.MakeText(Android.App.Application.Context, "你的位置服务没有开启，请打开GPS", ToastLength.Short);
                    toast.SetGravity(GravityFlags.Center, 0, 0);
                    toast.Show();
                }
                else
                {
                    var locp = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (locp != PermissionStatus.Granted)
                    {
                        var toast = Toast.MakeText(Android.App.Application.Context, "你的位置服务没有开启，请打开GPS", ToastLength.Short);
                        toast.SetGravity(GravityFlags.Center, 0, 0);
                        toast.Show();
                    }
                    else
                    {
                        var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(2));
                        var location = await Geolocation.GetLocationAsync(request, Cts.Token);
                        if (location != null)
                        {
                            Converter(map, location.Latitude, location.Longitude);
                        }
                    }
                }
            }
            catch (FeatureNotEnabledException)
            {
                Cts.Token.ThrowIfCancellationRequested();
                Cts.Cancel();
            }
            catch (FeatureNotSupportedException)
            {
                Cts.Token.ThrowIfCancellationRequested();
                Cts.Cancel();
            }
            catch (PermissionException)
            {
                Cts.Token.ThrowIfCancellationRequested();
                Cts.Cancel();
            }
            catch (Exception)
            {
                Cts.Token.ThrowIfCancellationRequested();
                Cts.Cancel();
            }
        }
    }

    /// <summary>
    /// 侦听器
    /// </summary>
    public class MyLocationListener : BDAbstractLocationListener
    {
        public override void OnReceiveLocation(BDLocation lct)
        {
            if (lct == null)
                return;

            try
            {
                var asyncTasker = new AsyncTasker(async () =>
               {
                   try
                   {
                       if (lct != null)
                       {
                           //var msg = $"OnReceiveLocation:{lct.Latitude} Latitude:{lct.Longitude} Address:{lct.Country}{lct.Province}{lct.City}{lct.Address?.Street}";
                           //Android.Util.Log.Info("获取位置：", msg);

                           var _conn = App.Resolve<ILiteDbService<TrackingModel>>();
                           if (_conn != null)
                           {
                               var tracking = new TrackingModel()
                               {
                                   StoreId = Settings.StoreId,
                                   BusinessUserId = Settings.UserId,
                                   BusinessUserName = Settings.UserRealName,
                                   Latitude = lct.Latitude,
                                   Longitude = lct.Longitude,
                                   CreateDateTime = DateTime.Now,
                                   Province = lct.Province,
                                   County = lct.Country,
                                   City = lct.City,
                                   Address = $"{lct.Country}{lct.Province}{lct.City}{lct.Address?.Street}"
                               };

                               //存储本地  
                               if (!string.IsNullOrWhiteSpace(tracking.Address))
                               {
                                   var exits = await _conn.Table.CountAsync(s => s.Latitude == lct.Latitude
                                  && s.Longitude == lct.Longitude && s.Address == tracking.Address);
                                   if (exits == 0)
                                   {
                                       await _conn.InsertAsync(tracking);
                                   }
                               }
                           }
                       }
                   }
                   catch (Exception ex)
                   {
                       Crashes.TrackError(ex);
                   }
               });

                asyncTasker?.Execute();

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }


    public class BaiduNavigationService : IBaiduNavigationService
    {
        /// <summary>
        /// 导航到
        /// </summary>
        /// <param name="latitude">目的地latitude</param>
        /// <param name="longitude">目的地longitude</param>
        /// <param name="addressName">目的地名称</param>
        public void OpenNavigationTo(double latitude, double longitude, string addressName)
        {
            try
            {
                var currentActivity = MainActivity.Instance;
                var uri = Android.Net.Uri.Parse("baidumap://map/direction?destination=latlng:" + latitude + "," + longitude + "|name:" + addressName + "&mode=driving");
                var mapIntent = new Intent(Intent.ActionView, uri);
                currentActivity?.StartActivity(mapIntent);
            }
            catch (Exception) { }
        }
    }
}