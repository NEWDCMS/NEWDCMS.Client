namespace Wesley.Client.Services
{
    public enum SendEnum
    {
        Fanout = 1,
        Direct = 2,
        Topic = 3
    }

    public class PushMsg
    {
        /// <summary>
        /// 发送的数据
        /// </summary>
        public object sendMsg { get; set; }

        /// <summary>
        /// 消息推送的模式
        /// 订阅模式,推送模式,主题路由模式
        /// </summary>
        public SendEnum sendEnum { get; set; }

        /// <summary>
        /// 管道名称
        /// </summary>
        public string exchangeName { get; set; }

        /// <summary>
        /// 队列名称
        /// </summary>
        public string queueName { get; set; }

        /// <summary>
        /// 路由名称
        /// </summary>
        public string routeName { get; set; }
    }

    public class MesArgs
    {
        /// <summary>
        /// 消息推送的模式
        /// 现在支持：订阅模式,推送模式,主题路由模式
        /// </summary>
        public SendEnum sendEnum { get; set; }

        /// <summary>
        /// 管道名称
        /// </summary>
        public string exchangeName { get; set; }

        /// <summary>
        /// 对列名称
        /// </summary>
        public string rabbitQueeName { get; set; }

        /// <summary>
        /// 路由名称
        /// </summary>
        public string routeName { get; set; }
    }
}
