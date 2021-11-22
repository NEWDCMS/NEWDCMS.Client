using Wesley.Client.Models.QA;
using Wesley.Client.Resources;
using Wesley.Client.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
namespace Wesley.Client.ViewModels
{
    public class MessagerPageViewModel : ViewModelBase
    {
        private readonly IConversationsDataStore _conversationsDataStore;
        private readonly IMessagesDataStore _messagesDataStore;

        [Reactive] public Conversation CurrentConversation { get; set; }
        public ICommand SendMessageCommand { get; private set; }
        public ICommand MessageSwippedCommand { get; private set; }
        public ICommand CancelReplyCommand { get; private set; }
        public ICommand ReplyMessageSelectedCommand { get; private set; }

        [Reactive] public bool IsTyping { get; set; }
        [Reactive] public Message ReplyMessage { get; set; }
        [Reactive] public ObservableCollection<MessagesGroup> Messages { get; set; } = new ObservableCollection<MessagesGroup>();
        private List<Message> _messages { get; set; }

        [Reactive] public string CurrentMessage { get; set; }
        [Reactive] public string ConversationId { get; set; }

        public MessagerPageViewModel(INavigationService navigationService,
            IConversationsDataStore conversationsDataStore,
            IMessagesDataStore messagesDataStore,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "帮助中心";

            _conversationsDataStore = conversationsDataStore;
            _messagesDataStore = messagesDataStore;
            _messages = new List<Message>();


            //回复
            this.ReplyMessageSelectedCommand = ReactiveCommand.Create<Message>((message) =>
            {
                ScrollToMessage(message);
            });

            this.MessageSwippedCommand = ReactiveCommand.Create<Message>((message) =>
            {
                ReplyMessage = message;
                MessagingCenter.Send(this, Constants.ShowKeyboard, new MyFocusEventArgs { IsFocused = true });
            });

            //发送消息
            this.SendMessageCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var message = new Message
                {
                    Content = CurrentMessage,
                    ReplyTo = ReplyMessage,
                    CreationDate = DateTime.Now,
                    Sender = new User(),
                    ISentPreviousMessage = (bool)Messages?.Last()?.Last()?.ISent,
                    ISent = true,
                    ConversationId = CurrentConversation.Id,
                    SenderId = new User().Id
                };

                CurrentConversation.LastMessage = message;
                await _conversationsDataStore.UpdateItemAsync(CurrentConversation);
                CurrentMessage = string.Empty;
                Messages.Last().Add(message);
                ReplyMessage = null;
                await _messagesDataStore.AddItemAsync(message);
                CurrentConversation.LastMessage = message;
                ScrollToMessage(message);
                await FakeMessaging();

            }, this.WhenAnyValue(vm => vm.CurrentMessage, curm => !String.IsNullOrEmpty(curm)));


            //取消回复
            this.CancelReplyCommand = ReactiveCommand.Create(() =>
            {
                ReplyMessage = null;
                MessagingCenter.Send(this, Constants.ShowKeyboard, new MyFocusEventArgs { IsFocused = false });
            });


            //初始化 
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {
                //获取对话
                this.CurrentConversation = await _conversationsDataStore.GetItemAsync(this.ConversationId);

                //获取消息
                var messages = await _messagesDataStore.GetMessagesForConversation(this.ConversationId);

                _messages.AddRange(messages);

                var messagesGroups = _messages.GroupBy(m => m.CreationDate.Day)
                    .Select(grp =>
                    {
                        var messagesGrp = grp.ToList().OrderBy(m => m.CreationDate);
                        var msg = messagesGrp.First();
                        var date = msg.CreationDate.Date;
                        var dayDiff = DateTime.Now.Day - date.Day;
                        string groupHeader = string.Empty;

                        if (dayDiff == 0)
                            groupHeader = TextResources.Today;
                        else if (dayDiff == 1)
                            groupHeader = TextResources.Yesterday;
                        else groupHeader = date.ToString("MM-dd-yyyy");

                        return new MessagesGroup
                        (
                            dateTime: date,
                            groupHeader: groupHeader,
                            messages: new ObservableCollection<Message>(messagesGrp)
                        );
                    })
                    .OrderBy(m => m.DateTime.Day)
                    .ToList();

                this.Messages = new ObservableCollection<MessagesGroup>(messagesGroups);

                await Task.Delay(TimeSpan.FromSeconds(0.5));
                if (Messages.Any())
                    ScrollToMessage(Messages?.Last()?.Last());

            }));


            this.BindBusyCommand(Load);

        }

        private void ScrollToMessage(Message message)
        {
            MessagingCenter.Send(this, Constants.ScrollToItem, new ScrollToItemEventArgs { Item = message });
        }


        /// <summary>
        /// 模拟消息
        /// </summary>
        /// <returns></returns>
        public async Task FakeMessaging()
        {
            //var shouldReply = new Random().Next(0, 3) > 0 ? true : false;
            var shouldReply = true;

            if (shouldReply)
            {
                ScrollToMessage(Messages.Last().Last());
                IsTyping = true;
                await Task.Delay(TimeSpan.FromSeconds(3));
                var message = new Message
                {
                    Content = "你好，欢迎使用Wesley帮助服务，请问你有什么需要解答的吗？",
                    CreationDate = DateTime.Now,
                    Sender = CurrentConversation.Peer,
                    ISentPreviousMessage = Messages.Last().Last().ISent,
                    ISent = false,
                    ConversationId = CurrentConversation.Id,
                    SenderId = CurrentConversation.Peer.Id
                };
                Messages.Last().Add(message);
                CurrentConversation.LastMessage = message;
                await _conversationsDataStore.UpdateItemAsync(CurrentConversation);

                IsTyping = false;
                ScrollToMessage(message);
                await _messagesDataStore.AddItemAsync(message);
            }
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("ConversationId"))
            {
                parameters.TryGetValue("ConversationId", out string conversationId);
                this.ConversationId = conversationId.ToString();
                ((ICommand)Load)?.Execute(null);
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
