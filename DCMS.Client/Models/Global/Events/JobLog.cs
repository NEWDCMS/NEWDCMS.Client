using System;
namespace Wesley.Client.Models
{
    public class PushEvent : EntityBase
    {
        public string? Token { get; set; }
        public string? Payload { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class NotificationEvent : EntityBase
    {
        public int NotificationId { get; set; }
        public string? NotificationTitle { get; set; }
        public string? Action { get; set; }
        public string? ReplyText { get; set; }
        public string? Payload { get; set; }
        public bool IsEntry { get; set; }
        public DateTime Timestamp { get; set; }
    }


}
