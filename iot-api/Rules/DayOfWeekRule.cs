using System;
using System.Collections.Generic;
using System.Linq;
using iot_api.Extensions;
using Newtonsoft.Json.Linq;

namespace iot_api.Rules
{
    public static class DayOfWeekRule
    {
        public static bool ShouldRun(Dictionary<string, dynamic> fields)
        {
            var daysOfWeek = fields.GetValue<JArray>("days");
            var dayOfWeek = (int) DateTime.Now.DayOfWeek;

            return daysOfWeek.Values().Contains(dayOfWeek);
        }
    }
}