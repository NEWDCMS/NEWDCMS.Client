namespace Wesley.Client
{
    public interface IConnectivityHandler
    {
        /// <summary>
        /// Map Apizr connectivity check to your connectivity handler
        /// </summary>
        /// <returns></returns>
        bool IsConnected();
    }
}
