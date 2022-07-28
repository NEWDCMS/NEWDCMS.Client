
using ReactiveUI.Fody.Helpers;
using System;
namespace Wesley.Client.Models.QA
{
    /// <summary>
    /// 表示 TSS 服务支持（在线帮助）
    /// </summary>
    public class Conversation : Base
    {
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }

        public string[] UserIds { get; set; }
        [Reactive] public Message LastMessage { get; set; }
        [Reactive] public User Peer { get; set; }

        public Conversation()
        {
            UserIds = new string[2];
        }
    }
}
