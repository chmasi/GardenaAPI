using System;

namespace Netatmo.Net.Extensions
{
    public static class DateTimeExtensions
    {
        public static double ToUtcTimestamp(this DateTime value)
        {
            var span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            return span.TotalSeconds;
        }

        public static DateTime ToDateTime(this long timestamp)
        {            
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
