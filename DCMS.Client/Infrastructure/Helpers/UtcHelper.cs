using System;

namespace Wesley.Infrastructure.Helpers
{
    public static class UtcHelper
    {
        public static DateTime ConvertDateTimeInt(DateTime time)
        {
            //DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1));
            double result = (time - startTime).TotalSeconds;

            startTime = startTime.AddSeconds(result);
            startTime = startTime.AddHours(8);

            return startTime;
        }

    }
}
