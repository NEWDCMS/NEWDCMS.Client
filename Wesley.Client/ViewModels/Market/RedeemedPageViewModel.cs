using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Pages;
using Wesley.Client.Services;
using Prism.Navigation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class RedeemedPageViewModel : ViewModelBase
    {
        //[Reactive] public ObservableCollection<VisitStoreGroup> VisitStores { get; set; } = new ObservableCollection<VisitStoreGroup>();
        //[Reactive] public VisitStore Selecter { get; set; }

        public RedeemedPageViewModel(INavigationService navigationService,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "已兑奖";

            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(() =>
           {
                //TODO return list 调用接口返回对付数据
                //return null;
            }));

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

             ((ICommand)Load)?.Execute(null);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            _popupMenu?.Show(8, 9, 10, 13, 14);


        }
    }
}
