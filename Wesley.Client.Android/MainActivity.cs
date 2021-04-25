using Acr.UserDialogs;
using Akavache.Sqlite3;
using Android;
using Android.Annotation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.Content;
using Com.Baidu.Location;
using Com.Baidu.Mapapi.Map;
using Com.Baidu.Mapapi.Utils;
using Wesley.Client.BaiduMaps;
using Wesley.Client.Droid.AutoUpdater;
using Wesley.Client.Droid.Utils;
using Wesley.Client.Models.Census;
using Wesley.Client.Pages;
using FFImageLoading;
using FFImageLoading.Forms.Platform;
using ImageCircle.Forms.Plugin.Droid;
using Microsoft.AppCenter.Crashes;
using Rg.Plugins.Popup;
using Shiny;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Xamarin.Forms;
using Platform = Xamarin.Essentials.Platform;
using ZXingPlatform = global::ZXing.Net.Mobile.Forms.Android;
using Android.Locations;


namespace Wesley.Client.Droid
{
    [Activity(Label = "@string/ApplicationName",
        Icon = "@mipmap/ic_launcher",
        Theme = "@style/MainTheme",
         MainLauncher = true,
        //LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected Context mContext;
        public static MainActivity Instance { set; get; }
        private bool isBacking = false;
        private PowerManager.WakeLock _wakeLock;

        /*
        PARTIAL_WAKE_LOCK:保持CPU 运转，屏幕和键盘灯有可能是关闭的。
        SCREEN_DIM_WAKE_LOCK：保持CPU 运转，允许保持屏幕显示但有可能是灰的，允许关闭键盘灯
        SCREEN_BRIGHT_WAKE_LOCK：保持CPU 运转，允许保持屏幕高亮显示，允许关闭键盘灯
        FULL_WAKE_LOCK：保持CPU 运转，保持屏幕高亮显示，键盘灯也保持亮度
        ACQUIRE_CAUSES_WAKEUP：强制使屏幕亮起，这种锁主要针对一些必须通知用户的操作.
        ON_AFTER_RELEASE：当锁被释放时，保持屏幕亮起一段时间**
        */

        #region override

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            //base.Window.RequestFeature(WindowFeatures.NoTitle);
            //重置主题:(注意 RequestFeature 一定先于OnCreate执行)
            base.Window.RequestFeature(WindowFeatures.ActionBar);
            base.OnCreate(bundle);

            //添加Activity
            ActivityCollector.AddActivity(this);

            mContext = this;
            Instance = this;

            #region 拷贝数据架构到本地存储

            //try
            //{
            //    DbContext.LocalFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            //    var fileName = System.IO.Path.Combine(DbContext.LocalFilePath, DbContext.DB_NAME);
            //    if (!File.Exists(fileName))
            //    {
            //        using (var source = Assets.Open(DbContext.DB_NAME))
            //        using (var dest = OpenFileOutput(DbContext.DB_NAME, FileCreationMode.Append))
            //        {
            //            source.CopyTo(dest);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Crashes.TrackError(ex);
            //}

            #endregion

            //获取电源锁
            AcquireWakeLock();

            //图片裁切
            BitImageEditor.Droid.Platform.Init(this, bundle);

            //初始 Popup
            Popup.Init(this);

            //版本更新配置
            AutoUpdate.Init(this, "com.dcms.clientv3.fileprovider");

            //初始 Xamarin.Forms 实验性标志
            Forms.SetFlags("Shell_Experimental",
                "SwipeView_Experimental",
                "CarouselView_Experimental",
                "RadioButton_Experimental",
                "IndicatorView_Experimental",
                "Expander_Experimental",
                "Visual_Experimental",
                "CollectionView_Experimental",
                "FastRenderers_Experimental");

            #region //初始 FFImageLoading

            CachedImageRenderer.Init(true);
            CachedImageRenderer.InitImageViewHandler();
            ImageCircleRenderer.Init();
            var config = new FFImageLoading.Config.Configuration()
            {
                VerboseLogging = false,
                VerbosePerformanceLogging = false,
                VerboseMemoryCacheLogging = false,
                VerboseLoadingCancelledLogging = false,
                //Logger = new CustomLogger(),
            };
            ImageService.Instance.Initialize(config);

            #endregion 


            Platform.Init(this, bundle);
            Forms.Init(this, bundle);

            //初始 对话框组件
            UserDialogs.Init(this);

            //初始 ZXing
            ZXingPlatform.Platform.Init();

            //注册Effects
            Effects.Droid.Effects.Init(this);

            this.LoadApplication(new App(new AndroidInitializer()));

            //this.LoadApplication(new App());
  
            this.ShinyOnCreate();

            //在andrioid m之后，必须在运行时请求权限
            GetPersimmions();
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            //Activity加载完毕（完成渲染）清空背景图片
            base.Window.SetBackgroundDrawable(null);

            if (hasFocus)
            {
                //状态栏
                base.Window.AddFlags(WindowManagerFlags.Fullscreen);
                base.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                base.Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
                //if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                //{
                //    base.Window.DecorView.SetFitsSystemWindows(true);
                //    base.Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
                //}
                //移除Activity
                //ActivityCollector.Finish("SplashActivity");
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted)
            {
                RequestPermissions(new string[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation }, 1);
            }
            InitGPS();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            //移除Activity
            ActivityCollector.RemoveActivity(this);
            //释放电源锁
            ReleaseWakeLock();
        }

        /// <summary>
        /// 软导航后退
        /// </summary>
        public override void OnBackPressed()
        {
            var actionPage = App.Current.MainPage;
            if (!Wesley.BitImageEditor.ImageEditor.IsOpened)
                Wesley.BitImageEditor.Droid.Platform.OnBackPressed();

            if (actionPage?.Navigation != null && actionPage?.Navigation?.NavigationStack?.Count != 0)
            {
                actionPage = actionPage.Navigation.NavigationStack.Last();
                if (null != actionPage && actionPage is MainLayoutPage)
                {
                    if (isBacking)
                    {
                        Finish();
                    }
                    else
                    {
                        isBacking = true;
                        ToastUtils.ShowSingleToast("再按一次退出");
                    }
                }
                else if (actionPage is LoginPage)
                {
                    MoveTaskToBack(true);
                }
                else
                {
                    Popup.SendBackPressed(base.OnBackPressed);
                }
            }
            else
            {
                Popup.SendBackPressed(base.OnBackPressed);
            }
        }

        DateTime? lastBackKeyDownTime;//记录上次按下Back的时间
        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Down)
            {
                if (!lastBackKeyDownTime.HasValue || DateTime.Now - lastBackKeyDownTime.Value > new TimeSpan(0, 0, 2))
                {
                    var actionPage = App.Current.MainPage;
                    if (actionPage?.Navigation != null && actionPage?.Navigation?.NavigationStack?.Count != 0)
                    {
                        actionPage = actionPage.Navigation.NavigationStack.Last();
                        if (null != actionPage && actionPage is MainLayoutPage)
                        {
                            ToastUtils.ShowSingleToast("再按一次退出程序");
                            lastBackKeyDownTime = DateTime.Now;
                            return true;
                        }
                        else if (actionPage is LoginPage)
                        {
                            MoveTaskToBack(true);
                        }
                        else
                        {
                            if (actionPage != null)
                            {
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    Popup.SendBackPressed(base.OnBackPressed);
                                    await actionPage?.Navigation.PopAsync();
                                });
                            }
                            return false;
                        }
                    }
                    else
                    {
                        ToastUtils.ShowSingleToast("再按一次退出程序");
                        lastBackKeyDownTime = DateTime.Now;
                    }
                }
                else
                {
                    Intent intent = new Intent();
                    intent.SetClass(this, typeof(SplashActivity));
                    StartActivity(intent);
                }

                return true;
            }

            return base.OnKeyDown(keyCode, e);
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            base.OnTrimMemory(level);
        }

        public override void OnLowMemory()
        {
            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            base.OnLowMemory();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            this.ShinyOnNewIntent(intent);
        }

        private static readonly int REQUEST_CODE_GPS = 1;
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);
            Wesley.BitImageEditor.Droid.Platform.OnActivityResult(requestCode, resultCode, intent);
        }

        private void InitGPS()
        {
            LocationManager locationManager = (LocationManager)GetSystemService(Context.LocationService);
            //判断GPS是否开启，没有开启，则开启
            if (!locationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                OpenGPSDialog();
            }
        }

        private void OpenGPSDialog()
        {
            AlertDialog.Builder b = new AlertDialog.Builder(this);
            b.SetTitle("请打开GPS连接")
                .SetIcon(Resource.Drawable.water_drop)
                .SetMessage("为了提高定位的准确度，更好的为您服务，请打开GPS")
                .SetPositiveButton("设置", (s, e) =>
                {
                    //跳转到手机打开GPS页面
                    Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                    //设置完成后返回原来的界面
                    StartActivityForResult(intent, REQUEST_CODE_GPS);

                }).SetNeutralButton("取消", (s, e) =>
                { 
                });
            b.Create();
            b.Show();
        }

        /// <summary>
        /// 权限请求
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="permissions"></param>
        /// <param name="grantResults"></param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            //Essentials
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //Shiny
            this.ShinyOnRequestPermissionsResult(requestCode, permissions, grantResults);
            //Base
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void AcquireWakeLock()
        {
            if (null == _wakeLock)
            {
                PowerManager pm = (PowerManager)GetSystemService(Context.PowerService);
                _wakeLock = pm.NewWakeLock(WakeLockFlags.Partial | WakeLockFlags.OnAfterRelease, "MyWakelockTag");
                if (_wakeLock != null)
                {
                    _wakeLock.Acquire();
                }
            }
        }

        private void ReleaseWakeLock()
        {
            if (null != _wakeLock || _wakeLock.IsHeld)
            {
                _wakeLock.Release();
                _wakeLock = null;
            }
        }

        #endregion


        /// <summary>
        /// 请求权限
        /// </summary>
        [TargetApi(Value = 23)]
        private void GetPersimmions()
        {
            //var currentActivity = MainActivity.Instance;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                List<string> permissions = new List<string>();
                /***
                 * 定位权限为必须权限，用户如果禁止，则每次进入都会申请
                 */
                // 定位精确位置
                if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    permissions.Add(Manifest.Permission.AccessFineLocation);
                }

                if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.AccessBackgroundLocation) != Permission.Granted)
                {
                    permissions.Add(Manifest.Permission.AccessBackgroundLocation);
                }

                if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.ForegroundService) != Permission.Granted)
                {
                    permissions.Add(Manifest.Permission.ForegroundService);
                }

                if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.AccessNetworkState) != Permission.Granted)
                {
                    permissions.Add(Manifest.Permission.AccessNetworkState);
                }

                if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.Internet) != Permission.Granted)
                {
                    permissions.Add(Manifest.Permission.Internet);
                }

                if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
                {
                    permissions.Add(Manifest.Permission.AccessCoarseLocation);
                }

                if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.AccessNetworkState) != Permission.Granted)
                {
                    permissions.Add(Manifest.Permission.AccessNetworkState);
                }

                if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                {
                    permissions.Add(Manifest.Permission.ReadExternalStorage);
                }

                if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.WriteSettings) != Permission.Granted)
                {
                    permissions.Add(Manifest.Permission.WriteSettings);
                }

                if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.AccessWifiState) != Permission.Granted)
                {
                    permissions.Add(Manifest.Permission.AccessWifiState);
                }

                //读写权限和电话状态权限非必要权限(建议授予)只会申请一次，用户同意或者禁止，只会弹一次
                // 读写权限
                if (AddPermission(permissions, Manifest.Permission.WriteExternalStorage))
                {
                    //permissionInfo += "Manifest.permission.WRITE_EXTERNAL_STORAGE Deny \n";
                }

                if (permissions.Count > 0)
                {
                    //private final int SDK_PERMISSION_REQUEST = 127;
                    RequestPermissions(permissions.ToArray(), 127);
                }
            }

        }

        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="permissionsList"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        [TargetApi(Value = 23)]
        private bool AddPermission(List<string> permissionsList, string permission)
        {
            // 如果应用没有获得对应权限,则添加到列表中,准备批量申请
            if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
            {
                if (ShouldShowRequestPermissionRationale(permission))
                {
                    return true;
                }
                else
                {
                    permissionsList.Add(permission);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

    }


    /// <summary>
    /// Ensure the Akavache.Sqlite3 dll will not be removed by Xamarin build tools
    /// </summary>
    public static class LinkerPreserve
    {
        static LinkerPreserve()
        {
            _ = typeof(SQLitePersistentBlobCache).FullName;
            _ = typeof(SQLiteEncryptedBlobCache).FullName;
        }
    }

    public class BaiduLocationServiceImpl : IBaiduLocationService
    {
        private static LocationClient client = null;
        private static LocationClientOption mOption;
        private static MyLocationListener myLocationListener;
        //private readonly Context _context;
        private bool IsDisposed = false;

        //private Notification mNotification;
        private readonly MapView _mapView;
        private readonly object objLock = new Object();

        public BaiduLocationServiceImpl(MapView mapView, Context context)
        {
            _mapView = mapView;
            //_context = context;

            // 注册定位监听
            myLocationListener = new MyLocationListener((lct) =>
            {
                if (_mapView != null && lct != null)
                {
                    //初始化坐标转换工具类，指定源坐标类型和坐标数据
                    CoordinateConverter converter = new CoordinateConverter()
                            .From(CoordinateConverter.CoordType.Gps)
                            .Coord(new Com.Baidu.Mapapi.Model.LatLng(lct.Latitude, lct.Longitude));
                    //desLatLng 转换后的坐标
                    var lcts = converter.Convert();
                    GlobalSettings.UpdatePoi(lct.Latitude, lct.Longitude);

                    //此处设置开发者获取到的方向信息，顺时针0 - 360
                    var locData = new MyLocationData.Builder()
                        .Accuracy(lct.Radius)
                        .Direction(lct.Direction).Latitude(lcts.Latitude)
                        .Longitude(lcts.Longitude)
                        .Build();

                    try
                    {
                        if (_mapView != null && _mapView.Map != null)
                            _mapView.Map.SetMyLocationData(locData);
                    }
                    catch (System.ObjectDisposedException )
                    { 
                    
                    }
                }
            });

            client = new LocationClient(context)
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
                mOption.ScanSpan = 3000;
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

        public void Converter(Map map,double lat,double lng)
        {
            //初始化坐标转换工具类，指定源坐标类型和坐标数据
            CoordinateConverter converter = new CoordinateConverter()
                    .From(CoordinateConverter.CoordType.Gps)
                    .Coord(new Com.Baidu.Mapapi.Model.LatLng(lat, lng));

            //desLatLng 转换后的坐标
            var lct = converter.Convert();

            if (map != null)
                map.Center = new Coordinate(lct.Latitude, lct.Longitude);

          
            //当前位置
            if (!IsDisposed)
            {
                // 此处设置开发者获取到的方向信息，顺时针0-360
                var locData = new MyLocationData.Builder().Latitude(lct.Latitude)
                    .Longitude(lct.Longitude)
                    .Build();

                try
                {
                    if (_mapView != null && _mapView.Map != null)
                        _mapView.Map.SetMyLocationData(locData);
                }
                catch (System.ObjectDisposedException)
                {

                }
            }

            GlobalSettings.UpdatePoi(lct.Latitude, lct.Longitude);
        }

        public void Converter(double lat, double lng)
        {
            //初始化坐标转换工具类，指定源坐标类型和坐标数据
            CoordinateConverter converter = new CoordinateConverter()
                    .From(CoordinateConverter.CoordType.Gps)
                    .Coord(new Com.Baidu.Mapapi.Model.LatLng(lat, lng));
            //desLatLng 转换后的坐标
            var lct = converter.Convert();
            GlobalSettings.UpdatePoi(lct.Latitude, lct.Longitude);
        }


        public void Start()
        {
            lock (objLock)
            {
                IsDisposed = false;

                if (client != null && !client.IsStarted)
                {
                    RegisterListener(myLocationListener);
                    //启用前台服务
                    //ForegroundNotification(_context);
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
                    //关闭前台定位，同时移除通知栏
                    client?.DisableLocInForeground(true);
                    client.Stop();
                }
            }
        }

        public void OnDestroy()
        {
            try
            {
                lock (objLock)
                {
                    IsDisposed = true;

                    if (client != null)
                    {
                        //停止Baidu定位服务
                        if (client.IsStarted)
                            client?.Stop();

                        //关闭前台定位服务
                        client?.DisableLocInForeground(true);

                        //取消之前注册的 BDAbstractLocationListener 定位监听函数
                        UnregisterListener(myLocationListener);
                    }
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

        ///// <summary>
        ///// android8.0 初始化前台服务
        ///// </summary>
        //private void ForegroundNotification(Context context)
        //{
        //    try
        //    {
        //        if (context == null)
        //            return;

        //        if (mNotification == null)
        //        {
        //            //设置后台定位
        //            //android8.0及以上使用NotificationUtils
        //            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        //            {
        //                var notificationUtils = new NotificationUtils(context);
        //                var builder = notificationUtils.GetAndroidChannelNotification("Wesley", "正在后台定位");
        //                mNotification = builder.Build();
        //            }
        //            else
        //            {
        //                //获取一个Notification构造器
        //                Notification.Builder builder = new Notification.Builder(context, NotificationUtils.ANDROID_CHANNEL_ID);
        //                Intent nfIntent = new Intent(context, typeof(MainActivity));

        //                var currenttimemillis = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

        //                builder.SetContentIntent(PendingIntent.GetActivity(context, 0, nfIntent, 0))
        //                    .SetContentTitle("Wesley定位服务")
        //                    .SetSmallIcon(Android.Resource.Mipmap.SymDefAppIcon)
        //                    .SetContentText("正在后台定位")
        //                    .SetWhen(currenttimemillis);

        //                //获取构建好的Notification
        //                mNotification = builder.Build();
        //            }

        //            // 将定位SDK的SERVICE设置成为前台服务, 提高定位进程存活率
        //            client?.EnableLocInForeground(1001, mNotification);
        //        }
        //    }
        //    catch (Java.Lang.Exception ex)
        //    {
        //        Crashes.TrackError(ex);
        //    }
        //}

    }

    /// <summary>
    /// 侦听器
    /// </summary>
    public class MyLocationListener : BDAbstractLocationListener
    {
        readonly Action<BDLocation> _update;

        public MyLocationListener(Action<BDLocation> update)
        {
            _update = update;
        }

        public override void OnReceiveLocation(BDLocation lct)
        {
            if (lct == null)
                return;

            try
            {
                _update?.Invoke(lct);

                var asyncTasker = new AsyncTasker(async () =>
                {
                    try
                    {
                        if (lct != null)
                        {
                            var msg = $"OnReceiveLocation:{lct.Latitude} Latitude:{lct.Longitude} Address:{lct.Country}{lct.Province}{lct.City}{lct.Address?.Street}";
                            Android.Util.Log.Info("获取位置：", msg);

                            //GlobalSettings.UpdatePoi(lct.Latitude, lct.Longitude);

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
                                Address = $"{lct.Country}{lct.Province}{lct.City}{lct.Address?.Street}"
                            };

                            //存储本地  
                            if (!string.IsNullOrWhiteSpace(tracking.Address))
                            {
                                var lg = await _conn.LocationSyncEvents.CountAsync();
                                if (lg > 50)
                                {
                                    await _conn.RemoveTopLocation();
                                }
                                await _conn.InsertAsync(tracking);
                            }
                        }
                    }
                    catch (Java.Lang.Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                });
                asyncTasker.Execute();

            }
            catch (Java.Lang.Exception ex)
            {
                Android.Util.Log.Error("上报", ex.Message);
                Crashes.TrackError(ex);
            }
        }
    }

    public class AsyncTasker : AsyncTask<Java.Lang.Void, Java.Lang.Void, Java.Lang.Void>
    {
        private Action Action { get; }
        public event EventHandler<object> TaskCompleted;
        public void NotifyCompletedTask(object result)
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            {
                this.TaskCompleted?.Invoke(this, result);
            });
        }
        public AsyncTasker(Action action)
        {
            this.Action = action;
        }
        protected override Java.Lang.Void RunInBackground(params Java.Lang.Void[] @params)
        {
            if (this.Action == null)
                return null;
            this.Action();
            return null;
        }
    }

    public class ActivityCollector
    {
        private static readonly Dictionary<string, Activity> Activities = new Dictionary<string, Activity>();
        public static void AddActivity(Activity activity)
        {
            var name = activity.GetType().Name;
            if (Activities.Where(s => s.Key == name).Count() == 0)
                Activities.Add(name, activity);
        }
        public static void RemoveActivity(Activity activity)
        {
            if (Activities.Any())
                Activities.Remove(activity.GetType().Name);
        }

        public static void FinishAll()
        {
            foreach (var activity in Activities)
            {
                if (!activity.Value.IsFinishing)
                {
                    activity.Value.Finish();
                }
            }
            Activities.Clear();
        }

        public static void Finish(string key)
        {
            var ac = Activities.Where(s => s.Key == key).FirstOrDefault();
            if (ac.Value != null)
            {
                ac.Value.Finish();
                Activities.Remove(ac.Key);
            }
        }
    }
}

