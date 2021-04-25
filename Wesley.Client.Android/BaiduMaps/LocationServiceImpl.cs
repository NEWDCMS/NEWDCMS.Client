using Com.Baidu.Location;
using DCMS.Client.BaiduMaps;
using DCMS.Client.Models;
using DCMS.Client.Models.Census;
using System;
using System.Threading;

namespace DCMS.Client.Droid
{
    public class BaiduLocationServiceImpl : IBaiduLocationService
    {
        public static LocationClient mLocationClient = null;
        public MyLocationListener myLocationListener = new MyLocationListener();
        private bool stopped;
        //public static event EventHandler<LocationFailedEventArgs> Failed;
        //public static event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

        public BaiduLocationServiceImpl()
        {
            try
            {
                mLocationClient = new LocationClient(MainApplication.Context)
                {
                    LocOption = SetLocationClientOption()
                };
                mLocationClient.RegisterLocationListener(myLocationListener);
            }
            catch (Java.Lang.Exception ex)
            {
                Android.Util.Log.Error("上报", ex.Message);
            }
        }

        /// <summary>
        /// 定位客户端参数设定
        /// </summary>
        /// <returns></returns>
        private LocationClientOption SetLocationClientOption()
        {
            int span = 5 * 1000;
            var option = new LocationClientOption
            {
                CoorType = "bd09ll",
                ScanSpan = span
            };
            ////可选，默认高精度，设置定位模式，高精度，低功耗，仅设备  
            option.SetLocationMode(LocationClientOption.LocationMode.HightAccuracy);
            option.SetIsNeedAddress(true);//可选，设置是否需要地址信息，默认不需要  
            option.OpenGps = true;//可选，默认false,设置是否使用gps  
            option.LocationNotify = true;//可选，默认false，设置是否当GPS有效时按照1S/1次频率输出GPS结果  
            option.SetIsNeedLocationDescribe(true);
            option.SetIsNeedLocationPoiList(true);
            option.SetIgnoreKillProcess(true);
            option.SetIgnoreCacheException(true);//可选，默认false，设置是否收集CRASH信息，默认收集  
            option.EnableSimulateGps = true;//可选，默认false，设置是否需要过滤GPS仿真结果，默认需要 
            return option;
        }

        public void Start()
        {
            try
            {
                if (!mLocationClient.IsStarted)
                {
                    mLocationClient?.Start();
                    stopped = false;
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Android.Util.Log.Error("上报", ex.Message);
            }
        }

        public void Stop()
        {
            try
            {
                if (mLocationClient != null)
                {
                    mLocationClient.Stop();
                    if (myLocationListener != null)
                        mLocationClient.UnRegisterLocationListener(myLocationListener);
                    stopped = true;
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Android.Util.Log.Error("上报", ex.Message);
            }
        }

        /// <summary>
        /// 此处的BDLocation为定位结果信息类，通过它的各种get方法可获取定位相关的全部结果
        /// 以下只列举部分获取经纬度相关（常用）的结果信息
        /// 更多结果信息获取说明，请参照类参考中BDLocation类中的说明
        /// </summary>
        public class MyLocationListener : BDAbstractLocationListener
        {
            public override void OnReceiveLocation(BDLocation lct)
            {
                if (lct == null)
                    return;

                try
                {
                    var msg = $"OnReceiveLocation:{lct.Latitude} Latitude:{lct.Longitude} Address:{lct.Country}{lct.Province}{lct.City}{lct.Address?.Street}";
                    Android.Util.Log.Info("位置：", msg);

                    //ToastUtils.ShowSingleToast(msg);

                    GlobalSettings.Latitude = lct.Latitude;
                    GlobalSettings.Longitude = lct.Longitude;

                    new Thread(async () =>
                    {
                        if (lct != null && !GlobalSettings.IsNotConnected)
                        {
                            var _conn = App.Resolve<LocalDatabase>();
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
                                //Town = lct.Town,
                                Address = lct.Address?.Street
                            };

                            var gpsEvent = new GpsEvent()
                            {
                                Latitude = lct.Latitude,
                                Longitude = lct.Longitude,
                                CreateTime = DateTime.Now,
                                Address = $"{lct.Country}{lct.Province}{lct.City}{lct.Address?.Street}"
                            };

                            var cg = await _conn.GpsEvents.CountAsync();
                            var lg = await _conn.LocationSyncEvents.CountAsync();

                            if (cg > 50)
                            {
                                await _conn.ResetGpsEvents();
                            }

                            if (lg > 50)
                            {
                                await _conn.ResetLocationSyncEvents();
                            }

                            //存储本地  
                            if (!string.IsNullOrWhiteSpace(gpsEvent.Address))
                            {
                                await _conn.InsertAsync(gpsEvent);
                            }

                            if (!string.IsNullOrWhiteSpace(tracking.Address))
                            {
                                await _conn.InsertAsync(tracking);
                            }
                        }
                    }).Start();

                }
                catch (Java.Lang.Exception ex)
                {
                    Android.Util.Log.Error("上报", ex.Message);
                }
            }
        }
    }
}

