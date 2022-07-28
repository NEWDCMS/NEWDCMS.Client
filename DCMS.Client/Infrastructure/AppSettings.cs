using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Wesley.Client
{
    public interface IAppSettings
    {
        string? Token { get; set; }
        bool UseNotificationsBle { get; set; }
    }

    public class AppSettings : ReactiveObject, IAppSettings
    {
        [Reactive] public string? Token { get; set; }
        [Reactive] public bool UseNotificationsBle { get; set; }
    }
}
