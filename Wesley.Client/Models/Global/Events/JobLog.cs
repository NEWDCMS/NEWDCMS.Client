using ReactiveUI;
using SQLite;
using System;
namespace Wesley.Client.Models
{
    /// <summary>
    /// 系统错误日志
    /// </summary>
    public class ErrorLog : ReactiveObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string Parameters { get; set; }
    }


    public class PushEvent
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string? Token { get; set; }
        public string? Payload { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class NotificationEvent
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public int NotificationId { get; set; }
        public string? NotificationTitle { get; set; }
        public string? Action { get; set; }
        public string? ReplyText { get; set; }
        public string? Payload { get; set; }
        public bool IsEntry { get; set; }
        public DateTime Timestamp { get; set; }
    }


}
