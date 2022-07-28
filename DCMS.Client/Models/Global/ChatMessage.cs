namespace Wesley.Client.Models
{
    public enum MessageEnumType : int
    {
        Links = 0,
        Text = 1,
        Json = 2,
        RichText = 3
    }

    public class MessageHeader
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 加密私钥
        /// </summary>
        public string PrivateKey { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageEnumType MessageType { get; set; }
    }

    public class ReciveMessageInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public int MType { get; set; }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string MessageTitle { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string MessageBody { get; set; }
    }
}

