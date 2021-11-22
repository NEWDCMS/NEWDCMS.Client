using Wesley.Client.Models;
using Wesley.Client.Models.QA;
using Wesley.Client.Pages;
using Wesley.Client.Services;
using Wesley.Client.Services.QA;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Wesley.Client.ViewModels
{
    public class NotificationsPageViewModel : ViewModelBase
    {
        private readonly ILiteDbService<MessageInfo> _conn;
        private readonly MessageBus _messageBus;
        private readonly IQueuedMessageService _queuedMessageService;
        private readonly static object _lock = new object();

        [Reactive] public ObservableCollection<MessageInfo> Notifs { get; set; } = new ObservableCollection<MessageInfo>();

        private List<QueuedMessage> QueuedMessages { get; set; } = new List<QueuedMessage>();

        public IReactiveCommand AddSubscribe => this.Navigate("AddSubscribePage");
        [Reactive] public MessageInfo Selecter { get; set; }


        public NotificationsPageViewModel(INavigationService navigationService,
            ILiteDbService<MessageInfo> conn,
            IDialogService dialogService,
            IQueuedMessageService queuedMessageService,
            MessageBus messageBus
            ) : base(navigationService, dialogService)
        {
            this._conn = conn;
            this._messageBus = messageBus;
            this._queuedMessageService = queuedMessageService;

            /*
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                try
                {
                    var includes = new[] { 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                    if (!string.IsNullOrEmpty(Settings.SubscribeDatas))
                    {
                        var apps = JsonConvert.DeserializeObject<List<MessageInfo>>(Settings.SubscribeDatas) ?? new List<MessageInfo>();
                        if (apps != null)
                        {
                            apps = apps.Where(s => s.Selected == true).ToList();
                            var messages = await _conn?.Table.FindAsync(s => includes.Contains(s.MTypeId));
                            var lists = new List<MessageInfo>();
                            if (messages != null && messages.Any())
                            {
                                foreach (IGrouping<int, MessageInfo> group in messages.GroupBy(c => (int)c.MType))
                                {
                                    var defaultMsg = group.FirstOrDefault();
                                    var msg = new MessageInfo
                                    {
                                        MType = (MTypeEnum)group.Key,
                                        Title = defaultMsg == null ? "" : defaultMsg.Title,
                                        Content = defaultMsg == null ? "" : defaultMsg.Content,
                                        Count = group.Count(),
                                        Date = defaultMsg == null ? DateTime.Now : defaultMsg.Date
                                    };
                                    lists.Add(msg);
                                }
                            }

                            apps = apps.Where(a => includes.Contains(a.MTypeId)).ToList();
                            apps.ForEach(n =>
                            {
                                var notif = lists.Where(s => s.MType == n.MType).FirstOrDefault();
                                if (notif != null)
                                {
                                    notif.Icon = n.Icon;
                                    n.Title = n.Title;
                                    n.Content = notif.Content;
                                    n.Count = notif.Count;
                                    n.Date = notif.Date;
                                }
                            });

                            var filters = apps?.Where(s => !string.IsNullOrEmpty(s.Title));
                            if (filters != null && filters.Any())
                                this.Notifs = new ObservableCollection<MessageInfo>(filters);
                        }
                    }
                }
                catch (Exception) { }
            }));
            */

            //加载数据
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(() =>
            {
                try
                {
                    bool firstOrDefault = false;

                    int? storeId = Settings.StoreId;
                    int[] mTypeId = new[] { 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                    string toUser = Settings.UserMobile;
                    bool? sentStatus = false;
                    bool? orderByCreatedOnUtc = true;
                    int? maxSendTries = 0;
                    DateTime? startTime = null;
                    DateTime? endTime = null;
                    int pageIndex = 0;
                    int pageSize = 30;

                    if (!this.Notifs.Any())
                        UpdateUI();

                    _queuedMessageService.Rx_GetQueuedMessages(storeId,
                       mTypeId,
                       toUser,
                       sentStatus,
                       orderByCreatedOnUtc,
                       maxSendTries,
                       startTime,
                       endTime,
                       pageIndex,
                       pageSize)
                   .Subscribe((results) =>
                   {
                       System.Diagnostics.Debug.Print("GetQueuedMessages------------------------------------------------>");
                       lock (_lock)
                       {
                          //确保只取缓存数据
                          if (!firstOrDefault)
                           {
                               firstOrDefault = true;

                               if (results != null && results?.Code >= 0)
                               {
                                   var messages = results?.Data;
                                   if (messages != null)
                                   {
                                       this.QueuedMessages = messages.ToList();
                                       UpdateUI();
                                   }
                               }
                           }
                       }
                   }).DisposeWith(DeactivateWith);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }));

            //选择消息
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
              .Skip(1)
              .Where(x => x != null)
              .SubOnMainThread(async x =>
             {
                 await this.NavigateAsync($"{nameof(ViewSubscribePage)}", ("MTypeEnum", x.MType));
             }).DisposeWith(DeactivateWith);

            //接收通知刷新UI
            //this._messageBus
            //.Listener<NotificationEvent>()
            //.Select(_ => Unit.Default)
            //.InvokeCommand((ICommand)this.Load)
            //.DisposeWith(DeactivateWith);

            this.BindBusyCommand(Load);

        }


        private void UpdateUI()
        {
            try
            {
                var includes = new[] { 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                if (!string.IsNullOrEmpty(Settings.SubscribeDatas))
                {
                    var apps = JsonConvert.DeserializeObject<List<MessageInfo>>(Settings.SubscribeDatas) ?? new List<MessageInfo>();
                    if (apps != null)
                    {
                        apps = apps.Where(s => s.Selected == true).ToList();

                        //var messages = await _conn?.Table.FindAsync(s => includes.Contains(s.BillTypeId));

                        var messages = this.QueuedMessages;

                        var lists = new List<MessageInfo>();
                        if (messages != null && messages.Any())
                        {
                            var sts = messages.Select(s => s.ToStructure()).ToList();
                            foreach (IGrouping<int, MessageStructure> group in sts.GroupBy(c => (int)c.MType))
                            {
                                var defaultMsg = group.FirstOrDefault();
                                var msg = new MessageInfo
                                {
                                    MType = (MTypeEnum)group.Key,
                                    Title = defaultMsg == null ? "" : defaultMsg.Title,
                                    Content = defaultMsg == null ? "" : defaultMsg.Content,
                                    Count = group.Where(m => m.IsRead == false).Count(),
                                    Date = defaultMsg == null ? DateTime.Now : defaultMsg.Date
                                };
                                lists.Add(msg);
                            }
                        }

                        apps = apps.Where(a => includes.Contains(a.MTypeId)).ToList();
                        apps.ForEach(n =>
                        {
                            var notif = lists.Where(s => s.MType == n.MType).FirstOrDefault();
                            if (notif != null)
                            {
                                notif.Icon = n.Icon;
                                n.Title = n.Title;
                                n.Content = notif.Content;
                                n.Count = notif.Count;
                                n.Date = notif.Date;
                            }
                        });

                        //Filter
                        var filters = apps?.Where(s => !string.IsNullOrEmpty(s.Title));
                        if (filters != null && filters.Any())
                            this.Notifs = new ObservableCollection<MessageInfo>(filters);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
