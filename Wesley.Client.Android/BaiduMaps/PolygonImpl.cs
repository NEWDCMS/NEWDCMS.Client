using Com.Baidu.Mapapi.Map;
using Com.Baidu.Mapapi.Model;
using Wesley.Client.BaiduMaps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Xamarin.Forms.Platform.Android;
using BMap = Com.Baidu.Mapapi.Map;

namespace Wesley.Client.Droid
{
    /// <summary>
    /// 轨迹回放
    /// </summary>
    internal class PolygonImpl : BaseItemImpl<BaiduMaps.Polygon, BMap.MapView, BMap.Polygon>
    {
        protected override IList<BaiduMaps.Polygon> GetItems(Map map) => map.Polygons;

        protected override BMap.Polygon CreateNativeItem(BaiduMaps.Polygon item)
        {
            List<LatLng> points = new List<LatLng>();
            foreach (var point in item.Points)
            {
                points.Add(point.ToNative());
            }

            PolygonOptions options = new PolygonOptions()
                .InvokePoints(points)
                .InvokeStroke(new Stroke(item.Width, item.Color.ToAndroid()))
                .InvokeFillColor(item.FillColor.ToAndroid());

            BMap.Polygon polygon = (BMap.Polygon)NativeMap.Map.AddOverlay(options);
            item.NativeObject = polygon;

            ((INotifyCollectionChanged)(IList)item.Points).CollectionChanged += (sender, e) =>
            {
                OnItemPropertyChanged(item, new PropertyChangedEventArgs(BaiduMaps.Polygon.PointsProperty.PropertyName));
            };

            return polygon;
        }

        protected override void UpdateNativeItem(BaiduMaps.Polygon item)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveNativeItem(BaiduMaps.Polygon item)
        {
            ((BMap.Polygon)item.NativeObject).Remove();
        }

        protected override void RemoveNativeItems(IList<BaiduMaps.Polygon> items)
        {
            foreach (BaiduMaps.Polygon item in items)
            {
                ((BMap.Polygon)item.NativeObject).Remove();
            }
        }

        public override void OnMapPropertyChanged(PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            BaiduMaps.Polygon item = (BaiduMaps.Polygon)sender;
            BMap.Polygon native = (BMap.Polygon)item?.NativeObject;
            if (null == native)
            {
                return;
            }

            if (BaiduMaps.Polygon.TitleProperty.PropertyName == e.PropertyName)
            {
                return;
            }

            if (BaiduMaps.Polygon.PointsProperty.PropertyName == e.PropertyName)
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

