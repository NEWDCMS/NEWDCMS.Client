using Wesley.Client.Models;
using Wesley.Infrastructure.Helpers;
using EasyNetQ;
using EasyNetQ.Topology;
using Newtonsoft.Json;
using RabbitMQ.Client.Exceptions;
using Shiny;
using Shiny.Notifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
    internal interface ISend
    {
        Task SendMsgAsync(PushMsg pushMsg, IBus bus);

        void SendMsg(PushMsg pushMsg, IBus bus);
    }
    internal class SendMessageMange : ISend
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
    public interface IMessageConsume
    {
        void Consume(string message);
    }
    public class RabbitMQManage
    {
        private static string connectionString => GlobalSettings.PushServerEndpoint;

        private volatile static IBus bus = null;

        public static bool Status = false;

        private static readonly object lockHelper = new object();

        /// <summary>
        /// 创建服务总线
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IBus CreateEventBus()
        {
            //获取RabbitMq的连接地址
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("RabbitMq地址未配置");

            if (bus == null && !string.IsNullOrEmpty(connectionString))
            {
                lock (lockHelper)
                {
                    if (bus == null)
                    {
                        bus = RabbitHutch.CreateBus(connectionString);
                        bus.Advanced.Connected += Advanced_Connected;
                        bus.Advanced.Disconnected += Advanced_Disconnected;
                    }
                }
            }
            return bus;
        }

        private static void Advanced_Disconnected(object sender, DisconnectedEventArgs e)
        {
            Status = false;
        }

        private static void Advanced_Connected(object sender, ConnectedEventArgs e)
        {
            Status = true;
        }

        /// <summary>
        /// 释放服务总线
        /// </summary>
        public static void DisposeBus()
        {
            bus.Advanced.Connected -= Advanced_Connected;
            bus.Advanced.Disconnected -= Advanced_Disconnected;
            bus?.Dispose();
        }

        /// <summary>
        ///  消息同步投递
        /// </summary>
        /// <param name="pushMsg"></param>
        /// <returns></returns>
        public static bool PushMessage(PushMsg pushMsg)
        {
            bool b;
            try
            {
                if (bus == null)
                    CreateEventBus();

                new SendMessageMange().SendMsg(pushMsg, bus);
                b = true;
            }
            catch (Exception)
            {

                b = false;
            }
            return b;
        }

        /// <summary>
        /// 消息异步投递
        /// </summary>
        /// <param name="pushMsg"></param>
        public static async Task PushMessageAsync(PushMsg pushMsg)
        {
            try
            {
                if (bus == null)
                    CreateEventBus();
                await new SendMessageMange().SendMsgAsync(pushMsg, bus);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 消息订阅
        /// </summary>
        public static void Subscribe<TConsum>(MesArgs args, CancellationToken token) where TConsum : IMessageConsume, new()
        {
            try
            {
                if (bus == null)
                    CreateEventBus();

                if (string.IsNullOrEmpty(args.exchangeName))
                    return;


                Expression<Action<TConsum>> methodCall;
                IExchange ex = null;

                //判断推送模式
                if (args.sendEnum == SendEnum.Direct)
                {
                    ex = bus.Advanced.ExchangeDeclare(args.exchangeName, ExchangeType.Direct, cancellationToken: token);
                }
                if (args.sendEnum == SendEnum.Fanout)
                {
                    //广播订阅模式
                    ex = bus.Advanced.ExchangeDeclare(args.exchangeName, ExchangeType.Fanout, cancellationToken: token);
                }
                if (args.sendEnum == SendEnum.Topic)
                {
                    //主题路由模式
                    ex = bus.Advanced.ExchangeDeclare(args.exchangeName, ExchangeType.Topic, cancellationToken: token);
                }

                IQueue qu;
                if (string.IsNullOrEmpty(args.rabbitQueeName))
                    qu = bus.Advanced.QueueDeclare();
                else
                    qu = bus.Advanced.QueueDeclare(args.rabbitQueeName);

                if (!string.IsNullOrEmpty(args.routeName) && !args.routeName.Equals("0"))
                {
                    bus.Advanced.Bind(ex, qu, args.routeName);
                    bus.Advanced.Consume(qu, (body, properties, info) => Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            lock (lockHelper)
                            {
                                //处理消息
                                string message = Encoding.UTF8.GetString(body);
                                if (!string.IsNullOrEmpty(message))
                                {
                                    methodCall = job => job.Consume(message);
                                    methodCall.Compile()(new TConsum());
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }));
                }
            }
            catch (Exception ex) when (!(ex is TimeoutException || ex is BrokerUnreachableException || ex is AlreadyClosedException))
            {
                if (!token.IsCancellationRequested)
                {
                    throw;
                }
            }
        }
    }

    public static class MQConsumer
    {
        public static bool Connected => RabbitMQManage.Status;
        public static void Subsribe(string userId, CancellationToken token)
        {
            try
            {
                RabbitMQManage.Subscribe<MessageConsume>(new MesArgs()
                {
                    sendEnum = SendEnum.Direct,
                    exchangeName = "direct_mq",
                    rabbitQueeName = $"message_{userId}",
                    routeName = $"message_{userId}"
                }, token);

                RabbitMQManage.Subscribe<NoticeConsume>(new MesArgs()
                {
                    sendEnum = SendEnum.Direct,
                    exchangeName = "direct_mq",
                    rabbitQueeName = $"notice_{userId}",
                    routeName = $"notice_{userId}"
                }, token);
            }
            catch (AlreadyClosedException) { }
        }
    }
    public class MessageConsume : IMessageConsume
    {
        public async void Consume(string mqObj)
        {
            var serializer = CommonHelper.FormatJsonStr(mqObj);
            var message = JsonConvert.DeserializeObject<MessageInfo>(serializer);
            if (message != null)
            {
                var nHelper = new NotificationHelper
                {
                    Payload = serializer
                };
                await nHelper.BuildAndSend(message.Title, message.Content, "Message", null);
            }
        }
    }

    public class NoticeConsume : IMessageConsume
    {
        public async void Consume(string mqObj)
        {
            var serializer = CommonHelper.FormatJsonStr(mqObj);
            var message = JsonConvert.DeserializeObject<MessageInfo>(serializer);
            if (message != null)
            {
                var nHelper = new NotificationHelper
                {
                    Payload = serializer
                };
                await nHelper.BuildAndSend(message.Title, message.Content, "Notice", null);
            }
        }
    }

    public class NotificationHelper
    {
        public string SelectedSoundType { get; set; } = "None";
        public int Identifier { get; set; }
        public int BadgeCount { get; set; }
        public string Payload { get; set; }
        public string AndroidChannel { get; set; }
        public bool UseAndroidVibrate { get; set; } = true;
        public bool UseAndroidBigTextStyle { get; set; } = true;
        public bool UseAndroidHighPriority { get; set; } = true;

        public async Task BuildAndSend(string title, string message, string category, DateTime? scheduleDate)
        {
            var _notificationManager = Shiny.ShinyHost.Resolve<INotificationManager>();
            var notification = new Notification
            {
                Title = title,
                Message = message,
                BadgeCount = this.BadgeCount,
                ScheduleDate = scheduleDate,
                Channel = category,
                //Sound = NotificationSound.Default
            };

            //notification.Id = 0;

            if (!this.Payload.IsEmpty())
            {
                notification.Payload = new Dictionary<string, string>
                {
                    { nameof(this.Payload), this.Payload }
                };
            }

            if (!this.AndroidChannel.IsEmpty())
            {
                //notification.Android.ChannelId = this.AndroidChannel;
                //notification.Android.Channel = this.AndroidChannel;
            }

            if (this.UseAndroidHighPriority)
            {
                //notification.Android.Priority = 9;
                //notification.Android.NotificationImportance = AndroidNotificationImportance.Max;
            }

            //notification.Android.Vibrate = this.UseAndroidVibrate;
            notification.Android.UseBigTextStyle = this.UseAndroidBigTextStyle;

            await _notificationManager.Send(notification);

            this.Reset();
        }
        private void Reset()
        {
            this.Payload = String.Empty;
        }
    }

}
