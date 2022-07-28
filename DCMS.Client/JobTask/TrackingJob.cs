using DCMS.Client.BaiduMaps;
using DCMS.Client.Services;
using Microsoft.AppCenter.Crashes;
using Shiny;
using Shiny.Jobs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DCMS.Client
{
    /// <summary>
    /// 位置上报作业
    /// </summary>
    public class TrackingJob : IJob, IShinyStartupTask
    {
        public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            try
            {
                var seconds = jobInfo.Parameters.Get("SecondsToRun", 0);
                Debug.Print($"TrackingJob:{seconds}秒上报一次数据.....");

                if (Settings.IsAuthenticated)
                {
                    var _terminalService = App.Resolve<ITerminalService>();
                    var _conn = App.Resolve<LocalDatabase>();
                    var _locationService = App.Resolve<IBaiduLocationService>();
                    //如果百度定位服务停止，则拉起
                    if (_locationService != null)
                    {
                        if (!_locationService.IsStarted())
                        {
                            _locationService.Start();
                        }
                    }

                    if (_terminalService != null)
                    {
                        var trackings = await _conn?.LocationSyncEvents?.ToListAsync();
                        if (trackings != null && trackings.Any())
                        {

                            if (trackings.Count >= 50)
                            {
                                int[] ids = trackings.Select(s => s.Id).ToArray();
                                await _terminalService.ReportingTrackAsync(trackings);
                                await _conn?.LocationSyncEvents.DeleteAsync(s => ids.Contains(s.Id));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        public void Start() { }
    }
}
