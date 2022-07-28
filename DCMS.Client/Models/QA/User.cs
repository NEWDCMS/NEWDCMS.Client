
using ReactiveUI.Fody.Helpers;
using System;
namespace Wesley.Client.Models.QA
{
    /// <summary>
    /// 表示 TSS 服务支持（在线帮助）
    /// </summary>
    public class User : Base
    {
        public string Id { get; set; }
        public string ConversationId { get; set; }
        public DateTime CreationDate { get; set; }
        [Reactive] public string FirstName { get; set; }
        [Reactive] public string LastName { get; set; }
        [Reactive] public string Bio { get; set; }
        [Reactive] public int NumberOfConversations { get; set; }
        [Reactive] public int NumberOfMessagesSent { get; set; }
        [Reactive] public string ProfilePic { get; set; }
        [Reactive] public bool IsOnline { get; set; }

        public User()
        {

        }

        public User(string userId, string firstName, string secondName, string bio, string profilePic, int numberOfConversations, int numberOfMessagesSent)
        {
            Id = userId;
            FirstName = firstName;
            LastName = secondName;
            NumberOfConversations = numberOfConversations;
            Bio = bio;
            ProfilePic = profilePic;
            NumberOfMessagesSent = numberOfMessagesSent;
        }
    }
}
