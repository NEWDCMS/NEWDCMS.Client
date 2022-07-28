using DCMS.Client.Models.Census;
using DCMS.Client.Services;

using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DCMS.Client.ViewModels
{
    public class FieldTrackPageViewModel : ViewModelBase
    {

        private readonly ITerminalService _terminalService;
        [Reactive] public DateTime TrackDate { get; internal set; }
        [Reactive] public BusinessVisitList BusinessVisit { get; internal set; } = new BusinessVisitList();
        public IReactiveCommand BusinessSelected { get; set; }
        private bool IsRefresh { get; set; } = false;

        public FieldTrackPageViewModel(INavigationService navigationService,
            ITerminalService terminalService,
              IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "外勤轨迹";
            _navigationService = navigationService;
            _dialogService = dialogService;
            _terminalService = terminalService;

            this.GoBackAsync = ReactiveCommand.CreateFromTask<object>(async e =>
            {
                await _navigationService.TryNavigateBackAsync();
            });


            var canExecute = this.WhenAnyValue(
               x => x.BusinessUsers.Count,
               (a) => { return a == 0; }).ObserveOn(RxApp.MainThreadScheduler);


            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                var result = await _terminalService.GetAllUserVisitedListAsync(TrackDate, cts);
                if (result != null)
                {
                    Refresh(result.Take(2).ToList());
                }
                IsRefresh = true;
            }), canExecute);

            this.BusinessSelected = ReactiveCommand.Create<object>(e => this.NavigateAsync("SelectUserPage", ("SelectDateTime", TrackDate)));

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }



        private DelegateCommand<string> _changeDateCmd;
        public DelegateCommand<string> ChangeDateCmd
        {
            get
            {
                if (_changeDateCmd == null)
                {
                    _changeDateCmd = new DelegateCommand<string>((e) =>
                    {
                        try
                        {
                            if (e.ToString() == "0")
                            {
                                TrackDate = TrackDate.AddDays(-1);
                            }

                            if (e.ToString() == "1")
                            {
                                TrackDate = TrackDate.AddDays(1);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Write(ex);
                        }
                    });
                }
                return _changeDateCmd;
            }
        }


        /// <summary>
        /// 选择轨迹
        /// </summary>
        private DelegateCommand<object> _userSelected;
        public DelegateCommand<object> UserSelected
        {
            get
            {
                if (_userSelected == null)
                {
                    _userSelected = new DelegateCommand<object>((r) =>
                    {
                        try
                        {
                            BusinessUsers.ToList().ForEach(b =>
                           {
                               if (r is BusinessVisitList user && b.BusinessUserId == user.BusinessUserId)
                               {
                                   b.Selected = true;
                                   b.BgColor = "#8cabe1";
                                   b.TxtColor = "#FFFFFF";
                               }
                               else
                               {
                                   b.Selected = false;
                                   b.BgColor = "#FFFFFF";
                                   b.TxtColor = "#333333";
                               }
                           });
                        }
                        catch (Exception ex)
                        {
                            Log.Write(ex);
                        }
                    });
                }
                return _userSelected;
            }
        }


        public void Refresh(List<BusinessVisitList> lists)
        {
            int i = 0;
            lists.ForEach(u =>
            {
                if (i == 0)
                {
                    u.Selected = true;
                    u.BgColor = "#8cabe1";
                    u.TxtColor = "#FFFFFF";
                }
                u.ColumnIndex = i++;
            });

            BusinessUsers = new ObservableRangeCollection<BusinessVisitList>(lists);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                if (parameters.ContainsKey("BusinessVisitUser"))
                {
                    parameters.TryGetValue<BusinessVisitList>("BusinessVisitUser", out BusinessVisitList selecter);

                    var lists = BusinessUsers.ToList() ?? new List<BusinessVisitList>();
                    if (lists.Count > 1)
                    {
                        lists.RemoveAt(0);
                        lists.Add(selecter);
                    }
                    else
                    {
                        lists.Add(selecter);
                    }

                    Refresh(lists);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }



        public override void OnAppearing()
        {
            base.OnAppearing();
            TrackDate = DateTime.Now;

            if (!IsRefresh)
                ((ICommand)Load)?.Execute(null);
        }
    }
}
