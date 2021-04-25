using Wesley.Client.AutoUpdater.Exceptions;
using Wesley.Client.AutoUpdater.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace Wesley.Client.AutoUpdater
{
    /// <summary>
    /// 更新管理器
    /// </summary>
    public class UpdateManager
    {

        /// <summary>
        /// 检查并更新
        /// </summary>
        private readonly Func<Task<UpdatesCheckResponse>> checkForUpdatesFunction;
        private readonly TimeSpan? runEvery;

        private bool didCheck;
        private readonly Page mainPage;
        private readonly UpdateMode mode;


#if DEBUG
        public static string AppIDDummy
        {
            get
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    return "com.dcms.client";
                }

                if (Device.RuntimePlatform == Device.UWP)
                {
                    return "9ncbcszsjrsb";
                }

                if (Device.RuntimePlatform == Device.iOS)
                {
                    return "id324684580";
                }

                return String.Empty;
            }
        }
#endif

        private static UpdateManager instance;


        /// <summary>
        ///  初始更新管理器
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="mode"></param>
        public static void Initialize(UpdateManagerParameters parameters, UpdateMode mode)
        {
            if (instance != null)
            {
                throw new AutoUpdateException("更新管理器已经初始化.");
            }

            instance = new UpdateManager(parameters, mode);
        }

        private UpdateManager(Func<Task<UpdatesCheckResponse>> checkForUpdatesFunction, TimeSpan? runEvery = null)
        {
            this.runEvery = runEvery;
            this.checkForUpdatesFunction = checkForUpdatesFunction ?? throw new AutoUpdateException("检查未提供的更新功能，必须在构造函数中传递它.");

            try
            {
                //主页呈现时弹出提示
                mainPage = Application.Current.MainPage;
                mainPage.Appearing += OnMainPageAppearing;
            }
            catch (Exception)
            {

            }
        }

        private UpdateManager(UpdateManagerParameters parameters, UpdateMode mode) : this(parameters.CheckForUpdatesFunction, parameters.RunEvery)
        {
            if (mode == UpdateMode.MissingNo)
            {
                throw new AutoUpdateException("不支持次选择模式.");
            }

            this.mode = mode;
        }


        /// <summary>
        /// 呈现主页时检查更新版本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnMainPageAppearing(object sender, EventArgs e)
        {
            if (!didCheck)
            {
                didCheck = true;

                bool run = true;
                if (runEvery.HasValue && Application.Current.Properties.TryGetValue("UpdateManager.LastUpdateTime", out object lastUpdate))
                {
                    DateTime lastUpdateTime = (DateTime)lastUpdate;
                    if (lastUpdateTime + runEvery.Value < DateTime.Now)
                    {
                        run = false;
                    }
                }

                if (run)
                {
                    await CheckForUpdatesAsync();
                }
            }
        }

        private async Task CheckForUpdatesAsync()
        {
            if (mode == UpdateMode.AutoInstall)
            {
                await CheckAndUpdateAsync();
            }

            Application.Current.Properties["UpdateManager.LastUpdateTime"] = DateTime.Now;
        }

        /// <summary>
        /// 异步检查并更新
        /// </summary>
        /// <returns></returns>
        private async Task CheckAndUpdateAsync()
        {
            UpdatesCheckResponse response = await checkForUpdatesFunction();
            //&& await mainPage.DisplayAlert(title, message, confirm, cancel)
            if (response.IsNewVersionAvailable)
            {
                if (Device.RuntimePlatform == Device.UWP || Device.RuntimePlatform == Device.Android)
                {
                    var _operatingSystemVersionProvider = App.Resolve<IOperatingSystemVersionProvider>();
                    _operatingSystemVersionProvider.CheckUpdate(response.UpdateInfo);
                }
                else
                {
                    throw new AutoUpdateException("自动安装只支持android和uwp.");
                }
            }
        }

        //private async Task CheckAndOpenAppStoreAsync()
        //{
        //    UpdatesCheckResponse response = await checkForUpdatesFunction();
        //    if (response.IsNewVersionAvailable && await mainPage.DisplayAlert(title, message, confirm, cancel))
        //    {
        //        DependencyService.Get<IStoreOpener>().OpenStore();
        //    }
        //}

    }
}
