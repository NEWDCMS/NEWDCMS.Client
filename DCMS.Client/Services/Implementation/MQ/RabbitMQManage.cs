using EasyNetQ;
using EasyNetQ.Topology;
using RabbitMQ.Client.Exceptions;
using System;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
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

        /// <summary>
        /// 连接断开时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Advanced_Disconnected(object sender, DisconnectedEventArgs e)
        {
            Status = false;
        }


        /// <summary>
        /// 连接时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Advanced_Connected(object sender, ConnectedEventArgs e)
        {
            Status = true;
        }

        /// <summary>
        /// 释放服务总线
        /// </summary>
        public static void DisposeBus()
        {
            try
            {
                if (bus != null)
                {
                    Status = false;
                    bus.Advanced.Connected -= Advanced_Connected;
                    bus.Advanced.Disconnected -= Advanced_Disconnected;
                    bus?.Dispose();
                    bus = null;
                }
            }
            catch (Exception) { }
        }


        /// <summary>
        /// 消息订阅
        /// </summary>
        public static void Subscribe<TConsum>(MesArgs args, CancellationToken token) where TConsum : IMessageConsume, new()
        {
            try
            {
                if (string.IsNullOrEmpty(Settings.UserMobile) && string.IsNullOrEmpty(Settings.PusherMQEndpoint))
                    return;

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
                                    if (methodCall != null)
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
                    //System.Threading.Tasks.TaskCanceledException: A task was canceled
                    throw new Exception("任务取消");
                }
            }
        }
    }
}
