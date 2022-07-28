using Com.Baidu.Mapapi.Map;
using Com.Baidu.Mapapi.Model;
using DCMS.Client.BaiduMaps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Xamarin.Forms.Platform.Android;
using BMap = Com.Baidu.Mapapi.Map;


namespace DCMS.Client.Droid
{
    /// <summary>
    /// 画线
    /// </summary>
    internal class PolylineImpl : BaseItemImpl<BaiduMaps.Polyline, BMap.MapView, BMap.Polyline>
    {
        protected override IList<BaiduMaps.Polyline> GetItems(Map map) => map.Polylines;

        protected override BMap.Polyline CreateNativeItem(BaiduMaps.Polyline item)
        {
            List<LatLng> points = new List<LatLng>();
            foreach (var point in item.Points)
            {
                points.Add(point.ToNative());
            }

            if (points.Count >= 2)
            {
                PolylineOptions options = new PolylineOptions()
                .InvokePoints(points)
                .InvokeWidth(item.Width)
                .InvokeColor(item.Color.ToAndroid());

                BMap.Polyline polyline = (BMap.Polyline)NativeMap.Map.AddOverlay(options);
                item.NativeObject = polyline;

                /*
                OverlayOptions ooPolyline = new PolylineOptions().width(12)
                .color(0xAAFF0000).points(points);
		
                mMarkerPolyLine = (Polyline) mBaiduMap.addOverlay(ooPolyline);	
		 
                OverlayOptions ooA = new MarkerOptions().position(p2).icon(bdA);
                mMarkerA = (Marker) (mBaiduMap.addOverlay(ooA));
                 */

                ((INotifyCollectionChanged)(IList)item.Points).CollectionChanged += (sender, e) =>
                {
                    OnItemPropertyChanged(item, new PropertyChangedEventArgs(BaiduMaps.Polyline.PointsProperty.PropertyName));
                };
                return polyline;
            }
            else
                return null;
        }

        protected override void UpdateNativeItem(BaiduMaps.Polyline item)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveNativeItem(BaiduMaps.Polyline item)
        {
            ((BMap.Polyline)item.NativeObject).Remove();
        }

        protected override void RemoveNativeItems(IList<BaiduMaps.Polyline> items)
        {
            foreach (BaiduMaps.Polyline item in items)
            {
                ((BMap.Polyline)item.NativeObject).Remove();
            }
        }

        public override void OnMapPropertyChanged(PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            BaiduMaps.Polyline item = (BaiduMaps.Polyline)sender;
            BMap.Polyline native = (BMap.Polyline)item?.NativeObject;


            if (null == native)
            {
                return;
            }

            if (BaiduMaps.Polyline.TitleProperty.PropertyName == e.PropertyName)
            {
                return;
            }

            if (BaiduMaps.Polyline.PointsProperty.PropertyName == e.PropertyName)
            {
                List<LatLng> points = new List<LatLng>();
                foreach (Coordinate point in item.Points)
                {
                    points.Add(point.ToNative());
                }

                native.Points = points;
            }
        }
    }
}

