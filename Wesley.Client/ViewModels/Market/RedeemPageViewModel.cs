using Acr.UserDialogs;
using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Sales;
using Wesley.Client.Pages;
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

namespace Wesley.Client.ViewModels
{
    public class RedeemPageViewModel : ViewModelBase
    {
        public ViewModelLoader<IReadOnlyCollection<RedeemModel>> RedeemItemsLoader { get; set; } = new ViewModelLoader<IReadOnlyCollection<RedeemModel>>(ApplicationExceptions.ToString, Resources.TextResources.EmptyText);


        [Reactive] public ObservableCollection<RedeemModel> Redeems { get; set; } = new ObservableCollection<RedeemModel>();

        public RedeemPageViewModel(INavigationService navigationService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "兑奖";

            //Load = RedeemItemsLoader.Load(() =>
            //{
            //    ItemTreshold = 1;
            //    //TODO load data return  Redeems
            //    Redeems.Add(new RedeemModel() {Name="1元换购"});
            //    Redeems.Add(new RedeemModel() { Name = "1元换购" });
            //    Redeems.Add(new RedeemModel() { Name = "1元换购" });
            //    Redeems.Add(new RedeemModel() { Name = "1元换购" });
            //    Redeems.Add(new RedeemModel() { Name = "1元换购" });
            //    Redeems.Add(new RedeemModel() { Name = "1元换购" });
            //    Redeems.Add(new RedeemModel() { Name = "1元换购" });
            //    Redeems.Add(new RedeemModel() { Name = "1元换购" });
            //    return Redeems;
            //});


            this.ItemTresholdReachedCommand = ReactiveCommand.Create(() =>
            {
                int pageIdex = 0;
                if (Redeems?.Count != 0)
                    pageIdex = Redeems.Count / (PageSize == 0 ? 1 : PageSize);

                if (PageCounter < pageIdex)
                {
                    PageCounter = pageIdex;
                    using (var dig = UserDialogs.Instance.Loading("加载中..."))
                    {
                        try
                        {
                            Redeems.Add(new RedeemModel() { Name = "2元换购" });
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                            ItemTreshold = -1;
                        }
                    }
                }
            }, this.WhenAny(x => x.Redeems, x => x.GetValue().Count > 0));




            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object>(async e =>
            {
                var redeems = Redeems.Select(p => p).Where(p => p.Selected == true).ToList();
                if (redeems.Count == 0)
                {
                    this.Alert("请选择兑奖项目");
                    return;
                }
                var ok = await _dialogService.ShowConfirmAsync("是否兑奖项目吗？", "", "确定", "取消");
                if (!ok)
                {
                    return;
                }
                //TODO 请求服务

            });


            this.BindBusyCommand(Load);

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

            // ((ICommand)Load)?.Execute(null);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            _popupMenu?.Show(8, 9, 10, 13, 14);


        }
    }
}
