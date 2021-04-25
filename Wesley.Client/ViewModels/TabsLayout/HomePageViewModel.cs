using Acr.UserDialogs;
using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Pages;
using Wesley.Client.Pages.Market;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{
    /// <summary>
    /// 首页
    /// </summary>
    public class HomePageViewModel : ViewModelBaseCutom
    {
        private readonly INewsService _newsService;
        private readonly IReportingService _reportingService;
        private readonly LocalDatabase _conn;

        [Reactive] public IList<Module> TopAppSeries { get; set; } = new ObservableCollection<Module>();
        [Reactive] public string UserFace { get; set; } = "profile_placeholder.png";
        [Reactive] public IList<NewsInfoModel> NewsSeries { get; set; } = new ObservableCollection<NewsInfoModel>();

        public IReactiveCommand AddAPPCommand => this.Navigate("AddAppPage");
        public ReactiveCommand<string, Unit> CmdSelectedCommand { get; }
        public ReactiveCommand<NewsInfoModel, Unit> NewsSelectedCommand { get; }
        public ReactiveCommand<Module, Unit> InvokeAppCommand { get; }

        public new IReactiveCommand RefreshCommand { get; set; }
        public IReactiveCommand TopMessageLoad { get; set; }
        public IReactiveCommand DashboardReportLoad { get; set; }
        public IReactiveCommand PermissionLoad { get; set; }

        [Reactive] public int TodayOrderQuantity { get; set; }
        [Reactive] public int TodayVisitQuantity { get; set; }
        [Reactive] public decimal TodaySaleAmount { get; set; }
        [Reactive] public int TodayAddTerminalQuantity { get; set; }

        public HomePageViewModel(INavigationService navigationService,
            IProductService productService,
            IReportingService reportingService,
            IUserService userService,
            ITerminalService terminalService,
            IWareHousesService wareHousesService,
            INewsService newsService,
            LocalDatabase conn,
            IAccountingService accountingService,
            IDialogService dialogService) : base(navigationService,
                productService,
                terminalService,
                userService,
                wareHousesService,
                accountingService,
                dialogService)
        {
            Title = "首页";

            _newsService = newsService;
            _reportingService = reportingService;
            _conn = conn;

            this.RefreshCommand = ReactiveCommand.Create<object>(e =>
            {
                //强制刷新
                this.ForceRefresh = true;
                ((ICommand)Load)?.Execute(null);
                ((ICommand)DashboardReportLoad)?.Execute(null);
                ((ICommand)PermissionLoad)?.Execute(null);
            });

            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(() =>
           {
               var pending = new List<NewsInfoModel>();
               for (var i = 0; i <= 3; i++)
               {
                   pending.Add(new NewsInfoModel() { PicturePath = $"b{i + 1}.png" });
               }
               if (pending != null && pending.Any())
                   this.NewsSeries = new ObservableCollection<NewsInfoModel>(pending);

               try
               {
                   //头像
                   if (!string.IsNullOrEmpty(Settings.FaceImage) && Settings.FaceImage.StartsWith("http"))
                       UserFace = Settings.FaceImage;
                   this.StoreName = string.IsNullOrEmpty(Settings.StoreName) ? "" : Settings.StoreName;
                   this.UserName = string.IsNullOrEmpty(Settings.UserRealName) ? "" : Settings.UserRealName;

                   //快捷方式
                   var apps = GlobalSettings.AppDatas;
                   if (apps.Any() && apps.Count >= 11)
                   {
                       apps = apps.Where(s => s.Selected).Take(11).ToList();
                       foreach (var a in apps)
                       {
                           if (!string.IsNullOrEmpty(a.Name))
                               a.Name = a.Name.Length > 4 ? CommonHelper.CutString(a.Name, 0, 4) : a.Name;
                       }
                       apps.Add(new Module()
                       {
                           Id = 1,
                           Navigation = "AddAppPage",
                           Name = "更多",
                           Icon = "&#xf009;",
                           Color = "#BCBCBC",
                           Selected = true
                       });
                   }
                   this.TopAppSeries = new ObservableCollection<Module>(apps);
               }
               catch (Exception ex)
               {
                   Crashes.TrackError(ex);
               }
           }));

            //统计
            this.DashboardReportLoad = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                var pending = await _reportingService.GetDashboardReportAsync(this.ForceRefresh, cts.Token);
                if (pending != null)
                {
                    this.TodayOrderQuantity = pending?.TodayOrderQuantity ?? 0;
                    this.TodayVisitQuantity = pending?.TodayVisitQuantity ?? 0;
                    this.TodaySaleAmount = pending?.TodaySaleAmount ?? 0;
                    this.TodayAddTerminalQuantity = pending?.TodayAddTerminalQuantity ?? 0;
                }
            }));

            //刷新权限
            this.PermissionLoad = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                await App.Resolve<IGlobalService>()?.SynchronizationPermission(cts.Token);
            }));

            //页面选择事件
            this.CmdSelectedCommand = ReactiveCommand.CreateFromTask<string>((r) => Task.Run(async () =>
            {
                if (!IsFastClick())
                    return;

                if (r == "VisitStorePage")
                {
                    using (UserDialogs.Instance.Loading("加载中..."))
                    {
                        //获取业务员最近一次未签退情况
                        var csg = await this.CheckSignIn();
                        if (!csg)
                        {
                            //转向当前客户选择（创建）
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await this.NavigateAsync($"{nameof(CurrentCustomerPage)}");
                            });
                            return;
                        }
                    }
                }
                else if (r == "MyInfoPage")
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await this.NavigateAsync($"{nameof(MyInfoPage)}");
                    });
                    return;
                }

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await this.NavigateAsync(r);
                });
            }));

            //活动内容选择
            this.NewsSelectedCommand = ReactiveCommand.Create<NewsInfoModel>((r) => Task.Run(() =>
            {
                if (!IsFastClick())
                    return;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await this.NavigateAsync($"{nameof(NewsViewerPage)}", ("newsId", r.Id));
                });
            }));

            //调用APP应用
            this.InvokeAppCommand = ReactiveCommand.CreateFromTask<Module>((r) => Task.Run(async () =>
            {
                return await this.Access(r, AccessStateEnum.View, async () =>
                {
                    if (r.Navigation == "VisitStorePage")
                    {
                        var csg = await this.CheckSignIn();
                        if (!csg)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await this.NavigateAsync($"{nameof(CurrentCustomerPage)}");
                            });
                            return;
                        }
                    }
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await this.NavigateAsync(r.Navigation);
                    });
                });
            }));

            this.InvokeAppCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.CmdSelectedCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.AddAPPCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.NewsSelectedCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.DashboardReportLoad.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.PermissionLoad.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.ExceptionsSubscribe();
            this.BindBusyCommand(Load);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            if (!loaded)
            {
                loaded = true;
                ((ICommand)Load)?.Execute(null);
                ((ICommand)DashboardReportLoad)?.Execute(null);
                ((ICommand)PermissionLoad)?.Execute(null);
            }
        }

    }
}

