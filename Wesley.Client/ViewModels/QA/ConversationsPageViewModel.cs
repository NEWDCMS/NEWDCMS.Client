using Wesley.Client.Models.QA;
using Wesley.Client.Services;
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
    public class ConversationsPageViewModel : ViewModelBase
    {
        private readonly IConversationsDataStore _conversationsDataStore;

        [Reactive] public IList<Conversation> Conversations { get; set; } = new ObservableCollection<Conversation>();
        //[Reactive] public Conversation Selecter { get; set; }

        public ICommand FilterOptionChangedCommand { get; private set; }
        public ICommand ConversationSelectedCommand { get; private set; }
        [Reactive] public string SerchKey { get; set; }
        private bool notOnLine = true;
        [Reactive] public string OnLineCount { get; set; } = "0";

        private IEnumerable<Conversation> ConversationAll = new List<Conversation>();
        private IEnumerable<Conversation> ConversationOnline = new List<Conversation>();

        public ConversationsPageViewModel(INavigationService navigationService,
            IConversationsDataStore conversationsDataStore,
            IDialogService dialogService
            ) : base(navigationService, dialogService)
        {
            Title = "帮助中心";
            _conversationsDataStore = conversationsDataStore;

            //搜索
            this.WhenAnyValue(x => x.Filter.SerchKey)

                .Select(s => s)
                .Throttle(TimeSpan.FromSeconds(2), RxApp.MainThreadScheduler)
                .Subscribe(s =>
                {
                    ((ICommand)SerchCommand)?.Execute(s);
                }).DisposeWith(DeactivateWith);
            this.SerchCommand = ReactiveCommand.Create<string>(e =>
            {
                if (!IsBusy)
                {
                    ((ICommand)Load)?.Execute(null);
                }

            });

            //初始化 
            this.Load = ReactiveCommand.CreateFromTask(() => Task.Run(async () =>
            {

                await FilterOptionChanged();
            }));

            this.FilterOptionChangedCommand = ReactiveCommand.CreateFromTask<bool>(OnTabChange,
                this.WhenAnyValue(vm => vm.IsBusy, isBusy => !isBusy));

            //选择客服
            this.ConversationSelectedCommand = ReactiveCommand.Create<Conversation>(async item =>
           {
               await this.NavigateAsync("MessagerPage", ("ConversationId", item.Id));
               //this.Selecter = null;
           });

            this.BindBusyCommand(Load);

        }

        /// <summary>
        /// 获取对话列表
        /// </summary>
        /// <param name="notOnline"></param>
        /// <returns></returns>
        private async Task FilterOptionChanged()
        {


            var conversations = await _conversationsDataStore.GetConversationsForUser("13002929017");
            if (!string.IsNullOrEmpty(Filter.SerchKey))
            {
                conversations = conversations.Where(c => c.Peer.FirstName.Contains(Filter.SerchKey));

            }
            conversations = conversations.OrderByDescending(c => c.LastMessage.CreationDate);
            ConversationAll = conversations;
            ConversationOnline = ConversationAll.Where(c => c.Peer.IsOnline);
            this.OnLineCount = ConversationOnline.Count().ToString();
            ChangeTabData();


            //if (!notOnline)
            //{
            //    conversations = conversations.Where(c => c.Peer.IsOnline);
            //}
            //else
            //{
            //    conversations = conversations.OrderByDescending(c => c.LastMessage.CreationDate);
            //}
            //if (!string.IsNullOrEmpty(Filter.SerchKey)) 
            //{
            //    conversations = conversations.Where(c => c.Peer.FirstName.Contains(Filter.SerchKey));

            //}
            //conversations = conversations.OrderByDescending(c => c.LastMessage.CreationDate);

            //this.Conversations = new ObservableCollection<Conversation>(conversations);
        }
        private void ChangeTabData()
        {
            if (this.notOnLine)
            {
                this.Conversations = new ObservableCollection<Conversation>(ConversationAll);
            }
            else
            {
                this.Conversations = new ObservableCollection<Conversation>(ConversationOnline);
            }

        }

        private async Task OnTabChange(bool flag)
        {
            this.notOnLine = flag;
            if (ConversationAll != null && ConversationAll.Count() != 0)
            {
                ChangeTabData();
                return;
            }
            await FilterOptionChanged();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
