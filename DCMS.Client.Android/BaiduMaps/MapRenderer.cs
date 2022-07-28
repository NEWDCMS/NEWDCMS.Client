using Android.Content;
using Com.Baidu.Mapapi.Map;
using Com.Baidu.Mapapi.Model;
using DCMS.Client.BaiduMaps;
using DCMS.Client.Droid;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AG = Android.Graphics;

[assembly: ExportRenderer(typeof(Map), typeof(MapRenderer))]
namespace DCMS.Client.Droid
{
    public partial class MapRenderer : ViewRenderer<Map, MapView>, BaiduMap.IOnMapLoadedCallback
    {
        protected MapView NativeMap => Control;
        protected Map Map => Element;
        private bool _isDisposed;


        /// <summary>
        /// 标注
        /// </summary>
        private readonly PinImpl pinImpl = new PinImpl();

        /// <summary>
        /// 折线绘制
        /// </summary>
        private readonly PolylineImpl polylineImpl = new PolylineImpl();

        /// <summary>
        /// 多边绘制
        /// </summary>
        private readonly PolygonImpl polygonImpl = new PolygonImpl();

        /// <summary>
        /// 圆绘制
        /// </summary>
        private readonly CircleImpl circleImpl = new CircleImpl();

        public MapRenderer(Android.Content.Context context) : base(context) { }

        /// <summary>
        /// 释放（注意：MapView生命周期）
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;

                if (disposing && Element != null)
                {
                    //清除所有标注
                    if (Map.Pins.Count > 0)
                        Map.Pins.Clear();

                    //((BaiduLocationServiceImpl)Map?.LocationService).OnDestroy();

                    pinImpl.Unregister(Map);
                    polylineImpl.Unregister(Map);
                    polygonImpl.Unregister(Map);
                    circleImpl.Unregister(Map);

                    //清除所有图层
                    NativeMap?.Map?.Clear();

                    //卸载事件
                    NativeMap.Map.MapClick -= OnMapClick;
                    NativeMap.Map.MapPoiClick -= OnMapPoiClick;
                    NativeMap.Map.MapDoubleClick -= OnMapDoubleClick;
                    NativeMap.Map.MapLongClick -= OnMapLongClick;
                    NativeMap.Map.MarkerClick -= OnMarkerClick;
                    NativeMap.Map.MarkerDragStart -= OnMarkerDragStart;
                    NativeMap.Map.MarkerDrag -= OnMarkerDrag;
                    NativeMap.Map.MarkerDragEnd -= OnMarkerDragEnd;
                    NativeMap.Map.MapStatusChangeFinish -= MapStatusChangeFinish;
                    NativeMap.Map.SetOnMapLoadedCallback(null);
                    NativeMap.OnDestroy();
                }
            }
            catch (Exception)
            {

            }
            base.Dispose(disposing);
        }

        public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            return new SizeRequest(new Size(Context.ToPixels(0), Context.ToPixels(0)));
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (null != e.OldElement)
            {
                var oldMap = e.OldElement;

                if (oldMap.Pins.Count > 0)
                    oldMap.Pins.Clear();

                //((BaiduLocationServiceImpl)oldMap?.LocationService).OnDestroy();

                MapView oldMapView = Control;

                oldMapView.Map.Clear();
                oldMapView.Map.MapClick -= OnMapClick;
                oldMapView.Map.MapPoiClick -= OnMapPoiClick;
                oldMapView.Map.MapDoubleClick -= OnMapDoubleClick;
                oldMapView.Map.MapLongClick -= OnMapLongClick;
                oldMapView.Map.MarkerClick -= OnMarkerClick;
                oldMapView.Map.MarkerDragStart -= OnMarkerDragStart;
                oldMapView.Map.MarkerDrag -= OnMarkerDrag;
                oldMapView.Map.MarkerDragEnd -= OnMarkerDragEnd;
                oldMapView.Map.MapStatusChangeFinish -= MapStatusChangeFinish;
                oldMapView.Map.SetOnMapLoadedCallback(null);
                //
                oldMapView.OnDestroy();
            }

            if (null != e.NewElement)
            {
                //仅当 e.NewElement 属性不是 null 且 Control 属性为null 时，才应调用 SetNativeControl 方法。
                if (null == Control)
                {
                    SetNativeControl(new MapView(Context));
                }

                //Map.LocationService = new BaiduLocationServiceImpl(NativeMap, Context);

                NativeMap.Map.MyLocationEnabled = true;
                NativeMap.Map.MapClick += OnMapClick;
                NativeMap.Map.MapPoiClick += OnMapPoiClick;
                NativeMap.Map.MapDoubleClick += OnMapDoubleClick;
                NativeMap.Map.MapLongClick += OnMapLongClick;
                NativeMap.Map.MarkerClick += OnMarkerClick;
                NativeMap.Map.MarkerDragStart += OnMarkerDragStart;
                NativeMap.Map.MarkerDrag += OnMarkerDrag;
                NativeMap.Map.MarkerDragEnd += OnMarkerDragEnd;
                NativeMap.Map.MapStatusChangeFinish += MapStatusChangeFinish;
                NativeMap.Map.SetOnMapLoadedCallback(this);

                NativeMap.ShowZoomControls(false);

                UpdateMapType();
                UpdateUserTrackingMode();
                UpdateShowUserLocation();

                UpdateShowCompass();
                UpdateCompassPosition();

                UpdateZoomToSpan();
                UpdateZoomLevel();
                UpdateMinZoomLevel();
                UpdateMaxZoomLevel();

                UpdateCenter();
                UpdateShowScaleBar();
                UpdateShowZoomControl();

                UpdateOverlay();

                pinImpl.Unregister(e.OldElement);
                pinImpl.Register(Map, NativeMap);

                polylineImpl.Unregister(e.OldElement);
                polylineImpl.Register(Map, NativeMap);

                polygonImpl.Unregister(e.OldElement);
                polygonImpl.Register(Map, NativeMap);

                circleImpl.Unregister(e.OldElement);
                circleImpl.Register(Map, NativeMap);

            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("Height" == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("Height = " + Map.Height);
                return;
            }

            if ("Width" == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("Width = " + Map.Width);
                return;
            }

            if (Map.MapTypeProperty.PropertyName == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("MapType = " + Map.MapType);
                UpdateMapType();
                return;
            }

            if (Map.UserTrackingModeProperty.PropertyName == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("UserTrackingMode = " + Map.UserTrackingMode);
                UpdateUserTrackingMode();
                return;
            }


            if (Map.ZoomToSpanProperty.PropertyName == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("ZoomToSpan = " + Map.ZoomToSpan);
                UpdateZoomToSpan();
                return;
            }

            if (Map.ShowUserLocationProperty.PropertyName == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("ShowUserLocation = " + Map.ShowUserLocation);
                UpdateShowUserLocation();
                return;
            }

            if (Map.ShowCompassProperty.PropertyName == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("ShowCompass = " + Map.ShowCompass);
                UpdateShowCompass();
                return;
            }

            if (Map.CompassPositionProperty.PropertyName == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("CompassPosition = " + Map.CompassPosition);
                UpdateCompassPosition();
                return;
            }

            if (Map.ZoomLevelProperty.PropertyName == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("ZoomLevel = " + Map.ZoomLevel);
                UpdateZoomLevel();
                return;
            }

            if (Map.MinZoomLevelProperty.PropertyName == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("MinZoomLevel = " + Map.MinZoomLevel);
                UpdateMinZoomLevel();
                return;
            }

            if (Map.MaxZoomLevelProperty.PropertyName == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("MaxZoomLevel = " + Map.MaxZoomLevel);
                UpdateMaxZoomLevel();
                return;
            }

            if (Map.CenterProperty.PropertyName == e.PropertyName)
            {
                // System.Diagnostics.Debug.WriteLine("Center = " + Map.Center);
                UpdateCenter();
                return;
            }

            if (Map.ShowScaleBarProperty.PropertyName == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("ShowScaleBar = " + Map.ShowScaleBar);
                UpdateShowScaleBar();
                return;
            }

            if (Map.ShowZoomControlProperty.PropertyName == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("ShowZoomControl = " + Map.ShowZoomControl);
                UpdateShowZoomControl();
                return;
            }


            if (Map.ClearOverlayProperty.PropertyName == e.PropertyName)
            {
                //System.Diagnostics.Debug.WriteLine("ClearOverlay = " + Map.ClearOverlay);
                UpdateOverlay();
                return;
            }

            //System.Diagnostics.Debug.WriteLine("OnElementPropertyChanged: " + e.PropertyName);
            base.OnElementPropertyChanged(sender, e);
        }

        private void UpdateMapType()
        {
            switch (Map.MapType)
            {
                case MapType.None:
                    NativeMap.Map.MapType = 0;
                    break;

                case MapType.Standard:
                    NativeMap.Map.MapType = 1;
                    break;

                case MapType.Satellite:
                    NativeMap.Map.MapType = 2;
                    break;
            }
        }

        private void UpdateUserTrackingMode()
        {
            if (Map != null)
            {
                var mode = Map.UserTrackingMode switch
                {
                    UserTrackingMode.Follow => MyLocationConfiguration.LocationMode.Following,
                    UserTrackingMode.FollowWithCompass => MyLocationConfiguration.LocationMode.Compass,
                    _ => MyLocationConfiguration.LocationMode.Normal,
                };

                if (NativeMap.Map != null)
                    NativeMap.Map.SetMyLocationConfiguration(new MyLocationConfiguration(mode, true, null));

                if (UserTrackingMode.FollowWithCompass != Map.UserTrackingMode)
                {
                    // 恢复俯视角
                    MapStatusUpdate overlook = MapStatusUpdateFactory.NewMapStatus(
                        new MapStatus.Builder(NativeMap.Map.MapStatus).Rotate(0).Overlook(0).Build()
                    );
                    if (NativeMap.Map != null)
                        NativeMap.Map.AnimateMapStatus(overlook);
                }
            }
        }

        private void UpdateZoomToSpan()
        {
            ZoomToSpan();
        }

        private void UpdateShowUserLocation()
        {
            if (NativeMap.Map != null)
                NativeMap.Map.MyLocationEnabled = Map.ShowUserLocation;
        }

        private void UpdateShowCompass()
        {
            if (NativeMap.Map != null)
                NativeMap.Map.UiSettings.CompassEnabled = Map.ShowCompass;
        }

        private void UpdateCompassPosition()
        {
            if (NativeMap.Map != null)
            {
                NativeMap.Map.CompassPosition = new AG.Point
                {
                    X = (int)Map.CompassPosition.X,
                    Y = (int)Map.CompassPosition.Y
                };
            }
        }

        private void UpdateZoomLevel()
        {
            if (NativeMap.Map != null)
                NativeMap.Map.AnimateMapStatus(
                MapStatusUpdateFactory.ZoomTo(Map.ZoomLevel)
            );
        }

        private void UpdateMinZoomLevel()
        {
            if (NativeMap.Map != null)
                NativeMap.Map.SetMaxAndMinZoomLevel(Map.MaxZoomLevel, Map.MinZoomLevel);
        }

        private void UpdateMaxZoomLevel()
        {
            if (NativeMap.Map != null)
                NativeMap.Map.SetMaxAndMinZoomLevel(Map.MaxZoomLevel, Map.MinZoomLevel);
        }

        private void UpdateCenter()
        {
            if (NativeMap.Map != null)
            {
                var locData = new MyLocationData.Builder().Latitude(Map.Center.Latitude).Longitude(Map.Center.Longitude).Build();
                NativeMap.Map.AnimateMapStatus(MapStatusUpdateFactory.NewLatLng(Map.Center.ToNative()));
                NativeMap.Map.SetMyLocationData(locData);
            }
        }

        private void UpdateShowScaleBar()
        {
            if (NativeMap.Map != null)
                NativeMap.ShowScaleControl(Map.ShowScaleBar);
        }

        private void UpdateShowZoomControl()
        {
            if (NativeMap.Map != null)
                NativeMap.ShowZoomControls(Map.ShowZoomControl);
            //SetZoomControlsPosition new Point(150,60)
            //NativeMap.SetZoomControlsPosition(new AG.Point(10, 100));
        }

        /// <summary>
        /// 清除所有覆盖物
        /// </summary>
        private void UpdateOverlay()
        {
            if (Map != null)
            {
                if (Map.ClearOverlay)
                {
                    Map.Pins.Clear();
                    Map.ClearOverlay = false;
                }
            }
        }

        /// <summary>
        /// 缩放地图，使所有Overlay都在合适的视野内
        /// 注： 该方法只对Marker类型的overlay有效
        /// </summary>
        public void ZoomToSpan()
        {
            ////获取地图可视区域 
            //var bounds = map.GetBounds();
            ////获取西南角的经纬度(左下端点)
            //var sw = bounds.getSouthWest();
            ////获取东北角的经纬度(右上端点)
            //var ne = bounds.getNorthEast();
            ////获取西北角的经纬度(左上端点)
            //var wn = new BMap.Point(sw.lng, ne.lat);
            ////获取东南角的经纬度(右下端点)
            //var es = new BMap.Point(ne.lng, sw.lat);  


            if (NativeMap.Map == null)
            {
                return;
            }
            var mOverlayList = Map.Pins;
            if (mOverlayList.Count > 0)
            {
                LatLngBounds.Builder builder = new LatLngBounds.Builder();
                foreach (var item in mOverlayList)
                {
                    // polyline 中的点可能太多，只按marker 缩放
                    var overlay = (Marker)item?.NativeObject;
                    if (overlay != null)
                    {
                        builder.Include(((Marker)overlay).Position);
                    }
                }

                MapStatus mapStatus = NativeMap.Map.MapStatus;
                if (null != mapStatus)
                {
                    int width = mapStatus.WinRound.Right - NativeMap.Map.MapStatus.WinRound.Left - 400;
                    int height = mapStatus.WinRound.Bottom - NativeMap.Map.MapStatus.WinRound.Top - 400;
                    NativeMap.Map.SetMapStatus(MapStatusUpdateFactory.NewLatLngBounds(builder.Build(), width, height));
                }

            }
        }

        /// <summary>
        /// 设置显示在规定宽高中的地图地理范围
        /// </summary>
        /// <param name="paddingLeft"></param>
        /// <param name="paddingTop"></param>
        /// <param name="paddingRight"></param>
        /// <param name="paddingBottom"></param>
        public void ZoomToSpanPaddingBounds(int paddingLeft, int paddingTop, int paddingRight, int paddingBottom)
        {
            if (NativeMap.Map == null)
            {
                return;
            }
            var mOverlayList = Map.Pins;
            if (mOverlayList.Count > 0)
            {
                LatLngBounds.Builder builder = new LatLngBounds.Builder();
                foreach (var item in mOverlayList)
                {
                    // polyline 中的点可能太多，只按marker 缩放
                    var overlay = (Marker)item?.NativeObject;
                    if (overlay != null)
                    {
                        builder.Include(((Marker)overlay).Position);
                    }
                }
                NativeMap.Map.SetMapStatus(MapStatusUpdateFactory
                   .NewLatLngBounds(builder.Build(), paddingLeft, paddingTop, paddingRight, paddingBottom));
            }
        }

    }
}

