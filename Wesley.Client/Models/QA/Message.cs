
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wesley.Client.Models.QA
{
    /// <summary>
    /// 表示 TSS 服务支持（在线帮助）
    /// </summary>
    public class MessagesGroup : ObservableCollection<Message>
    {
        public string GroupHeader { get; set; }
        public DateTime DateTime { get; set; }

        public MessagesGroup(DateTime dateTime, string groupHeader, IEnumerable<Message> messages) : base(messages)
        {
            DateTime = dateTime;
            GroupHeader = groupHeader;
        }
    }

    /// <summary>
    /// 表示 TSS 服务支持（在线帮助）
    /// </summary>
    public class Message : Base
    {
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
        [Reactive] public string Content { get; set; }
        [Reactive] public string ConversationId { get; set; }
        [Reactive] public bool ISent { get; set; }

        [Reactive] public bool ISentPreviousMessage { get; set; }

        [Reactive] public Message ReplyTo { get; set; }
        [Reactive] public string SenderId { get; set; }
        [Reactive] public User Sender { get; set; }
    }
}
