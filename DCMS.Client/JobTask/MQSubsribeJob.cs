using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Shiny;
using Shiny.Jobs;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DCMS.Client
{
    /// <summary>
    /// MQ消息订阅Job
    /// </summary>
    public class MQSubsribeJob : IJob, IShinyStartupTask
    {
        //private readonly CoreDelegateServices services;
        //public MQSubsribeJob(CoreDelegateServices services) => this.services = services;

        public Task Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            //发送通知
            //await this.services.Notifications.Send(this.GetType(), true, "MQ消息订阅Job已启动",
            //    $"{jobInfo.Identifier} 启动");

            try
            {

                var seconds = jobInfo.Parameters.Get("SecondsToRun", 10);
                Debug.Print($"MQSubsribeJob:{seconds}秒订阅一次数据.....");

                //判断连接状态
                if (!RabbitMQManage.Status && Settings.IsAuthenticated)
                {
                    Task.Run(() =>
                    {
                        if (!string.IsNullOrEmpty(Settings.UserMobile))
                        {
                            try
                            {
                                RabbitMQManage.Subscribe<MessageConsume>(new MesArgs()
                                {
                                    sendEnum = SendEnum.Direct,
                                    exchangeName = "direct_mq",
                                    rabbitQueeName = $"message_{Settings.UserMobile}",
                                    routeName = $"message_{Settings.UserMobile}"
                                }, cancelToken);

                                RabbitMQManage.Subscribe<NoticeConsume>(new MesArgs()
                                {
                                    sendEnum = SendEnum.Direct,
                                    exchangeName = "direct_mq",
                                    rabbitQueeName = $"notice_{Settings.UserMobile}",
                                    routeName = $"notice_{Settings.UserMobile}"
                                }, cancelToken);
                            }
                            catch (Exception) { }
                        }

                    }, cancelToken);
                }
            }
            catch (Exception ex) 
            { 
                Crashes.TrackError(ex); 
            }

            return Task.FromResult(true);

            //发送通知
            //await this.services.Notifications.Send(this.GetType(), false, 
            //    "MQ消息订阅Job已完成", $"{jobInfo.Identifier} 完成");
        }

        public void Start() { }

        //public void Start() => this.services.Notifications.Register(this.GetType(), true, "MQ消息订阅作业");
    }
}
