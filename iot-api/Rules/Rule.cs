using System;
using System.Collections.Generic;
using iot_api.DataAccess;
using iot_api.Extensions;
using Newtonsoft.Json.Linq;

namespace iot_api.Rules
{
    public class Rule : IDocument
    {
        public Rule(JToken json)
        {
            Fields = json.ToObject<Dictionary<string, dynamic>>();
        }

        public Dictionary<string, dynamic> Fields { get; set; }
        public string Id => Fields.GetValue<string>("id");

        public string Type => Fields.GetValue<string>("type");

        public bool ShouldRun()
        {
            var type = Fields.GetValue<string>("type").ToLower();
            switch (type)
            {
                case "time":
                    return TimeRule.ShouldRun(Fields);
                case "dayofweek":
                    return DayOfWeekRule.ShouldRun(Fields);
                default:
                    throw new InvalidOperationException();
            }
        }
        
        JObject IDocument.ToJObject => ToJObject();
        public JObject ToJObject()
        {
            return JObject.FromObject(Fields);
        }
    }
}