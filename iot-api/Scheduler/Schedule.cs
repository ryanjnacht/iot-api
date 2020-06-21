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
        //[JsonProperty("days")] // public int[]? Days;
        //private readonly int[] _days;
        private IEnumerable<int> Days => Fields.GetValue<JArray>("days").Select(jv => (int) jv).ToList();

        //[JsonProperty("devices")] // public List<SchedulerDevice> Devices;
        //public readonly List<SchedulerDevice> Devices;
        public IEnumerable<SchedulerDevice> Devices
        {
            get
            {
                var devices = new List<SchedulerDevice>();
                foreach(var jObj in Fields.GetValue<JArray>("devices"))
                {
                    devices.Add(new SchedulerDevice
                    {
                        DeviceId = jObj["deviceId"]?.ToString(),
                        Action = jObj["action"]?.ToString()
                        
                    });
                }

                return devices;
            }
        }

        //[JsonProperty("start_time")] // public TimeSpan StartTime;
        //private readonly TimeSpan _startTime;
        private TimeSpan StartTime => TimeSpan.Parse(Fields.GetValue<string>("start_time"));

        //[JsonIgnore] [BsonElement("fields")] private Dictionary<string, dynamic>? Fields { get; }
        private Dictionary<string, dynamic> Fields { get; set; }

        public Schedule(string startTime, IEnumerable days, List<SchedulerDevice> devices)
        {
            var json = new JObject
            {
                {"start_time", TimeSpan.Parse(startTime).WithoutMilliseconds().ToString()},
                {"devices", JToken.FromObject(devices)}
            };

            if (days == null)
            {
                json.Add("days", new JArray());
            }
            else
            {
                json.Add("days", JArray.FromObject(days));
            }

            Fields = json.ToObject<Dictionary<string, dynamic>>();

            //Days = Fields.GetValue<JArray>("days").Select(jv => (int) jv).ToArray();
            //StartTime = TimeSpan.Parse(Fields.GetValue<string>("start_time")).WithoutMilliseconds();

            //Devices = new List<SchedulerDevice>();
            /*
            foreach (var device in Fields.GetValue<JArray>("devices"))
            {
                var schedulerDevice = new SchedulerDevice()
                {
                    Action = device["action"]?.ToString(),
                    DeviceId = device["deviceId"]?.ToString()
                };
                Devices.Add(schedulerDevice);
            }
            */
        }

        public Schedule(JToken json)
        {
            Fields = json.ToObject<Dictionary<string, dynamic>>();

            //Days = Fields.GetValue<JArray>("days").Select(jv => (int) jv).ToArray();
            //StartTime = TimeSpan.Parse(Fields.GetValue<string>("start_time")).WithoutMilliseconds();

            //Devices = new List<SchedulerDevice>();
            /*
            foreach (var device in Fields.GetValue<JArray>("devices"))
            {
                var schedulerDevice = new SchedulerDevice()
                {
                    Action = device["action"]?.ToString(),
                    DeviceId = device["deviceId"]?.ToString()
                };
                Devices.Add(schedulerDevice);
            }
            */
        }

        [BsonElement("_id")]
        [JsonProperty("id")]
        //[BsonElement("id")]
        public string Id => Fields.GetValue<string>("id");

        JObject IDocument.ToJObject => ToJObject();

        public JObject ToJObject()
        {
            return JObject.FromObject(Fields);
        }

        public bool ShouldRun(TimeSpan? now = null)
        {
            now ??= DateTime.Now.TimeOfDay.WithoutMilliseconds();

            //Console.WriteLine($"comparing {StartTime} to now {now}");

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