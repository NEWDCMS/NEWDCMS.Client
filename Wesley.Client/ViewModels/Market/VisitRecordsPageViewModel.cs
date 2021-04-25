using Wesley.Client.Models;
using Wesley.Client.Models.Census;
using Wesley.Client.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;


namespace Wesley.Client.ViewModels
{
    public class VisitRecordsPageViewModel : ViewModelBase
    {
        private readonly ITerminalService _terminalService;

        [Reactive] public int? TotalOnTime { get; set; }
        [Reactive] public decimal? TotalAmount { get; set; }

        [Reactive] public ObservableCollection<VisitStoreGroup> VisitStores { get; set; } = new ObservableCollection<VisitStoreGroup>();


        [Reactive] public VisitStore Selecter { get; set; }

        public VisitRecordsPageViewModel(INavigationService navigationService,
            ITerminalService terminalService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "业务员拜访记录";

            _terminalService = terminalService;

            this.Load = VisitStoresLoader.Load(async () =>
            {
                int? terminalId = Filter?.TerminalId ?? 0;
                int? districtId = Filter?.DistrictId ?? 0;
                int? businessUserId = Filter?.BusinessUserId ?? 0;
                DateTime start = Filter.StartTime ?? DateTime.Now;
                DateTime end = Filter.EndTime ?? DateTime.Now;


                var visitStoreGroups = new List<VisitStoreGroup>();
                var result = await _terminalService.GetVisitStoresAsync(terminalId, districtId, businessUserId, start, end, this.ForceRefresh, calToken: cts.Token);
                if (result != null)
                {
                    var pending = result.ToList();
                    pending.ForEach(v =>
                    {
                        v.DoorheadPhoto = v.DoorheadPhotos.FirstOrDefault()?.StoragePath;
                        v.OnStoreStopSeconds = v.SignOutDateTime.Subtract(v.SigninDateTime).Minutes;
                        v.SaleAmount ??= 0;
                        v.FaceImage = string.IsNullOrEmpty(v.FaceImage) ? "profile_placeholder.png" : v.FaceImage;
                    });

                    this.TotalOnTime = pending.Select(v => v.SignOutDateTime.Subtract(v.SigninDateTime).Minutes).Sum();
                    this.TotalAmount = pending.Select(v => v.SaleAmount).Sum();

                    foreach (var group in pending.GroupBy(s => s.BusinessUserId))
                    {
                        var firs = group.FirstOrDefault();
                        visitStoreGroups.Add(new VisitStoreGroup(firs?.BusinessUserName, firs.FaceImage, group.ToList()));
                    }

                    this.VisitStores = new ObservableCollection<VisitStoreGroup>(visitStoreGroups);
                }

                return visitStoreGroups;
            });

            //预览照片
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
           .Skip(1)
           .Where(x => x != null)
           .SubOnMainThread(async item =>
           {
               if (item?.DisplayPhotos?.Any() ?? false)
               {
                   var images = item.DisplayPhotos.ToList();
                   await this.NavigateAsync("ImageViewerPage", ("ImageInfos", images));
               }
               Selecter = null;
           });

            //菜单选择
            this.SetMenus((x) =>
            {
                this.HitFilterDate(x, () => { ((ICommand)Load)?.Execute(null); });
            }, 8, 9, 10, 13, 14);

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
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

            ((ICommand)Load)?.Execute(null);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
