using Wesley.Client.Services;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Wesley.Client
{

    public interface ILogService
    {
        string Id { get; }

    }

    public class LogService : ILogService
    {
        public LogService(ILogger<LogService> logger)
        {
            logger.LogDebug($"Resolved object with ID: {Id} of type: \"{GetType().Name}\" ");
        }

        public string Id { get; } = Guid.NewGuid().ToString();

    }


    public class CrashlyticsService : ICrashlyticsService
    {

        public void TrackError(Exception e)
        {
            try
            {
                Crashes.TrackError(e);
            }
            catch (Exception) { }
        }

        public void TrackEvent(string eventName, IDictionary<string, string> dictionary)
        {
            try
            {
                Analytics.TrackEvent(eventName, dictionary);
            }
            catch (Exception) { }
        }
    }
}
