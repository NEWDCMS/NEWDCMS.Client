using DCMS.BitImageEditor.IOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformHelper))]

namespace DCMS.BitImageEditor.IOS
{
    internal class PlatformHelper : IPlatformHelper
    {
        public bool IsInitialized => Platform.IsInitialized;
    }
}