using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using iot_api.DataAccess;
using iot_api.Extensions;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace iot_api.Scheduler
{
    public class Schedule : IDocument
    {
        public Schedule(string startTime, IEnumerable days, List<SchedulerDevice> devices)
        {
            var json = new JObject
            {
                {"start_time", TimeSpan.Parse(startTime).WithoutMilliseconds().ToString()},
                {"devices", JToken.FromObject(devices)}
            };

            if (days == null)
                json.Add("days", new JArray());
            else
                json.Add("days", JArray.FromObject(days));

            Fields = json.ToObject<Dictionary<string, dynamic>>();
        }

        public Schedule(JToken json)
        {
            Fields = json.ToObject<Dictionary<string, dynamic>>();
        }

        private IEnumerable<int> Days => Fields.GetValue<JArray>("days").Select(jv => (int) jv).ToList();

        public IEnumerable<SchedulerDevice> Devices
        {
            get
            {
                var devices = new List<SchedulerDevice>();
                foreach (var jObj in Fields.GetValue<JArray>("devices"))
                    devices.Add(new SchedulerDevice
                    {
                        DeviceId = jObj["deviceId"]?.ToString(),
                        Action = jObj["action"]?.ToString()
                    });

                return devices;
            }
        }

        private TimeSpan StartTime => TimeSpan.Parse(Fields.GetValue<string>("start_time"));

        private Dictionary<string, dynamic> Fields { get; }

        [BsonElement("_id")]
        [JsonProperty("id")]
        public string Id => Fields.GetValue<string>("id");

        JObject IDocument.ToJObject => ToJObject();

        public JObject ToJObject()
        {
            return JObject.FromObject(Fields);
        }

        public bool ShouldRun(TimeSpan? now = null)
        {
            now ??= DateTime.Now.TimeOfDay.WithoutMilliseconds();

            if (Days != null && Days.Any() && !Days.Contains((int) DateTime.Now.DayOfWeek))
                return false;

            return StartTime == now;
        }
    }

    public class SchedulerDevice
    {
        [JsonProperty("action")] public string Action;
        [JsonProperty("deviceId")] public string DeviceId;
    }
}