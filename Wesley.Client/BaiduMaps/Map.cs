using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
namespace Wesley.Client.BaiduMaps
{
    public enum MapType
    {
        None,
        Standard,
        Satellite
    }
    public enum UserTrackingMode
    {
        None,
        Follow,
        FollowWithCompass
    }

    public class Map : View
    {
        public Map()
        {
            VerticalOptions = HorizontalOptions = LayoutOptions.FillAndExpand;
            //pointAnnotations.CollectionChanged += AnnotationsCollectionChanged;
        }

        /// <summary>
        /// 地图坐标类型
        /// </summary>
        public static readonly BindableProperty MapTypeProperty = BindableProperty.Create(
            propertyName: nameof(MapType),
            returnType: typeof(MapType),
            declaringType: typeof(Map),
            defaultValue: MapType.Standard
        );
        public MapType MapType
        {
            get { return (MapType)GetValue(MapTypeProperty); }
            set { SetValue(MapTypeProperty, value); }
        }

        /// <summary>
        /// 使用跟踪模式
        /// </summary>
        public static readonly BindableProperty UserTrackingModeProperty = BindableProperty.Create(
            propertyName: nameof(UserTrackingMode),
            returnType: typeof(UserTrackingMode),
            declaringType: typeof(Map),
            defaultValue: UserTrackingMode.None
        );
        public UserTrackingMode UserTrackingMode
        {
            get { return (UserTrackingMode)GetValue(UserTrackingModeProperty); }
            set { SetValue(UserTrackingModeProperty, value); }
        }

        /// <summary>
        /// 显示用户位置
        /// </summary>
        public static readonly BindableProperty ShowUserLocationProperty = BindableProperty.Create(
            propertyName: nameof(ShowUserLocation),
            returnType: typeof(bool),
            declaringType: typeof(Map),
            defaultValue: true
        );
        public bool ShowUserLocation
        {
            get { return (bool)GetValue(ShowUserLocationProperty); }
            set { SetValue(ShowUserLocationProperty, value); }
        }

        /// <summary>
        /// 缩放合适区域
        /// </summary>
        public static readonly BindableProperty ZoomToSpanProperty = BindableProperty.Create(
             propertyName: nameof(ZoomToSpan),
             returnType: typeof(bool),
             declaringType: typeof(Map),
             defaultValue: true
         );
        public bool ZoomToSpan
        {
            get { return (bool)GetValue(ZoomToSpanProperty); }
            set { SetValue(ZoomToSpanProperty, value); }
        }

        /// <summary>
        /// 显示指南针
        /// </summary>
        public static readonly BindableProperty ShowCompassProperty = BindableProperty.Create(
            propertyName: nameof(ShowCompass),
            returnType: typeof(bool),
            declaringType: typeof(Map),
            defaultValue: true
        );
        public bool ShowCompass
        {
            get { return (bool)GetValue(ShowCompassProperty); }
            set { SetValue(ShowCompassProperty, value); }
        }

        /// <summary>
        /// 指南针位置
        /// </summary>
        public static readonly BindableProperty CompassPositionProperty = BindableProperty.Create(
            propertyName: nameof(CompassPosition),
            returnType: typeof(Point),
            declaringType: typeof(Map),
            defaultValue: new Point(40, 40)
        );
        public Point CompassPosition
        {
            get { return (Point)GetValue(CompassPositionProperty); }
            set { SetValue(CompassPositionProperty, value); }
        }

        /// <summary>
        /// 缩放比例
        /// </summary>
        public static readonly BindableProperty ZoomLevelProperty = BindableProperty.Create(
            propertyName: nameof(ZoomLevel),
            returnType: typeof(float),
            declaringType: typeof(Map),
            defaultValue: 11f
        );
        public float ZoomLevel
        {
            get { return (float)GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }

        /// <summary>
        /// 最小缩放比例
        /// </summary>
        public static readonly BindableProperty MinZoomLevelProperty = BindableProperty.Create(
            propertyName: nameof(MinZoomLevel),
            returnType: typeof(float),
            declaringType: typeof(Map),
            defaultValue: 3f
        );
        public float MinZoomLevel
        {
            get { return (float)GetValue(MinZoomLevelProperty); }
            set { SetValue(MinZoomLevelProperty, value); }
        }

        /// <summary>
        /// 最大缩放比例
        /// </summary>
        public static readonly BindableProperty MaxZoomLevelProperty = BindableProperty.Create(
            propertyName: nameof(MaxZoomLevel),
            returnType: typeof(float),
            declaringType: typeof(Map),
            defaultValue: 22f
        );
        public float MaxZoomLevel
        {
            get { return (float)GetValue(MaxZoomLevelProperty); }
            set { SetValue(MaxZoomLevelProperty, value); }
        }

        /// <summary>
        /// 中心位置
        /// </summary>
        public static readonly BindableProperty CenterProperty = BindableProperty.Create(
            propertyName: nameof(Center),
            returnType: typeof(Coordinate),
            declaringType: typeof(Map),
            defaultValue: new Coordinate(28.693, 115.958)
        );
        public Coordinate Center
        {
            get { return (Coordinate)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }


        /// <summary>
        /// 显示缩放工具条
        /// </summary>
        public static readonly BindableProperty ShowScaleBarProperty = BindableProperty.Create(
            propertyName: nameof(ShowScaleBar),
            returnType: typeof(bool),
            declaringType: typeof(Map),
            defaultValue: true
        );
        public bool ShowScaleBar
        {
            get { return (bool)GetValue(ShowScaleBarProperty); }
            set { SetValue(ShowScaleBarProperty, value); }
        }

        /// <summary>
        /// 清除覆盖物
        /// </summary>
        public static readonly BindableProperty ClearOverlayProperty = BindableProperty.Create(
           propertyName: nameof(ClearOverlay),
           returnType: typeof(bool),
           declaringType: typeof(Map),
           defaultValue: false
       );
        public bool ClearOverlay
        {
            get { return (bool)GetValue(ClearOverlayProperty); }
            set { SetValue(ClearOverlayProperty, value); }
        }


        /// <summary>
        /// 显示缩放工具栏
        /// </summary>
        public static readonly BindableProperty ShowZoomControlProperty = BindableProperty.Create(
            propertyName: nameof(ShowZoomControl),
            returnType: typeof(bool),
            declaringType: typeof(Map),
            defaultValue: true
        );
        public bool ShowZoomControl
        {
            get { return (bool)GetValue(ShowZoomControlProperty); }
            set { SetValue(ShowZoomControlProperty, value); }
        }


        // public IBaiduLocationService LocationService { get; set; }
        public IProjection Projection { get; set; }
        public IList<Pin> Pins => pins;

        private readonly ObservableCollection<Pin> pins = new ObservableCollection<Pin>();

        public IList<Polyline> Polylines => polylines;
        private readonly ObservableCollection<Polyline> polylines = new ObservableCollection<Polyline>();

        public IList<Polygon> Polygons => polygons;
        private readonly ObservableCollection<Polygon> polygons = new ObservableCollection<Polygon>();

        public IList<Circle> Circles => circles;
        private readonly ObservableCollection<Circle> circles = new ObservableCollection<Circle>();

        public event EventHandler<MapBlankClickedEventArgs> BlankClicked;
        public void SendBlankClicked(Coordinate pos)
        {
            BlankClicked?.Invoke(this, new MapBlankClickedEventArgs(pos));
        }

        public event EventHandler<MapPoiClickedEventArgs> PoiClicked;
        public void SendPoiClicked(Poi poi)
        {
            PoiClicked?.Invoke(this, new MapPoiClickedEventArgs(poi));
        }

        public event EventHandler<MapDoubleClickedEventArgs> DoubleClicked;
        public void SendDoubleClicked(Coordinate pos)
        {
            DoubleClicked?.Invoke(this, new MapDoubleClickedEventArgs(pos));
        }

        public event EventHandler<MapLongClickedEventArgs> LongClicked;
        public void SendLongClicked(Coordinate pos)
        {
            LongClicked?.Invoke(this, new MapLongClickedEventArgs(pos));
        }

        public event EventHandler<EventArgs> Loaded;
        public void SendLoaded()
        {
            Loaded?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public void SendStatusChanged(double latitude, double longitude)
        {
            StatusChanged?.Invoke(this, new StatusChangedEventArgs(latitude, longitude));
        }
    }


    public class StatusChangedEventArgs : EventArgs
    {
        private double latitude;
        private double longitude;
        public StatusChangedEventArgs(double latitude,double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }
        public double Latitude
        {
            get { return latitude; }
        }
        public double Longitude
        {
            get { return longitude; }
        }
    }
}

