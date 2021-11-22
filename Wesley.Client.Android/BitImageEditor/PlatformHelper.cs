using Wesley.BitImageEditor.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformHelper))]

namespace Wesley.BitImageEditor.Droid
{
    internal class PlatformHelper : IPlatformHelper
    {
        public bool IsInitialized => Platform.IsInitialized;
    }
}