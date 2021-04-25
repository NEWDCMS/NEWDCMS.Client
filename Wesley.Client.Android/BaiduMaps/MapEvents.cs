using Android.Widget;
using AndroidX.CardView.Widget;
using Com.Baidu.Mapapi.Map;
using Wesley.Client.BaiduMaps;
using Java.Lang;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;

namespace Wesley.Client.Droid
{
    public partial class MapRenderer
    {
        void OnMapClick(object sender, BaiduMap.MapClickEventArgs e)
        {
            Map.SendBlankClicked(e.P0.ToUnity());
        }

        void OnMapPoiClick(object sender, BaiduMap.MapPoiClickEventArgs e)
        {
            Map.SendPoiClicked(new Poi
            {
                Coordinate = e.P0.Position.ToUnity(),
                Description = e.P0.Name
            });
        }

        void OnMapDoubleClick(object sender, BaiduMap.MapDoubleClickEventArgs e)
        {
            Map.SendDoubleClicked(e.P0.ToUnity());
        }

        void OnMapLongClick(object sender, BaiduMap.MapLongClickEventArgs e)
        {
            Map.SendLongClicked(e.P0.ToUnity());
        }


        /// <summary>
        /// 标注点击时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMarkerClick(object sender, BaiduMap.MarkerClickEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(e.P0.Title))
                {
                    var pin = Map.Pins.Find(e.P0);
                    if (pin?.Terminal != null)
                    {
                        var color = pin?.Terminal?.RankColor ?? "#4a89dc";

                        //cardView
                        var cardView = new CardView(Context);
                        cardView.Radius = 15;
                        cardView.SetCardBackgroundColor(Color.FromHex(color).ToAndroid());

                        //布局
                        var layout = new LinearLayout(Context);
                        layout.Orientation = Orientation.Vertical;
                        layout.SetPadding(10, 10, 10, 10);

                        //Title
                        var tview = new TextView(Context);
                        tview.SetPadding(10, 10, 10, 10);
                        tview.SetTextColor(Color.White.ToAndroid());
                        tview.TextSize = 12;
                        tview.Text = e.P0.Title;

                        //Button
                        var param = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                        param.Height = 45;
                        var button = new Android.Widget.Button(Context);
                        button.Text = "去拜访";
                        button.TextSize = 10;
                        button.LayoutParameters = param;
                        button.SetBackgroundResource(Resource.Drawable.roundShape);
                        button.SetTextColor(Color.White.ToAndroid());
                        button.Click += (sender, ar) =>
                        {
                            var pin = Map.Pins.Find(e.P0);
                            pin?.SendClicked(pin?.Terminal);
                        };

                        layout.AddView(tview);
                        layout.AddView(button);

                        cardView.AddView(layout);

                        var window = new InfoWindow(cardView, e.P0.Position, -e.P0.Icon.Bitmap.Height);
                        NativeMap.Map.ShowInfoWindow(window);
                    }
                }
            }
            catch (Exception )
            {
            }
        }

        void OnMarkerDragStart(object sender, BaiduMap.MarkerDragStartEventArgs e)
        {
            try
            {
                NativeMap.Map.HideInfoWindow();
                Map.Pins.Find(e.P0)?.SendDrag(AnnotationDragState.Starting);
            }
            catch (Exception )
            {
            }
        }

        void OnMarkerDrag(object sender, BaiduMap.MarkerDragEventArgs e)
        {
            try
            {
                Pin pin = Map.Pins.Find(e.P0);
                if (null != pin)
                {
                    pinImpl.NotifyUpdate(pin);
                    pin.SendDrag(AnnotationDragState.Dragging);
                }
            }
            catch (Exception )
            {
            }
        }

        void OnMarkerDragEnd(object sender, BaiduMap.MarkerDragEndEventArgs e)
        {
            try
            {
                Pin pin = Map.Pins.Find(e.P0);
                if (null != pin)
                {
                    pinImpl.NotifyUpdate(pin);
                    pin.SendDrag(AnnotationDragState.Ending);
                }
            }
            catch (Exception )
            {
            }
        }

        void MapStatusChangeFinish(object sender, BaiduMap.MapStatusChangeFinishEventArgs e)
        {
            try
            {
                Map.SetValueSilent(Map.CenterProperty, e.P0.Target.ToUnity());
                Map.SetValueSilent(Map.ZoomLevelProperty, e.P0.Zoom);
                Map.SendStatusChanged();
            }
            catch (Exception )
            {
            }
        }

        public void OnMapLoaded()
        {
            try
            {
                Map.Projection = new ProjectionImpl(NativeMap);
                NativeMap.OnResume();
                Map.SendLoaded();
                ///ZoomToSpan();
            }
            catch (Exception )
            {
            }
        }


        //void OnOverlayClear(object sender, BaiduMap.MapClickEventArgs e)
        //{
        //    Map.SendOverlayCleared();
        //}
    }
}

