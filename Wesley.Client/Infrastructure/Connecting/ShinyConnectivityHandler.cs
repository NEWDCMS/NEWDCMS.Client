using Shiny.Net;

namespace Wesley.Client
{
    public class ShinyConnectivityHandler : IConnectivityHandler
    {
        private readonly IConnectivity _shinyConnectivity;

        public ShinyConnectivityHandler(IConnectivity shinyConnectivity)
        {
            _shinyConnectivity = shinyConnectivity;
        }

        public bool IsConnected() => _shinyConnectivity.IsInternetAvailable();
    }
}
