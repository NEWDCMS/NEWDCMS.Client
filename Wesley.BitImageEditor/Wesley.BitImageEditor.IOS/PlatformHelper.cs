using Wesley.BitImageEditor.IOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformHelper))]

namespace Wesley.BitImageEditor.IOS
{
    internal class PlatformHelper : IPlatformHelper
    {
        public bool IsInitialized => Platform.IsInitialized;
    }
}