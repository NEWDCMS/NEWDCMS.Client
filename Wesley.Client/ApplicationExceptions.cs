using System;
namespace Wesley.Client
{
    public static class ApplicationExceptions
    {
        public static string ToString(Exception exception)
        {
            return exception switch
            {
                ServerException _ => "抱歉，服务器错误",
                NetworkException _ => "抱歉，无法连接到Internet",
                _ => "抱歉，出错啦！"
            };
        }
    }

    public class ServerException : Exception
    {
    }

    public class NetworkException : Exception
    {
    }
}
