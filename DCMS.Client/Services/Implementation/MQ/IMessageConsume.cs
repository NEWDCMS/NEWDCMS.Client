using Wesley.Client.Models;
using Wesley.Infrastructure.Helpers;
using Newtonsoft.Json;
using System;


namespace Wesley.Client.Services
{
    /// <summary>
    /// 消息消费接口
    /// </summary>
    public interface IMessageConsume
    {
        void Consume(string message);
    }

    /// <summary>
    /// 消息
    /// </summary>
    public class MessageConsume : IMessageConsume
    {
        public void Consume(string mqObj)
        {
            try
            {
                var serializer = CommonHelper.FormatJsonStr(mqObj);
                if (!string.IsNullOrEmpty(serializer))
                {
                    var message = JsonConvert.DeserializeObject<MessageInfo>(serializer);
                    if (message != null)
                    {
                        //var nHelper = new NotificationBuilder
                        //{
                        //    Payload = serializer
                        //};
                        ////Notice 和  Message 
                        ////notificationId
                        //await nHelper.BuildAndSend("Message", 1000, message.Title, message.Content, null);
                    }
                }
            }
            catch (Exception) { }
        }
    }


    /// <summary>
    /// 通知
    /// </summary>
    public class NoticeConsume : IMessageConsume
    {
        public void Consume(string mqObj)
        {
            try
            {
                var serializer = CommonHelper.FormatJsonStr(mqObj);
                if (!string.IsNullOrEmpty(serializer))
                {
                    var message = JsonConvert.DeserializeObject<MessageInfo>(serializer);
                    if (message != null)
                    {
                        //var nHelper = new NotificationBuilder
                        //{
                        //    Payload = serializer
                        //};
                        ////Notice 和  Message 
                        //await nHelper.BuildAndSend("Notice", 2000, message.Title, message.Content, null);
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
