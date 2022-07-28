using DCMS.Client.BaiduMaps;
using DCMS.Client.ViewModels;
using Prism.Ioc;
using Shiny.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace DCMS.Client.Pages.Archive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FieldTrackPage : BaseContentPage<FieldTrackPageViewModel>
    {
        private bool IsPlayed { get; set; }
        public List<Coordinate> Coords { get; set; } = new List<Coordinate>(); //实时轨迹
        public List<Coordinate> CPoints { get; set; } = new List<Coordinate>(); //拜访轨迹
        private ICalculateUtils calc;


        public FieldTrackPage()
        {
            try
            {
                InitializeComponent();
                NavigationPage.SetHasNavigationBar(this, false);



                var mapManager = App.ContainerProvider.Resolve<IMapManager>();
                var offlineMap = App.ContainerProvider.Resolve<IOfflineMap>();
                calc = App.ContainerProvider.Resolve<ICalculateUtils>();

                //IMapManager mapManager = DependencyService.Get<IMapManager>();
                Debug.WriteLine($"坐标类型：{mapManager.CoordinateType}");
                mapManager.CoordinateType = CoordType.GCJ02;
                Debug.WriteLine($"坐标类型：{mapManager.CoordinateType}");

                map.Loaded += MapLoaded;

                //离线地图
                //IOfflineMap offlineMap = DependencyService.Get<IOfflineMap>();
                //offlineMap.HasUpdate += (_, e) => 
                //{
                //    Debug.WriteLine("离线地图有更新: " + e.CityID);
                //};
                //offlineMap.Downloading += (_, e) => 
                //{
                //    Debug.WriteLine("离线地图下载: " + e.CityID);
                //};
                //var list = offlineMap.HotList;
                //list = offlineMap.AllList;
                ////offlineMap.Remove(131);
                //var curr = offlineMap.Current;
                ////offlineMap.Start(27);
                ////offlineMap.Start(75);
                //curr = offlineMap.Current;

                // 计算
                //ICalculateUtils calc = DependencyService.Get<ICalculateUtils>();

                //var distance = calc.CalculateDistance(new Coordinate(33.355379, 108.854323), new Coordinate(34.355379, 108.954323));
                //Debug.WriteLine($"计算距离为：{distance}");//139599.429229778 in iOS, 139689.085961837 in Android

            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public void MapLoaded(object sender, EventArgs x)
        {
            map.ShowScaleBar = true;
            map.ZoomLevel = 15;

            //初始位置服务
            InitLocationService();
            //初始事件
            InitEvents();

            map.Center = new Coordinate(34.366391, 109.006694);

            //// 坐标转换
            //IProjection proj = map.Projection;
            //var coord = proj.ToCoordinate(new Point(100, 100));
            //Debug.WriteLine($"坐标转换:{proj.ToScreen(coord)}");
        }



        private static bool moved = false;
        public void InitLocationService()
        {
            try
            {
                //map.LocationService.LocationUpdated += (_, e) =>
                //{
                //    Debug.WriteLine("地图 更新 LocationUpdated: " + e.Coordinate);
                //    if (!moved)
                //    {
                //        //1536
                //        map.Center = e.Coordinate;
                //        moved = true;
                //    }
                //};

                map.LocationService.Start();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public void InitEvents()
        {

            btnTrack.Clicked += (_, e) =>
            {
                if (ViewModel != null)
                {
                    if (ViewModel.BusinessUsers.Count == 0)
                    {
                        ((ICommand)ViewModel.BusinessSelected)?.Execute(null);
                        return;
                    }
                }
                //回放
                Projection();
            };

            //长按事件
            map.LongClicked += (_, e) =>
            {
                AddPin(e.Coordinate);
            };

            map.StatusChanged += (_, e) =>
            {
                //Debug.WriteLine(map.Center + " @" + map.ZoomLevel);
            };
        }

        /// <summary>
        /// 播放轨迹
        /// </summary>
        private void Projection()
        {
            if (!IsPlayed)
            {
                IsPlayed = true;
                btnTrack.Text = "&#xf04d;";
                //============================
                //生成坐标点
                map.ClearOverlay = true;

                Task.Run(() =>
                {
                    try
                    {
                        if (ViewModel != null)
                        {
                            var select = ViewModel.BusinessUsers.Where(s => s.Selected).FirstOrDefault();
                            if (select != null)
                            {
                                //跟踪轨迹，包含拜访轨迹
                                Coords = select.RealTimeTracks.Select(b =>
                               {
                                   return new Coordinate(b.Latitude ?? 0, b.Longitude ?? 0, int.Parse(b.Ctype));
                               }).ToList();

                                //拜访路线
                                CPoints = select.VisitRecords.Select(b =>
                                {
                                    return new Coordinate(b.Latitude ?? 0, b.Longitude ?? 0, 1);
                                }).ToList();


                                if (Coords.Count >= 2)
                                {
                                    //轨迹细化
                                    var lastPoints = new Coordinate();
                                    for (int j = 0; j < Coords.Count; j++)
                                    {
                                        lastPoints = Coords[j];
                                        if (j > 0)
                                        {
                                            var dist = calc.CalculateDistance(Coords[j], lastPoints);
                                            if (dist > 3) //两点之间的距离大于3米
                                            {
                                                Coords.AddRange(addLatLng(Coords[j], lastPoints, dist));
                                            }
                                        }
                                    }

                                    //var dist = calc.CalculateDistance(Coords[0], Coords[Coords.Count-1]);
                                    //GetLevel(calc.CalculateDistance(Coords[0], Coords[Coords.Count - 1])); //缩放地图

                                    Coords = DouglasPeucker(Coords, 10); //轨迹抽稀
                                    Coords = OptimizePoints(Coords); //轨迹优化;


                                    //画线1
                                    map.Polylines.Add(new BaiduMaps.Polyline
                                    {
                                        Points = new ObservableCollection<Coordinate>(Coords.Take(2)),
                                        //Color = Color.FromHex("#8cabe1"),
                                        Color = Color.FromHex("#53a245"),
                                        Width = 15
                                    });

                                    for (int i = 0; i < Coords.Count; i++)
                                    {
                                        Task.Delay(1000).Wait();

                                        lastPoints = Coords[i];

                                        if (i == 0)
                                        {
                                            AddOverlay(Coords[i], "q10660.png");//起点
                                        }
                                        else if (i == Coords.Count - 1)
                                        {
                                            AddOverlay(Coords[i], "z10660.png");//终点
                                        }
                                        else if (Coords[i].Ctype == 1)
                                        {
                                            //当前标注
                                            AddOverlay(Coords[i], "red_location.png");
                                        }

                                        map.Polylines[0].Points.Add(Coords[i]);
                                        //移动地图
                                        map.Center = Coords[i];
                                    }

                                    //轨迹2
                                    //map.Polylines.Add(new BaiduMaps.Polyline
                                    //{
                                    //    Points = new ObservableCollection<Coordinate>(CPoints.Take(2)),
                                    //    Color = Color.FromHex("#53a245"),
                                    //    Width = 10
                                    //});

                                    //for (int i = 0; i < CPoints.Count; i++)
                                    //{
                                    //    Task.Delay(1000).Wait();

                                    //    if (i == 0)
                                    //    {
                                    //        AddOverlay(CPoints[i], "q10660.png");//起点
                                    //    }
                                    //    else if (i == CPoints.Count - 1)
                                    //    {
                                    //        AddOverlay(CPoints[i], "z10660.png");//终点
                                    //    }
                                    //    else
                                    //    {
                                    //        //当前标注
                                    //        AddOverlay(CPoints[i], "red_location.png");
                                    //    }

                                    //    map.Polylines[1].Points.Add(CPoints[i]);
                                    //    //移动地图
                                    //    map.Center = CPoints[i];
                                    //}
                                }
                                else
                                {
                                    ViewModel.Alert($"回放失败，业务员{select.BusinessUserName},无拜访轨迹.");
                                }
                            }
                            else
                            {
                                ViewModel.Alert("请选择业务员！");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex);
                    }
                });

                //============================
                btnTrack.Text = "&#xf04b;";
                IsPlayed = false;
            }
        }



        /// <summary>
        /// 添加覆盖物
        /// </summary>
        /// <param name="coord">坐标</param>
        /// <param name="image">图像</param>
        void AddOverlay(Coordinate coord, string image)
        {
            var annotation = new Pin
            {
                Coordinate = coord,
                Animate = true,
                Draggable = true,
                Enabled3D = true,
                Image = XImage.FromResource(image)
            };
            map.Pins.Add(annotation);
        }


        //添加覆盖物并连线
        void AddPin(Coordinate coord)
        {
            Pin annotation = new Pin
            {
                Title = coord,
                Coordinate = coord,
                Animate = true,
                Draggable = true,
                Enabled3D = true,
                Image = XImage.FromResource("pin_purple.png")
            };
            map.Pins.Add(annotation);

            annotation.Drag += (o, e) =>
            {
                Pin self = o as Pin;
                self.Title = "区域1";//self.Coordinate;
                int i = map.Pins.IndexOf(self);

                if (map.Polylines.Count > 0 && i > -1)
                {
                    map.Polylines[0].Points[i] = self.Coordinate;
                }
            };

            annotation.Clicked += (_, e) =>
            {
                //Debug.WriteLine("clicked");
                ((Pin)_).Image = XImage.FromResource("q10660.png");
                // XImage.FromStream(typeof(FieldTrackPage).GetTypeInfo().Assembly.GetManifestResourceStream("Sample.Images.10660.png"));
            };

            if (map.Polylines.Count == 0 && map.Pins.Count > 1)
            {
                BaiduMaps.Polyline polyline = new BaiduMaps.Polyline
                {
                    Points = new ObservableCollection<Coordinate> {
                        map.Pins[0].Coordinate, map.Pins[1].Coordinate
                    },
                    Width = 4,
                    Color = Color.Purple
                };

                map.Polylines.Add(polyline);
            }
            else if (map.Polylines.Count > 0)
            {
                map.Polylines[0].Points.Add(annotation.Coordinate);
            }
        }

        #region 五点平滑轨迹优化算法
        /// <summary>
        /// 五点平滑算法
        /// </summary>
        /// <param name="inPoint"></param>
        /// <returns></returns>
        public List<Coordinate> OptimizePoints(List<Coordinate> inPoint)
        {
            int size = inPoint.Count;
            List<Coordinate> outPoint = new List<Coordinate>();

            int i;
            if (size < 5)
            {
                return inPoint;
            }
            else
            {
                outPoint.Add(new Coordinate((3.0 * inPoint[0].Latitude + 2.0
                                * inPoint[1].Latitude + inPoint[2].Latitude - inPoint[4].Latitude) / 5.0, (3.0 * inPoint[0].Longitude + 2.0
                                * inPoint[1].Longitude + inPoint[2].Longitude - inPoint[4].Longitude) / 5.0, inPoint[0].Ctype));

                outPoint.Add(new Coordinate((4.0 * inPoint[0].Latitude + 3.0
                                * inPoint[1].Latitude + 2
                                * inPoint[2].Latitude + inPoint[3].Latitude) / 10.0, (4.0 * inPoint[0].Longitude + 3.0
                                * inPoint[1].Longitude + 2
                                * inPoint[2].Longitude + inPoint[3].Longitude) / 10.0, inPoint[1].Ctype));

                for (i = 2; i <= size - 3; i++)
                {
                    outPoint.Add(new Coordinate((inPoint[i - 2].Latitude + inPoint[i - 1].Latitude + inPoint[i].Latitude + inPoint[i + 1].Latitude + inPoint[i + 2].Latitude) / 5.0, (inPoint[i - 2].Longitude + inPoint[i - 1].Longitude + inPoint[i].Longitude + inPoint[i + 1].Longitude + inPoint[i + 2].Longitude) / 5.0, inPoint[i].Ctype));
                }

                outPoint.Add(new Coordinate((4.0 * inPoint[size - 1].Latitude + 3.0
                                * inPoint[size - 2].Latitude + 2
                                * inPoint[size - 3].Latitude + inPoint[
                                size - 4].Latitude) / 10.0, (4.0 * inPoint[size - 1].Longitude + 3.0
                                * inPoint[size - 2].Longitude + 2
                                * inPoint[size - 3].Longitude + inPoint[
                                size - 4].Longitude) / 10.0, inPoint[size - 2].Ctype));

                outPoint.Add(new Coordinate((3.0 * inPoint[size - 1].Latitude + 2.0
                                * inPoint[size - 2].Latitude
                                + inPoint[size - 3].Latitude - inPoint[
                                size - 5].Latitude) / 5.0, (3.0 * inPoint[size - 1].Longitude + 2.0
                                * inPoint[size - 2].Longitude
                                + inPoint[size - 3].Longitude - inPoint[
                                size - 5].Longitude) / 5.0, inPoint[size - 1].Ctype));
            }
            return outPoint;
        }
        #endregion

        #region 道格拉斯-普克轨迹抽稀算法
        private List<Coordinate> DouglasPeucker(List<Coordinate> points, int epsilon)
        {
            double maxH = 0;
            int index = 0;
            int end = points.Count;
            for (int i = 1; i < end - 1; i++)
            {
                double h = H(points[0], points[i], points[end - 1]);
                if (h > maxH)
                {
                    maxH = h;
                    index = i;
                }
            }
            List<Coordinate> result = new List<Coordinate>();
            if (maxH > epsilon)
            {
                List<Coordinate> leftPoints = new List<Coordinate>();//左曲线
                List<Coordinate> rightPoints = new List<Coordinate>();//右曲线 

                //分别保存左曲线和右曲线的坐标点
                for (int i = 0; i < end; i++)
                {
                    if (i <= index)
                    {
                        leftPoints.Add(points[i]);
                        if (i == index)
                        {
                            rightPoints.Add(points[i]);
                        }
                    }
                    else
                    {
                        rightPoints.Add(points[i]);
                    }
                }
                List<Coordinate> leftResult = new List<Coordinate>();
                List<Coordinate> rightResult = new List<Coordinate>();
                leftResult = DouglasPeucker(leftPoints, epsilon);
                rightResult = DouglasPeucker(rightPoints, epsilon);

                rightResult.RemoveAll(it => leftResult.Contains(it));//移除重复的点
                leftResult.AddRange(rightResult);
                result = leftResult;
            }
            else
            {
                result.Add(points[0]);
                result.Add(points[end - 1]);
            }
            return result;
        }

        private double H(Coordinate A, Coordinate B, Coordinate C)
        {
            double c = calc.CalculateDistance(A, B);

            double b = calc.CalculateDistance(A, C);

            double a = calc.CalculateDistance(C, B);

            double S = helen(a, b, c);

            double H = 2 * S / c;

            return H;
        }

        private double helen(double a, double b, double c)
        {
            double p = (a + b + c) / 2;
            double S = Math.Sqrt(p * (p - a) * (p - b) * (p - c));
            return S;
        }
        #endregion

        #region 轨迹点细化处理
        private List<Coordinate> addLatLng(Coordinate local1, Coordinate local2, double distance)
        {
            double a_x = local1.Latitude;
            double a_y = local1.Longitude;
            double b_x = local2.Latitude;
            double b_y = local2.Longitude;

            double partX = Math.Abs(a_x - b_x) / distance;
            double partY = Math.Abs(a_y - b_y) / distance;
            List<Coordinate> list = new List<Coordinate>();
            for (int i = 0; i < distance; i++)
            {
                //每隔1米切割一个点
                if (i % 3 == 0)
                {
                    double x;
                    if (a_x < b_x) x = a_x + partX * i;
                    else if (a_x > b_x) x = a_x - partX * i;
                    else x = a_x;
                    double y;
                    if (a_y < b_y) y = a_y + partY * i;
                    else if (a_y > b_y) y = a_y - partY * i;
                    else y = a_y;
                    list.Add(new Coordinate(x, y));
                }
            }
            //list.add(getLatLng(local2));
            return list;
        }

        /// <summary>
        /// 移动到指定位置，并缩放
        /// </summary>
        /// <param name="latlng"></param>
        /// <param name="flag"></param>
        //private void moveToLocation(Coordinate latlng)
        //{
        //    map.Center = latlng;
        //}

        /**
       *根据距离判断地图级别
       */
        private void GetLevel(double distance)
        {
            int[] zoom = { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 1000, 2000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 };

            for (int i = 0; i < zoom.Length; i++)
            {
                int zoomNow = zoom[i];
                if (zoomNow - distance * 1000 > 0)
                {
                    var level = 18 - i + 6;
                    map.ZoomLevel = level;
                    break;
                }
            }
        }
        #endregion


        protected override void OnAppearing()
        {
            base.OnAppearing();
            //ViewModel = (FieldTrackPageViewModel)BindingContext;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            map.LocationService.Stop();
        }

    }
}
