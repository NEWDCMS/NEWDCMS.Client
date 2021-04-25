namespace Wesley.Client.Services
{
    public interface IBaiduNavigationService
    {

        void OpenNavigationTo(double latitude, double longitude, string addressName);
    }
}
