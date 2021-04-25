using Wesley.Client.Models;
using Wesley.Client.Pages;
using Wesley.Client.Services;
using Microsoft.AppCenter.Crashes;
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
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class MessagesPageViewModel : ViewModelBase
    {
        private readonly IGlobalService _globalService;
        private readonly LocalDatabase _conn;


        [Reactive] public ObservableCollection<MessageInfo> MessageSeries { get; set; } = new ObservableCollection<MessageInfo>();
        [Reactive] public MessageInfo Selecter { get; set; }

        public IReactiveCommand AddSubscribe => this.Navigate("AddSubscribePage");
        //public IReactiveCommand SendNow { get; set; }

        public MessagesPageViewModel(INavigationService navigationService,
            LocalDatabase conn,
            IDialogService dialogService,
            IGlobalService globalService) : base(navigationService, dialogService)
        {
            _globalService = globalService;
            _conn = conn;


            Title = "消息";

            //加载数据
            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    var includes = new[] { 0, 1, 2, 3 };
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
                            var filters = apps.Where(s => !string.IsNullOrEmpty(s.Title));
                            this.MessageSeries = new ObservableCollection<MessageInfo>(filters);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            });

            //this.SendNow = ReactiveCommand.Create(() =>
            //{
            //    var mq = MQConsumer.Instance;
            //    mq.Payload = JsonConvert.SerializeObject(new MessageInfo() { BillId = 34343 });
            //    mq?.BuildAndSend("标题", "测试内容！", "Message", null);
            //});

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
            subMessage = MessageBus.Current.Listen<MessageInfo>(Constants.MESSAGE_PUSHERRECEIVED_KEY)
            .Select(x => x)
            .Subscribe(async (msg) =>
            {
                await RedirectAsync(msg);
            });
        }

        public override void OnActiveTabChangedAsync()
        {
            base.OnActiveTabChangedAsync();
            if (MessageSeries?.Count == 0)
                ((ICommand)Load)?.Execute(null);
        }
    }

}
