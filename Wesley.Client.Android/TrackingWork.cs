using Android.Content;
using Android.Util;
using AndroidX.Work;
using Wesley.Client.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Wesley.Client.Droid
{
    public class TrackingWork : Worker
    {
        public TrackingWork(Context context, WorkerParameters workerParameters) : base(context, workerParameters)
        {
        }
        public override Result DoWork()
        {
            try
            {
                System.Diagnostics.Debug.Print($"TrackingWork:30秒上报一次数据.....");
                if (!GlobalSettings.IsNotConnected)
                {
                    var taskList = new List<Task>
                    {
                       SendReport()
                    };
                    Task.WhenAll(taskList.ToArray()).ConfigureAwait(false);
                }

                return new Result.Retry();
            }
            catch (Exception)
            {
                return new Result.Failure();
            }
        }


        /// <summary>
        /// 上报
        /// </summary>
        private async Task SendReport()
        {
            try
            {
                if (!string.IsNullOrEmpty(Settings.CompanySetting))
                {
                    var _terminalService = App.Resolve<ITerminalService>();
                    var _conn = App.Resolve<LocalDatabase>();
                    if (_terminalService != null)
                    {
                        var trackings = await _conn?.LocationSyncEvents.ToListAsync();
                        if (trackings != null && trackings.Any())
                        {
                            Java.Lang.StringBuffer sb = new Java.Lang.StringBuffer();
                            foreach (var tracking in trackings)
                            {
                                tracking.StoreId = Settings.StoreId;
                                tracking.BusinessUserId = Settings.UserId;
                                sb.Append(JsonConvert.SerializeObject(tracking));
                            }
                            await _terminalService.ReportingTrackAsync(trackings);
                        }
                        //重建
                        await _conn.ResetLocationSyncEvents();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("SendReport", ex.Message);
            }
        }

    }
}