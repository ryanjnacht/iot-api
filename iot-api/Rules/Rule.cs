using System;
using System.Collections.Generic;
using iot_api.Extensions;
using Newtonsoft.Json.Linq;

namespace iot_api.Rules
{
    public class Rule
    {
        public Rule(JToken json)
        {
            Fields = json.ToObject<Dictionary<string, dynamic>>();
        }

        public Dictionary<string, dynamic> Fields { get; set; }
        public string Id => Fields.GetValue<string>("id");
        public string Type => Fields.GetValue<string>("type");

        public bool ShouldRunRule()
        {
            var type = Fields.GetValue<string>("type");
            switch (type)
            {
                case "time":
                    return ShouldRunTimeRule();
                default:
                    throw new InvalidOperationException();
            }
        }

        private bool ShouldRunTimeRule()
        {
            var startTime = TimeSpan.Parse(Fields.GetValue<string>("start_time"));
            var endTime = TimeSpan.Parse(Fields.GetValue<string>("end_time"));

            return DateTime.Now.IsBetween(startTime, endTime);
        }

        public JObject ToJObject()
        {
            return JObject.FromObject(Fields);
        }
    }
}