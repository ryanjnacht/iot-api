using System;

namespace iot_api.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsBetween(this DateTime datetime, TimeSpan start, TimeSpan end)
        {
            // convert datetime to a TimeSpan
            var now = datetime.TimeOfDay;

            // see if start comes before end
            if (start < end)
                return start <= now && now <= end;

            // start is after end, so do the inverse comparison
            return !(end < now && now < start);
        }
    }
}