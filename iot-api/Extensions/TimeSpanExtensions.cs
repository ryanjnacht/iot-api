using System;

namespace iot_api.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan WithoutMilliseconds(this TimeSpan ts)
        {
            return new TimeSpan(ts.Hours, ts.Minutes, ts.Seconds);
        }
    }
}