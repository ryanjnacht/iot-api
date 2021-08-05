using System;
using System.Collections.Generic;
using System.ComponentModel;
using iot_api.DataAccess;
using iot_api.Extensions;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace iot_api.Devices
{
    public class Device : IDevice
    {
        public enum DeviceStatuses
        {
            [Description("disabled")] Disabled,
            [Description("on")] On,
            [Description("off")] Off,
            [Description("unavailable")] Unavailable,
            [Description("unknown")] Unknown
        }

        public Device(JToken json)
        {
            Fields = json.ToObject<Dictionary<string, dynamic>>();
        }

        public virtual DeviceStatuses GetStatus()
        {
            return DeviceStatuses.Unknown;
        }

        [BsonElement("fields")] public Dictionary<string, dynamic> Fields { get; }

        public bool Disabled
        {
            get => Fields.GetValue<bool>("disabled");
            set => Fields.AddOrUpdate("disabled", value);
        }

        public string IpAddress => Fields.GetValue<string>("ipAddress");
        public string Type => Fields.GetValue<string>("type");

        [BsonElement("id")]
        public string Id
        {
            get => Fields.GetValue<string>("id");
            private set => Fields?.AddOrUpdate("id", value);
        }

        public DeviceStatuses DeviceStatus => GetStatus();

        public virtual void TurnOn()
        {
            throw new NotImplementedException();
        }

        public virtual void TurnOff()
        {
            throw new NotImplementedException();
        }

        public virtual void Toggle()
        {
            throw new NotImplementedException();
        }

        JObject IDocument.ToJObject => new JObject
        {
            {"id", Id},
            {"ipAddress", IpAddress},
            {"type", Type},
            {"disabled", Disabled}
        };

        public JObject ToJObject()
        {
            return
                new JObject
                {
                    {"id", Id},
                    {"ipAddress", IpAddress},
                    {"type", Type},
                    {"deviceStatus", DeviceStatus.GetDescription()}
                };
        }
    }
}