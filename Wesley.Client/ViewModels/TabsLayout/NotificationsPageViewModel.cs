using Wesley.Client.Models;
using Wesley.Client.Pages;
using Wesley.Client.Services;

using Newtonsoft.Json;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;

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

    public class NotificationsPageViewModel : ViewModelBase
    {
        private readonly LocalDatabase _conn;
        [Reactive] public ObservableCollection<MessageInfo> Notifs { get; set; } = new ObservableCollection<MessageInfo>();
        public IReactiveCommand AddSubscribe => this.Navigate("AddSubscribePage");
        [Reactive] public MessageInfo Selecter { get; set; }


        public NotificationsPageViewModel(INavigationService navigationService,
            LocalDatabase conn,

            IDialogService dialogService) : base(navigationService, dialogService)
        {
            _conn = conn;

            Title = "通知";

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
                            var messages = await _conn?.GetAllMessageInfos(includes);
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

                            var filters = apps.Where(s => !string.IsNullOrEmpty(s.Title));
                            this.Notifs = new ObservableCollection<MessageInfo>(filters);
                        }
                    }
                }
                catch (Exception) { }
            }));

            //选择消息
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
              .Skip(1)
              .Where(x => x != null)
              .SubOnMainThread(async x =>
             {
                 await this.NavigateAsync($"{nameof(ViewSubscribePage)}", ("MTypeEnum", x.MType));
             });


            this.AddSubscribe.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);

            //接收通知刷新UI
            subUpdateUI = MessageBus.Current.Listen<NotificationEvent>(Constants.UPDATEUI_KEY)
            .Select(_ => Unit.Default)
            .InvokeCommand((ICommand)this.Load);

            //预览跳转
            subMessage = MessageBus.Current.Listen<MessageInfo>(Constants.NOTIFI_PUSHERRECEIVED_KEY)
            .Select(x => x)
            .Subscribe((msg) => Device.BeginInvokeOnMainThread(async () => await RedirectAsync(msg)));
        }
        public override void OnActiveTabChangedAsync()
        {
            base.OnActiveTabChangedAsync();
            if (Notifs?.Count == 0)
                ((ICommand)Load)?.Execute(null);
        }
    }
}
