using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;

namespace Wesley.Client.Services
{
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
