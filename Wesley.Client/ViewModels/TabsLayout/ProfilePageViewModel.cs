using Wesley.Client.Models.Report;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wesley.Client.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        private readonly IReportingService _reportingService;

        [Reactive] public string ProfileName { get; set; }
        [Reactive] public string UserMobile { get; set; }
        [Reactive] public string UserFace { get; set; } = "profile_placeholder.png";
        [Reactive] public DashboardReport Analysis { get; set; } = new DashboardReport();

        [Reactive] public IList<CutMenusGroup> Menus { get; set; } = new ObservableCollection<CutMenusGroup>();
        [Reactive] public CutMenus Selecter { get; set; }

        public ProfilePageViewModel(INavigationService navigationService,
            IReportingService reportingService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "个人资料";

            _reportingService = reportingService;

            var menus = (new List<CutMenus>()
            {
                new CutMenus{ Type=0,  Name="系统设置",  Icon = "&#xf013;", Url="SystemSettingPage" ,ShowSeparator= true},
                new CutMenus{ Type=0,  Name="个人信息",  Icon = "&#xf007;", Url="MyInfoPage" ,ShowSeparator= true},
                new CutMenus{ Type=0, Name="账号安全",  Icon = "&#xf2bb;", Url="SecurityPage",ShowSeparator= true},
                new CutMenus{ Type=0,  Name="打印设置",  Icon = "&#xf02f;", Url="PrintSettingPage",ShowSeparator= true},
                new CutMenus{ Type=0,  Name="关于我们", Icon = "&#xf2dc;", Url="AboutPage",ShowSeparator= true},
                new CutMenus{ Type=0,  Name="版本更新",  Icon = "&#xf019;", Url="UpdatePage",ShowSeparator= false},
                new CutMenus{ Type=1, Name="技术支持", Icon = "&#xf086;", Url="ConversationsPage",ShowSeparator= true},
                new CutMenus{ Type=1,  Name="问题反馈",  Icon = "&#xf059;", Url="FeedbackPage",ShowSeparator= false}
            }).GroupBy(s => s.Type).Select(s => { return new CutMenusGroup(s.Key, s.ToList()); }).ToList();
            this.Menus = new ObservableCollection<CutMenusGroup>(menus);

            //菜单选择
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
                .Skip(1)
                .Where(x => x != null)
                .SubOnMainThread(async item =>
               {
                   await this.NavigateAsync(item.Url, null);
                   this.Selecter = null;
               });

            //加载数据
            this.Load = ReactiveCommand.Create(() =>
            {
                try
                {
                    this.ProfileName = Settings.UserRealName;
                    this.UserMobile = $"{Settings.StoreName} <b>{Settings.UserMobile}.</b>";
                    if (!string.IsNullOrEmpty(Settings.FaceImage) && Settings.FaceImage.StartsWith("http"))
                    {
                        this.UserFace = Settings.FaceImage;
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                Sync.Run(async () =>
                {
                    var result = await _reportingService.GetDashboardReportAsync(this.ForceRefresh, cts.Token);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (result != null)
                            Analysis = result;
                    });
                }, (ex) => { Crashes.TrackError(ex); });
            });

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }

        public class CutMenus
        {
            public int Type { get; set; }
            public string Name { get; set; }
            public string Describe { get; set; }
            public string Icon { get; set; }
            public string Url { get; set; }
            public bool ShowSeparator { get; set; }
        }
        public class CutMenusGroup : List<CutMenus>
        {
            public int Type { get; set; }
            public CutMenusGroup(int type, List<CutMenus> menus) : base(menus)
            {
                Type = type;
            }
        }
    }
}
