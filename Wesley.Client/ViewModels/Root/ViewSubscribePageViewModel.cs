using Acr.UserDialogs;
using Wesley.Client.Models;
using Wesley.Client.Resources;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;

using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
namespace Wesley.Client.ViewModels
{
    public class ViewSubscribePageViewModel : ViewModelBase
    {
        private readonly LocalDatabase _conn;

        [Reactive] public ObservableCollection<MessageItemsGroup> Notifs { get; set; } = new ObservableCollection<MessageItemsGroup>();
        [Reactive] public MessageInfo Selecter { get; set; }

        public MTypeEnum MType { get; set; }
        public ReactiveCommand<MessageInfo, Unit> RemoveCommand { get; set; }
        public ICommand RemoveAllCommand { get; set; }


        public ViewSubscribePageViewModel(INavigationService navigationService,
              LocalDatabase conn,
              IDialogService dialogService) : base(navigationService, dialogService)
        {
            _conn = conn;

            Title = "待办提示";


            //载入消息
            this.Load = MessageLoader.Load(async () =>
            {
                this.Title = MType == MTypeEnum.All ? "" : CommonHelper.GetEnumDescription(this.MType);

                var messageItems = new List<MessageInfo>();
                //取待办
                if (MType == MTypeEnum.Message || MType == MTypeEnum.Receipt || MType == MTypeEnum.Hold)
                {
                    var messages = await _conn?.GetAllMessageInfos(new[] { 0, 1, 2, 3 });
                    if (messages != null)
                    {
                        messageItems = messages.Where(s => s.MType == MType).ToList();
                    }
                }
                //取通知
                else
                {
                    var messages = await _conn?.GetAllMessageInfos(new[] { 4, 5, 6, 7, 8, 9, 10, 11, 12 });
                    if (messages != null)
                    {
                        messageItems = messages.Where(s => s.MType == MType).ToList();
                    }
                }

                //分组
                var messagesGroups = messageItems.GroupBy(m => m.Date.Day).Select(grp =>
                {
                    var messagesGrp = grp.ToList().OrderBy(m => m.Date);

                    var msg = messagesGrp.First();
                    var date = msg.Date.Date;
                    var dayDiff = DateTime.Now.Day - date.Day;
                    string groupHeader = string.Empty;

                    //今天
                    if (dayDiff == 0)
                        groupHeader = TextResources.Today;
                    //昨天
                    else if (dayDiff == 1)
                        groupHeader = TextResources.Yesterday;
                    //更早以前
                    else if (dayDiff >= 7 && dayDiff < 15)
                        groupHeader = TextResources.BeforeThat;
                    //年月日
                    else
                        groupHeader = date.ToString("yyyy/MM/dd");

                    return new MessageItemsGroup
                    (
                        dateTime: date,
                        groupHeader: groupHeader,
                        messages: new ObservableCollection<MessageInfo>(messagesGrp)
                    );
                })
               .OrderBy(m => m.DateTime.Day)
               .ToList();

                this.Notifs = new ObservableCollection<MessageItemsGroup>(messagesGroups);
                return Notifs;

            });

            //转向单据
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
              .Skip(1)
              .Where(x => x != null)
              .SubOnMainThread(async x =>
              {
                  using (UserDialogs.Instance.Loading("加载中..."))
                  {
                      await _conn.SetPending(x.Id, true);
                      await RedirectAsync(x);
                  }
                  Selecter = null;
              });

            //删除消息
            this.RemoveCommand = ReactiveCommand.Create<MessageInfo>(async x =>
            {
                var ok = await _dialogService.ShowConfirmAsync("是否要删除该消息?", okText: "确定", cancelText: "取消");
                if (ok)
                {
                    await _conn.RemoveMessageInfo(x.Id);
                    ((ICommand)Load)?.Execute(null);
                }
            });

            //删除全部消息
            this.RemoveAllCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (this.Notifs.Count > 0)
                {
                    var ok = await _dialogService.ShowConfirmAsync("是否要删除全部消息?", okText: "确定", cancelText: "取消");
                    if (ok)
                    {
                        await _conn.ResetMessageInfos();
                        ((ICommand)Load)?.Execute(null);
                    }
                }
            }, this.WhenAny(x => x.Notifs, (n) => { return n.GetValue().Count > 0; }));

            this.RemoveCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            this.BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void Initialize(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("MTypeEnum"))
            {
                parameters.TryGetValue("MTypeEnum", out MTypeEnum type);
                this.MType = type;
            }
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}