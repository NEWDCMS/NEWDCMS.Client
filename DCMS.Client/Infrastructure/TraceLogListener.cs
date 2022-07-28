#if DEBUG
using System.Diagnostics;
using Xamarin.Forms.Internals;


namespace Wesley.Client
{
    public class TraceLogListener : LogListener
    {
        public override void Warning(string category, string message) => Trace.WriteLine($"[Wesley] {category} : {message}");
    }
}
#endif
