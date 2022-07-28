using System;
using System.Threading.Tasks;

namespace Wesley.Client.AutoUpdater
{
    public class UpdateManagerParameters
    {
        public Func<Task<UpdatesCheckResponse>> CheckForUpdatesFunction { get; set; }
        public TimeSpan? RunEvery { get; set; }
    }
}
