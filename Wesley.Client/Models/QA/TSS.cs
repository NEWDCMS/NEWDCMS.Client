using System;
namespace Wesley.Client.Models
{
    /// <summary>
    /// 表示 TSS 服务支持（意见反馈）
    /// </summary>
    public class FeedBack : EntityBase
    {
        public int FeedbackTyoe { get; set; }
        public string IssueDescribe { get; set; }
        public string Contacts { get; set; }
        public string Screenshot1 { get; set; }
        public string Screenshot2 { get; set; }
        public string Screenshot3 { get; set; }
        public string Screenshot4 { get; set; }
        public string Screenshot5 { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
