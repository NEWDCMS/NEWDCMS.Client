using DCMS.BitImageEditor.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformHelper))]

namespace DCMS.BitImageEditor.Droid
{
    internal class PlatformHelper : IPlatformHelper
    {
        public bool IsInitialized => Platform.IsInitialized;
    }
}