using EasyNetQ;
using EasyNetQ.Topology;

using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    /// <summary>
    /// 消息发送器接口
    /// </summary>
    public interface IMessageSender
    {
        Task SendMsgAsync(PushMsg pushMsg, IBus bus);
        void SendMsg(PushMsg pushMsg, IBus bus);
    }


    public class SendMessageMange : IMessageSender
    {
        public async Task SendMsgAsync(PushMsg pushMsg, IBus bus)
        {
            //一对一推送

            var message = new Message<object>(pushMsg.sendMsg);
            IExchange ex = null;

            //判断推送模式
            if (pushMsg.sendEnum == SendEnum.Direct)
            {
                ex = bus.Advanced.ExchangeDeclare(pushMsg.exchangeName, ExchangeType.Direct);
            }
            if (pushMsg.sendEnum == SendEnum.Fanout)
            {
                //广播订阅模式
                ex = bus.Advanced.ExchangeDeclare(pushMsg.exchangeName, ExchangeType.Fanout);
            }
            if (pushMsg.sendEnum == SendEnum.Topic)
            {
                //主题路由模式
                ex = bus.Advanced.ExchangeDeclare(pushMsg.exchangeName, ExchangeType.Topic);
            }

            await bus.Advanced.PublishAsync(ex, pushMsg.routeName, false, message)
             .ContinueWith(task =>
             {
                 if (!task.IsCompleted && task.IsFaulted)//消息投递失败
                 {
                     //记录投递失败的消息信息   
                 }
             });
        }
        public void SendMsg(PushMsg pushMsg, IBus bus)
        {
            //一对一推送
            var basicProperties = new MessageProperties
            {
                //这里的deliveryMode=1代表不持久化，deliveryMode=2代表持久化。
                DeliveryMode = 2
            };

            var message = new Message<object>(pushMsg.sendMsg, basicProperties);
            IExchange ex = null;
            //判断推送模式
            if (pushMsg.sendEnum == SendEnum.Direct)
            {
                //Exchange持久化
                ex = bus.Advanced.ExchangeDeclare(pushMsg.exchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
            }
            if (pushMsg.sendEnum == SendEnum.Fanout)
            {
                //广播订阅模式 持久化
                ex = bus.Advanced.ExchangeDeclare(pushMsg.exchangeName, ExchangeType.Fanout, durable: true, autoDelete: false);
            }
            if (pushMsg.sendEnum == SendEnum.Topic)
            {
                //主题路由模式 持久化
                ex = bus.Advanced.ExchangeDeclare(pushMsg.exchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
            }

            //申明message 队列
            var queue = bus.Advanced.QueueDeclare(pushMsg.queueName, durable: true, exclusive: false, autoDelete: false);


            //绑定队列到交换并映射路由
            bus.Advanced.Bind(ex, queue, pushMsg.routeName);


            //bus.Advanced.Publish(ex, pushMsg.routeName, false, message);
            bus.Advanced.Publish(ex, pushMsg.routeName, false, message);

        }
    }
}
