using System;
using System.Collections.Generic;

namespace Wesley.Client.Services
{
    public interface ICrashlyticsService
    {
        void TrackEvent(string eventName, IDictionary<string, string> dictionary);

        void TrackError(Exception e);
    }
}
