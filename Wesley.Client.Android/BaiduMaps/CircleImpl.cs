using Com.Baidu.Mapapi.Map;
using Wesley.Client.BaiduMaps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms.Platform.Android;
using BMap = Com.Baidu.Mapapi.Map;

namespace Wesley.Client.Droid
{
    /// <summary>
    /// 范围标圆
    /// </summary>
    internal class CircleImpl : BaseItemImpl<BaiduMaps.Circle, BMap.MapView, BMap.Circle>
    {
        protected override IList<BaiduMaps.Circle> GetItems(Map map) => map.Circles;

        protected override BMap.Circle CreateNativeItem(BaiduMaps.Circle item)
        {
            CircleOptions options = new CircleOptions()
                .InvokeCenter(item.Coordinate.ToNative())
                .InvokeRadius((int)item.Radius)
                .InvokeStroke(new Stroke(item.Width, item.Color.ToAndroid()))
                .InvokeFillColor(item.FillColor.ToAndroid());

            BMap.Circle circle = (BMap.Circle)NativeMap.Map.AddOverlay(options);
            item.NativeObject = circle;

            return circle;
        }

        protected override void UpdateNativeItem(BaiduMaps.Circle item)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveNativeItem(BaiduMaps.Circle item)
        {
            ((BMap.Circle)item.NativeObject).Remove();
        }

        protected override void RemoveNativeItems(IList<BaiduMaps.Circle> items)
        {
            foreach (BaiduMaps.Circle item in items)
            {
                ((BMap.Circle)item.NativeObject).Remove();
            }
        }

        public override void OnMapPropertyChanged(PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            BaiduMaps.Circle item = (BaiduMaps.Circle)sender;
            BMap.Circle native = (BMap.Circle)item?.NativeObject;
            if (null == native)
            {
                return;
            }

            if (BaiduMaps.Circle.CoordinateProperty.PropertyName == e.PropertyName)
            {
                native.Center = item.Coordinate.ToNative();
                return;
            }

            if (BaiduMaps.Circle.RadiusProperty.PropertyName == e.PropertyName)
            {
                native.Radius = (int)item.Radius;
                return;
            }
        }


    }
}

