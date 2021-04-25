using System;
namespace Wesley.Client.AutoUpdater.Exceptions
{

    /// <summary>
    /// 自定义自动更新异常
    /// </summary>
    public class AutoUpdateException : Exception
    {
        public AutoUpdateException(string message = "") : base(message)
        { }
    }
}
