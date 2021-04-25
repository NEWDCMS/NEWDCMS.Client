using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny.Settings;

namespace Wesley.Client
{
    public interface IAppSettings
    {
        string? Token { get; set; }
        bool UseNotificationsBle { get; set; }
    }

    public class AppSettings : ReactiveObject, IAppSettings
    {
        public AppSettings(ISettings settings)
        {
            settings.Bind(this);
        }

        [Reactive] public string? Token { get; set; }
        [Reactive] public bool UseNotificationsBle { get; set; }
    }
}
