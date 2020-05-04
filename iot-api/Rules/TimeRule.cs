using System;
using System.Collections.Generic;
using iot_api.Extensions;

namespace iot_api.Rules
{
    public static class TimeRule
    {
        public static bool ShouldRun(Dictionary<string, dynamic> fields)
        {
            var startTime = TimeSpan.Parse(fields.GetValue<string>("start_time"));
            var endTime = TimeSpan.Parse(fields.GetValue<string>("end_time"));

            return DateTime.Now.IsBetween(startTime, endTime);
        }
    }
}