using Acr.UserDialogs;
using Akavache;
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
using Wesley.Client.Droid.AutoUpdater;
using Wesley.Client.Droid.Utils;
using Wesley.Client.Pages;
using FFImageLoading;
using FFImageLoading.Forms.Platform;
using ImageCircle.Forms.Plugin.Droid;
using Rg.Plugins.Popup;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Xamarin.Forms;
using Android.Media;
using System.Reactive.Disposables;
using Wesley.Client.BaiduMaps;
using System.Threading.Tasks;
using Platform = Xamarin.Essentials.Platform;
using ZXingPlatform = global::ZXing.Net.Mobile.Forms.Android;
using AndroidX.Annotations;
using Wesley.Client.Services;

namespace Wesley.Client.Droid
{
    [Activity(Label = "@string/ApplicationName",
        Icon = "@mipmap/app",
        Theme = "@style/MainTheme",
        LaunchMode = LaunchMode.SingleTask,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : BaseActivity
    {
        protected Context mContext;
        public static MainActivity Instance { set; get; }
        private PowerManager.WakeLock _wakeLock;


        #region override

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.Window.RequestFeature(WindowFeatures.ActionBar);
            base.OnCreate(bundle);

            mContext = this;
            Instance = this;

            try
            {
                var width = Resources.DisplayMetrics.WidthPixels;
                var height = Resources.DisplayMetrics.HeightPixels;
                var density = Resources.DisplayMetrics.Density; //屏幕密度
                App.ScreenWidth = width / density; //屏幕宽度
                App.ScreenHeight = height / density; //屏幕高度
            }
            catch (System.Exception) { }

            //返回此活动是否为任务的根
            if (!IsTaskRoot)
            {
                Finish();
                return;
            }

            PowerManager pm = (PowerManager)GetSystemService(Context.PowerService);
            isPause = pm.IsScreenOn;
            if (handler == null)
                handler = new Handler();


            //注册广播,屏幕点亮状态监听，用于单独控制音乐播放
            if (screenStateReceiver == null)
            {
                screenStateReceiver = new ScreenStateReceiver();
                var intentFilter = new IntentFilter();
                intentFilter.AddAction("_ACTION_SCREEN_OFF"); //熄屏
                intentFilter.AddAction("_ACTION_SCREEN_ON");//点亮
                intentFilter.AddAction("_ACTION_BACKGROUND");//后台
                intentFilter.AddAction("_ACTION_FOREGROUND");//前台
                RegisterReceiver(screenStateReceiver, intentFilter);
            }


            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                TranslucentStatubar.Immersive(Window);
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window.ClearFlags(WindowManagerFlags.Fullscreen);
            }

            //在andrioid m之后，必须在运行时请求权限
            GetPersimmions();

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
            };
            ImageService.Instance.Initialize(config);

            #endregion 

            Forms.Init(this, bundle);

            //初始 对话框组件
            UserDialogs.Init(this);

            //初始 ZXing
            ZXingPlatform.Platform.Init();

            LoadApplication(new App(new AndroidInitializer()));

        }


        /// <summary>
        /// 获取电源锁
        /// </summary>
        private void AcquireWakeLock()
        {
            if (null == _wakeLock)
            {
                PowerManager pm = (PowerManager)GetSystemService(Context.PowerService);
                _wakeLock = pm.NewWakeLock(WakeLockFlags.Partial | WakeLockFlags.OnAfterRelease, "MyWakelockTag");
                if (_wakeLock != null)
                {

                    this.SendBroadcast(new Intent("_ACTION_SCREEN_ON"));
                    _wakeLock.Acquire();
                }
            }
        }

        /// <summary>
        /// 释放电源锁
        /// </summary>
        private void ReleaseWakeLock()
        {
            if (null != _wakeLock || _wakeLock.IsHeld)
            {
                this.SendBroadcast(new Intent("_ACTION_SCREEN_OFF"));
                _wakeLock.Release();
                _wakeLock = null;
            }
        }


        /// <summary>
        /// 当屏幕获取焦点时
        /// </summary>
        /// <param name="hasFocus"></param>
        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus)
            {
                TranslucentStatubar.Immersive(Window);
            }
        }

        /// <summary>
        /// 在应用程序恢复后被发送到后台时调用
        /// </summary>
        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                //获取电源锁
                AcquireWakeLock();
                TranslucentStatubar.Immersive(Window);

            }
            catch (System.NullReferenceException)
            {
            }
            catch (System.Exception) { }
        }

        protected override void OnStart()
        {
            base.OnStart();
            this.Set();
            this.TryRun();

            //播放无声音乐
            if (mediaPlayer == null)
            {
                mediaPlayer = MediaPlayer.Create(this, Resource.Raw.cactus);
                if (mediaPlayer != null)
                {
                    mediaPlayer.SetVolume(0f, 0f);
                    mediaPlayer.SetOnCompletionListener(new OnCompletionListener());
                    mediaPlayer.SetOnErrorListener(new OnErrorListener());
                    Play();
                }
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnPause()
        {
            base.OnPause();
            //释放电源锁
            ReleaseWakeLock();
        }

        protected override void OnDestroy()
        {
            try
            {
                this.disposer?.Dispose();
                this.disposer = null;

                //卸载广播
                UnregisterReceiver(screenStateReceiver);

                //BlobCache.Shutdown()方法对 Akavache 缓存的完整性至关重要。
                //当您的应用程序关闭时，您必须调用它。而且，一定要等待结果：
                //不这样做可能意味着排队的项目不会刷新到缓存中。
                BlobCache.Shutdown().Wait();
            }
            catch (System.Exception) { }
            finally
            {
                base.OnDestroy();
            }
        }




        private DateTime? lastBackKeyDownTime;
        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Down)
            {
                //这里解决Popup 弹出后，点击回退健时不能释放Popup问题
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (PopupNavigation.Instance.PopupStack.Count > 0)
                        await PopupNavigation.Instance.PopAllAsync();
                });

                if (!lastBackKeyDownTime.HasValue || (DateTime.Now - lastBackKeyDownTime.Value) > new TimeSpan(0, 0, 2))
                {
                    var actionPage = App.Current.MainPage;
                    if (actionPage?.Navigation != null && actionPage?.Navigation?.NavigationStack?.Count != 0)
                    {
                        actionPage = actionPage.Navigation.NavigationStack.Last();
                        if (null != actionPage)
                        {
                            if (actionPage is MainLayoutPage)
                            {
                                ToastUtils.ShowSingleToast("再按一次退出程序");

                                lastBackKeyDownTime = DateTime.Now;
                                return true;
                            }
                            else if (actionPage is LoginPage)
                            {
                                MoveTaskToBack(true);
                                return false;
                            }
                            else
                            {
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    await actionPage?.Navigation.PopAsync(true);
                                });
                            }
                        }
                    }
                }
                else
                {
                    //移除Activity
                    AppManager.Instance().ExitApp();
                }
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }

        /// <summary>
        /// 内存紧张时释放内存
        /// </summary>
        public override void OnTrimMemory(TrimMemory level)
        {

            //RunningModerate = 5, 系统已经进入了低内存的状态，你的进程正在运行但是不会被杀死。

            //RunningLow = 10,

            //RunningCritical = 15,虽然你的进程不会被杀死，但是系统已经开始准备杀死其他的后台进程了，这时候你应该释放无用资源以防止性能下降。下一个阶段就是调用"onLowMemory()"来报告开始杀死后台进程了，特别是状况已经开始影响到用户。

            //TRIM_MEMORY_RUNNING_LOW：虽然你的进程不会被杀死，但是系统已经开始准备杀死其他的后台进程了，你应该释放不必要的资源来提供系统性能，否则会影响用户体验


            //UiHidden = 20,当前进程的界面已经不可见，这时是释放UI相关的资源的好时机。

            //Background = 40,当前进程在LRU列表的头部，虽然你的进程不会被高优杀死，但是系统已经开始准备杀死LRU列表中的其他进程了，
            //因此你应该尽量的释放能够快速回复的资源，以保证当用户返回你的app时可以快速恢复

            //Moderate = 60, 当前进程在LRU列表的中部，如果系统进一步需要内存，你的进程可能会被杀死。

            //Complete = 80 当前进程在LRU列表的尾部，如果没有足够的内存，它将很快被杀死。这时候你应该释放任何不影响app运行的资源。

            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Java.Util.Concurrent.TimeoutException ex)
            {

            }
        }

        /// <summary>
        /// 低内存时强制回收
        /// </summary>
        public override void OnLowMemory()
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnLowMemory();
            }
            catch (Java.Util.Concurrent.TimeoutException ex)
            {

            }
        }

        /// <summary>
        /// 获取intent传递值
        /// </summary>
        /// <param name="intent"></param>
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);
            BitImageEditor.Droid.Platform.OnActivityResult(requestCode, resultCode, intent);
        }

        /// <summary>
        /// 权限请求
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="permissions"></param>
        /// <param name="grantResults"></param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            //Base
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //Essentials
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        #endregion

        /// <summary>
        /// 请求权限
        /// </summary>
        [RequiresApi(Value = 24)]
        private void GetPersimmions()
        {
            //ACCESS_FINE_LOCATION
            var permissions = new List<string>()
            {
               Manifest.Permission.Camera,
               Manifest.Permission.RecordAudio,
               Manifest.Permission.AccessFineLocation,
               Manifest.Permission.WriteExternalStorage,
               Manifest.Permission.AccessFineLocation,
               Manifest.Permission.AccessBackgroundLocation,
               Manifest.Permission.ForegroundService,
               Manifest.Permission.AccessNetworkState,
               Manifest.Permission.Internet,
               Manifest.Permission.AccessCoarseLocation,
               Manifest.Permission.AccessNetworkState,
               Manifest.Permission.ReadExternalStorage,
               Manifest.Permission.WriteSettings,
               Manifest.Permission.AccessWifiState,
               Manifest.Permission.WriteExternalStorage
            };

            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                if (permissions.FirstOrDefault(x => ContextCompat.CheckSelfPermission(this, x) != Permission.Granted) != null)
                    RequestPermissions(permissions.ToArray(), 127);
            }
        }


        public static TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(30);
        CompositeDisposable? disposer;
        void Set()
        {
            this.disposer ??= new CompositeDisposable();
            this.disposer.Add
            (
                Observable
                    .Interval(Interval)
                    .Subscribe(_ => this.TryRun())
            );
        }

        static bool running = false;
        void TryRun()
        {
            try
            {
                if (running)
                    return;

                var loc = App.Resolve<IBaiduLocationService>();
                if (loc != null)
                {
                    if (!loc.IsStarted())
                    {
                        System.Diagnostics.Debug.Print("启动IBaiduLocationService....");

                        loc?.Start();
                        running = false;
                    }
                    else
                    {
                        running = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }



        #region KeeepLive

        private ScreenStateReceiver screenStateReceiver;

        //控制暂停
        private static bool isPause = true;
        private static MediaPlayer mediaPlayer;
        private static Handler handler;

        /// <summary>
        /// 屏幕息屏亮屏与前后台切换广播
        /// </summary>
        protected class ScreenStateReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context? context, Intent intent)
            {
                if (intent.Action.Equals("_ACTION_SCREEN_OFF"))
                {
                    // 熄屏
                    //播放
                    Play();
                }
                else if (intent.Action.Equals("_ACTION_SCREEN_ON"))
                {
                    //亮屏
                    //暂停
                    Pause();

                }
                else if (intent.Action.Equals("_ACTION_BACKGROUND"))
                {
                    //后台
                    //播放
                    Play();
                    //后台回调
                    OnBackground(true);
                }
                else if (intent.Action.Equals("_ACTION_FOREGROUND"))
                {
                    //前台
                    //暂停
                    Pause();
                    //前台回调
                    OnBackground(false);
                }
            }
        }


        public class OnCompletionListener : Java.Lang.Object, MediaPlayer.IOnCompletionListener
        {
            public void OnCompletion(MediaPlayer mediaPlayer)
            {
                if (!isPause)
                {
                    Play();

                    //ROGUE
                    //if (KeepLive.runMode == KeepLive.RunMode.ROGUE)
                    //{
                    //    Play();
                    //}
                    //else
                    //{
                    //    if (handler != null)
                    //    {
                    //        handler.PostDelayed(() =>
                    //        {
                    //            new Thread(() =>
                    //            {
                    //                Play();

                    //            }).Start();

                    //        }, 5000);
                    //    }
                    //}
                }
            }
        }

        public class OnErrorListener : Java.Lang.Object, MediaPlayer.IOnErrorListener
        {
            public bool OnError(MediaPlayer mp, [GeneratedEnum] MediaError what, int extra)
            {
                return false;
            }
        }

        /// <summary>
        /// 播放
        /// </summary>
        private static void Play()
        {
            if (mediaPlayer != null && !mediaPlayer.IsPlaying)
            {
                mediaPlayer.Start();
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        private static void Pause()
        {
            if (mediaPlayer != null && mediaPlayer.IsPlaying)
            {
                mediaPlayer.Pause();
            }
        }

        /// <summary>
        /// 是否是在后台
        /// </summary>
        public static void OnBackground(bool background)
        {
            if (MainApplication.BACKGROUND_CALLBACKS.Any())
            {
                foreach (var it in MainApplication.BACKGROUND_CALLBACKS)
                {
                    it.onBackground(background);
                }
            }
        }

        #endregion
    }



    /// <summary>
    /// 确保Akavache.Sqlite3 dll不会被Xamarin构建工具删除
    /// </summary>
    public static class LinkerPreserve
    {
        static LinkerPreserve()
        {
            _ = typeof(SQLitePersistentBlobCache).FullName;
            _ = typeof(SQLiteEncryptedBlobCache).FullName;

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
}

