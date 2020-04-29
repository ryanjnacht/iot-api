using System;
using System.Collections.Generic;

namespace iot_api.Extensions
{
    public static class DictionaryExtensions
    {
        public static T GetValue<T>(this Dictionary<string, dynamic> dict, string key)
        {
            if (!dict.ContainsKey(key)) return default;

            var dVal = dict[key];
            var t = typeof(T);

            return Convert.ChangeType(dVal, t);
        }

        public static void AddOrUpdate(this Dictionary<string, dynamic> dict, string key, dynamic value)
        {
            if (dict.ContainsKey(key))
                dict[key] = value;
            else
                dict.Add(key, value);
        }
    }
}