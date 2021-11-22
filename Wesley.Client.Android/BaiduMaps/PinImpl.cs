using Com.Baidu.Mapapi.Map;
using Wesley.Client.BaiduMaps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using BMap = Com.Baidu.Mapapi.Map;

namespace Wesley.Client.Droid
{

    /// <summary>
    /// 标注覆盖物实现
    /// </summary>
    internal class PinImpl : BaseItemImpl<Pin, BMap.MapView, Marker>
    {
        protected override IList<Pin> GetItems(Map map) => map.Pins;

        protected override Marker CreateNativeItem(Pin item)
        {
            //var options = new MarkerOptions().InvokePosition(item.Coordinate.ToNative()).InvokeTitle(item.Title);

            MarkerOptions options = new MarkerOptions().InvokePosition(item.Coordinate.ToNative());

            if (item.Animate)
            {
                options.InvokeAnimateType(MarkerOptions.MarkerAnimateType.Grow);
            }

            options.Draggable(item.Draggable);
            options.Flat(!item.Enabled3D);

            BitmapDescriptor bitmap = BitmapDescriptorFactory.FromResource(Resource.Drawable.water_drop);
            //BitmapDescriptor bitmap = item.Image?.ToNative();
            if (null == bitmap)
            {
                throw new Exception("必须提供一个图标");
            }
            //var nbitmap = ScaleBitmap(bitmap.Bitmap, 0.5f);
            //TextOptions textOptions = new TextOptions();
            //textOptions.InvokeText(item.Title);
            //textOptions.InvokePosition(item.Coordinate.ToNative());

            options.InvokeIcon(bitmap);

            //Marker marker = (Marker)NativeMap.Map.AddOverlay(textOptions);
            Marker marker = (Marker)NativeMap.Map.AddOverlay(options);
            marker.Scale = 0.5f;
            marker.Title = item.Title;
            item.NativeObject = marker;

            return marker;
        }

        //private Bitmap ScaleBitmap(Bitmap origin, float ratio)
        //{
        //    if (origin == null)
        //    {
        //        return null;
        //    }
        //    int width = origin.Width;
        //    int height = origin.Height;
        //    Matrix matrix = new Matrix();
        //    matrix.PreScale(ratio, ratio);
        //    Bitmap newBM = Bitmap.CreateBitmap(origin, 0, 0, width, height, matrix, false);
        //    if (newBM.Equals(origin))
        //    {
        //        return newBM;
        //    }
        //    origin.Recycle();
        //    return newBM;
        //}

        protected override void UpdateNativeItem(Pin item)
        {
            Marker native = (Marker)item?.NativeObject;
            if (null == native)
            {
                return;
            }
            item.SetValueSilent(Pin.CoordinateProperty, native.Position.ToUnity());
        }

        protected override void RemoveNativeItem(Pin item)
        {
            NativeMap.Map.HideInfoWindow();
            ((Marker)item.NativeObject).Icon.Recycle();
            ((Marker)item.NativeObject).Remove();
        }

        protected override void RemoveNativeItems(IList<Pin> items)
        {
            foreach (Pin item in items)
            {
                RemoveNativeItem(item);
            }
        }

        public override void OnMapPropertyChanged(PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Pin item = (Pin)sender;
            Marker native = (Marker)item?.NativeObject;
            if (null == native)
            {
                return;
            }

            if (Pin.CoordinateProperty.PropertyName == e.PropertyName)
            {
                native.Position = item.Coordinate.ToNative();
                return;
            }

            if (Pin.TitleProperty.PropertyName == e.PropertyName)
            {
                native.Title = item.Title;
                return;
            }

            if (Pin.ImageProperty.PropertyName == e.PropertyName)
            {
                native.Icon = item.Image?.ToNative();
                return;
            }
        }
    }
}

