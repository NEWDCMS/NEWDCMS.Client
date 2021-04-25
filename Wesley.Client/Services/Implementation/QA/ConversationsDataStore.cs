using Wesley.Client.Models.QA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public class ConversationsDataStore : IConversationsDataStore
    {
        private readonly List<Conversation> _conversations;
        private readonly List<User> _users;
        public ConversationsDataStore()
        {

            var msgs = new string[]
            {
                "Hi, 请问有什么可以帮你的 ?",
                "Hi, 怎么了？",
                "我早就知道了",
                "怎么开单.",
                "你还在吗?"
            };


            _users = new List<User>
            {
                new User("13002929017", "管理员", "小陈", "业务咨询","face.jpg", 5, 230){IsOnline = true,ConversationId = "ece4f4a60b764339b94a07c84e338a27" },
                new User("13002929027", "客服1", "小周", "技术支持.", "face.jpg", 10, 1500){ConversationId = "eae4f4a60b764339b94a07c84e338a27"  },
                new User("13002929037", "客服2", "小王", "问题反馈","face.jpg", 15, 100){ConversationId = "ebe4f4a60b764339b94a07c84e338a27"  },
                new User("13002929047", "客服3", "小张", "配送和物流查询","face.jpg", 2, 10){ConversationId = "ede4f4a60b764339b94a07c84e338a27"  },
                new User("13002929057", "客服4", "小李", "汇款和发票咨询", "face.jpg", 5, 230){ConversationId = "efe4f4a60b764339b94a07c84e338a27"  }
            };

            _conversations = new List<Conversation>();

            foreach (var user in _users)
            {
                int randomHours = new Random().Next(0, 24);
                _conversations.Add(new Conversation
                {
                    Id = user.ConversationId,
                    LastMessage = new Message
                    {
                        Content = msgs[new Random().Next(0, msgs.Length - 1)],
                        ISent = true,
                        CreationDate = DateTime.UtcNow - TimeSpan.FromHours(randomHours),
                        Sender = user
                    },
                    Peer = user,
                    UserIds = new string[] { "13002929017", user.Id }
                });
            }
            _conversations.OrderByDescending(c => c.LastMessage.CreationDate);
        }

        public Task<bool> AddItemAsync(Conversation item)
        {
            _conversations.Add(item);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            var conversation = _conversations.Where(c => c.Id == id);
            if (!conversation.Any())
                return Task.FromResult(false);
            _conversations.Remove(conversation.FirstOrDefault());
            return Task.FromResult(true);
        }

        public async Task<IEnumerable<Conversation>> GetConversationsForUser(string userId)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return await Task.FromResult(_conversations);
        }

        public Task<Conversation> GetItemAsync(string id)
        {
            return Task.FromResult(_conversations.Where(c => c.Id == id).FirstOrDefault());
        }

        public Task<IEnumerable<Conversation>> GetItemsAsync(bool forceRefresh = false)
        {
            return Task.FromResult((IEnumerable<Conversation>)_conversations);
        }

        public Task<bool> UpdateItemAsync(Conversation item)
        {
            var conv = _conversations.Where(c => c.Id == item.Id).FirstOrDefault();
            var i = _conversations.IndexOf(conv);
            _conversations[i] = conv;
            return Task.FromResult(true);
        }
    }
}
