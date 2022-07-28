using Acr.UserDialogs;
using DCMS.Client.Enums;
using DCMS.Client.Models;
using DCMS.Client.Models.Census;
using DCMS.Client.Pages;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using DCMS.Infrastructure.Helpers;
using System.Reactive.Linq;
using System.Windows.Input;

namespace DCMS.Client.ViewModels
{
    public class VisitRecordsPageViewModel : ViewModelBase
    {
        private readonly ITerminalService _terminalService;

        [Reactive] public int? TotalOnTime { get; set; }
        [Reactive] public decimal? TotalAmount { get; set; }
        [Reactive] public ObservableCollection<VisitStoreGroup> VisitStores { get; set; } = new ObservableCollection<VisitStoreGroup>();
        //[Reactive] public VisitStore Selecter { get; set; }
        [Reactive] public bool DataVewEnable { get; set; } = true;
        [Reactive] public bool NullViewEnable { get; set; }
        public ReactiveCommand<string, Unit> PhotoCommand { get; set; }

        public VisitRecordsPageViewModel(INavigationService navigationService,
            ITerminalService terminalService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "业务员拜访记录";

            _terminalService = terminalService;

            this.Load = ReactiveCommand.Create(async () =>
            {
                try
                {
                    //重载时排它
                    ItemTreshold = 1;
                    Terminals?.Clear();

                    DataVewEnable = false;
                    NullViewEnable = true;

                    int? terminalId = Filter?.TerminalId ?? 0;
                    int? districtId = Filter?.DistrictId ?? 0;

                    int? businessUserId = Settings.UserId;
                    if (Filter?.BusinessUserId > 0)
                    {
                        businessUserId = Filter?.BusinessUserId;
                    }

                    DateTime start = Filter.StartTime ?? DateTime.Now.AddDays(-30);
                    DateTime end = Filter.EndTime ?? DateTime.Now;

                    //清除列表
                    VisitStores?.Clear();

                    var items = await _terminalService.GetVisitStoresAsync(terminalId,
                        districtId,
                        businessUserId,
                        start,
                        end,
                        0,
                        PageSize,
                        this.ForceRefresh,
                        new System.Threading.CancellationToken());

                    if (items != null)
                    {
                        var pending = items.ToList();
                        pending.ForEach(v =>
                        {
                            v.DoorheadPhoto = v.DoorheadPhotos.FirstOrDefault()?.StoragePath;

                            var tts = v.SignOutDateTime.Subtract(v.SigninDateTime).TotalSeconds;
                            v.OnStoreStopSeconds = (int)tts;
                            v.OnStoreStopSecondsFMT = CommonHelper.ConvetToSeconds((int)tts);
                            v.SaleAmount ??= 0;
                            v.FaceImage = string.IsNullOrEmpty(v.FaceImage) ? "profile_placeholder.png" : v.FaceImage;
                        });

                        this.TotalOnTime = pending.Select(v => v.SignOutDateTime.Subtract(v.SigninDateTime).Minutes).Sum();
                        this.TotalAmount = pending.Select(v => v.SaleAmount).Sum();

                        foreach (var group in pending.GroupBy(s => s.BusinessUserId))
                        {
                            var firs = group.FirstOrDefault();
                            VisitStores.Add(new VisitStoreGroup(firs?.BusinessUserName, firs.FaceImage, group.ToList()));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                finally
                {
                    NullViewEnable = false;
                    DataVewEnable = true;
                }

            });

            //以增量方式加载数据
            this.ItemTresholdReachedCommand = ReactiveCommand.Create(async () =>
            {
                if (ItemTreshold == -1) return;
                try
                {
                    int pageIdex = Terminals.Count / (PageSize == 0 ? 1 : PageSize);
                    if (pageIdex > 0)
                    {
                        using (var dig = UserDialogs.Instance.Loading("加载中..."))
                        {
                            try
                            {
                                int? terminalId = Filter?.TerminalId ?? 0;
                                int? districtId = Filter?.DistrictId ?? 0;
                                int? businessUserId = Filter?.BusinessUserId ?? 0;
                                DateTime start = Filter.StartTime ?? DateTime.Now;
                                DateTime end = Filter.EndTime ?? DateTime.Now;

                                //清除列表
                                VisitStores?.Clear();

                                var items = await _terminalService.GetVisitStoresAsync(terminalId,
                                    districtId,
                                    businessUserId,
                                    start,
                                    end,
                                    pageIdex,
                                    PageSize,
                                    this.ForceRefresh,
                                    new System.Threading.CancellationToken());

                                if (items != null)
                                {
                                    var pending = items.ToList();
                                    pending.ForEach(v =>
                                    {
                                        var tts = v.SignOutDateTime.Subtract(v.SigninDateTime).TotalSeconds;
                                        v.DoorheadPhoto = v.DoorheadPhotos.FirstOrDefault()?.StoragePath;
                                        v.OnStoreStopSeconds = (int)tts;
                                        v.OnStoreStopSecondsFMT = CommonHelper.ConvetToSeconds((int)tts);
                                        v.SaleAmount ??= 0;
                                        v.FaceImage = string.IsNullOrEmpty(v.FaceImage) ? "profile_placeholder.png" : v.FaceImage;
                                    });

                                    this.TotalOnTime = pending.Select(v => v.SignOutDateTime.Subtract(v.SigninDateTime).Minutes).Sum();
                                    this.TotalAmount = pending.Select(v => v.SaleAmount).Sum();

                                    foreach (var group in pending.GroupBy(s => s.BusinessUserId))
                                    {
                                        var firs = group.FirstOrDefault();
                                        VisitStores.Add(new VisitStoreGroup(firs?.BusinessUserName, firs.FaceImage, group.ToList()));
                                    }
                                }

                                if (items.Count() == 0 || items.Count() == VisitStores.Count)
                                {
                                    ItemTreshold = -1;
                                }
                            }
                            catch (Exception ex)
                            {
                                Crashes.TrackError(ex);
                                ItemTreshold = -1;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    ItemTreshold = -1;
                }
            });


            //预览照片
            this.PhotoCommand = ReactiveCommand.Create<string>(async item =>
            {
                if (item != null)
                {
                    var images = new List<string>() { item };
                    await this.NavigateAsync("ImageViewerPage", ("ImageInfos", images));
                }
            });

            //绑定页面菜单
            _popupMenu = new PopupMenu(this, new Dictionary<MenuEnum, Action<SubMenu, ViewModelBase>>
            {
                //TODAY
                { MenuEnum.TODAY, (m,vm) => {
                    Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                        Filter.EndTime = DateTime.Now;
                    ((ICommand)Load)?.Execute(null);
                } },
                //YESTDAY
                { MenuEnum.YESTDAY, (m,vm) => {
                     Filter.StartTime = DateTime.Now.AddDays(-1);
                        Filter.EndTime = DateTime.Now;
                    ((ICommand)Load)?.Execute(null);
                } },
                //MONTH
                { MenuEnum.MONTH, (m,vm) => {
                      Filter.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01 00:00:00"));
                        Filter.EndTime = DateTime.Now;
                    ((ICommand)Load)?.Execute(null);
                } },
                //THISWEEBK
                { MenuEnum.THISWEEBK, (m,vm) => {
                      Filter.StartTime = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                        Filter.EndTime = DateTime.Now;
                    ((ICommand)Load)?.Execute(null);
                } },
                //OTHER
                { MenuEnum.OTHER, (m,vm) => {
                     SelectDateRang(true);
                } }
            });

            this.BindBusyCommand(Load);

        }



        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            //过滤器
            if (parameters.ContainsKey("Filter"))
            {
                parameters.TryGetValue("Filter", out FilterModel filter);

                if (filter != null)
                    this.Filter = filter;
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            _popupMenu?.Show(8, 9, 10, 13, 14);

            if (!VisitStores.Any())
                ((ICommand)Load)?.Execute(null);
        }
    }
}
