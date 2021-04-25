using Wesley.Client.Models.QA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public class MessagesDataStore : IMessagesDataStore
    {
        private readonly List<Message> _messages;

        public MessagesDataStore()
        {
            _messages = new List<Message>();
        }

        public Task<bool> AddItemAsync(Message item)
        {
            item.Id = Guid.NewGuid().ToString();
            _messages.Add(item);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Message> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetItemsAsync(bool forceRefresh = false)
        {
            return Task.FromResult((IEnumerable<Message>)_messages);
        }

        public Task<IEnumerable<Message>> GetMessagesForConversation(string conversationId)
        {
            var conversation = new ConversationsDataStore().GetItemAsync(conversationId).Result;

            _messages.Add(new Message
            {
                ConversationId = conversation.Id,
                Id = Guid.NewGuid().ToString(),
                Content = "亲，有什么需要帮助的吗？",
                CreationDate = DateTime.Now - TimeSpan.FromDays(1),
                ISent = false,
                SenderId = conversation.Peer.Id,
                Sender = conversation.Peer
            });
            _messages.Add(new Message
            {
                ConversationId = conversation.Id,
                Id = Guid.NewGuid().ToString(),
                Content = "销售订单怎么提交？",
                CreationDate = DateTime.Now - TimeSpan.FromDays(1),
                ISent = true,
                SenderId = conversation.UserIds[0],
            });
            _messages.Add(new Message
            {
                ConversationId = conversation.Id,
                Id = Guid.NewGuid().ToString(),
                ISentPreviousMessage = true,
                Content = "进入首页，或者全部应用，进入拜访，签到后，点击开单就可用开销售订单了呢",
                CreationDate = DateTime.Now - TimeSpan.FromMinutes(5),
                ISent = false,
                SenderId = conversation.Peer.Id,
                Sender = conversation.Peer
            });
            _messages.Add(new Message
            {
                ConversationId = conversation.Id,
                Id = Guid.NewGuid().ToString(),
                ISentPreviousMessage = false,
                Content = "同步显示员工的销售业绩排名、自动生成的商品列表能实现客户远程下单操作吗？",
                CreationDate = DateTime.Now - TimeSpan.FromMinutes(2),
                ISent = true,
                SenderId = conversation.Peer.Id,
                Sender = conversation.Peer,
                ReplyTo = _messages[_messages.Count - 3]
            });
            _messages.Add(new Message
            {
                ConversationId = conversation.Id,
                Id = Guid.NewGuid().ToString(),
                ISentPreviousMessage = true,
                Content = "可以的，系统支持。",
                CreationDate = DateTime.Now - TimeSpan.FromMinutes(1),
                ISent = false,
                SenderId = conversation.Peer.Id,
                Sender = conversation.Peer,
                ReplyTo = _messages[_messages.Count - 2]
            });
            _messages.Add(new Message
            {
                ConversationId = conversation.Id,
                Id = Guid.NewGuid().ToString(),
                ISentPreviousMessage = true,
                Content = "点击销售订单打开界面,单击界面里所示的增加的+号,出现销售订单的窗口:输入所有内 容。",
                CreationDate = DateTime.Now - TimeSpan.FromMinutes(1),
                ISent = false,
                SenderId = conversation.Peer.Id,
                Sender = conversation.Peer
            });
            _messages.Add(new Message
            {
                ConversationId = conversation.Id,
                Id = Guid.NewGuid().ToString(),
                ISentPreviousMessage = false,
                Content = "好的，非常感谢！",
                CreationDate = DateTime.Now,
                ISent = true,
                SenderId = conversation.Peer.Id,
                Sender = conversation.Peer
            });
            conversation.LastMessage = _messages.Last();

            var msgs = _messages.Where(m => m.ConversationId == conversationId);

            return Task.FromResult(msgs);
        }

        public Task<bool> UpdateItemAsync(Message item)
        {
            throw new NotImplementedException();
        }
    }
}
