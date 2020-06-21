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
            [Description("on")] On,
            [Description("off")] Off,
            [Description("unavailable")] Unavailable,
            [Description("unknown")] Unknown
        }

        public Device(JToken json)
        {
            Fields = json.ToObject<Dictionary<string, dynamic>>();
        }

        [BsonElement("fields")]
        public Dictionary<string, dynamic> Fields { get; }
        public string IpAddress => Fields.GetValue<string>("ipAddress");
        
        [BsonElement("id")]
        public string Id
        {
            get => Fields.GetValue<string>("id");
            private set => Fields?.AddOrUpdate("id", value);
        }
        
        public virtual DeviceStatuses DeviceStatus => DeviceStatuses.Unknown;

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

        JObject IDocument.ToJObject => ToJObject();

        public JObject ToJObject()
        {
            var jObj = JObject.FromObject(Fields);

            if (jObj["deviceStatus"] != null)
                jObj.Remove("deviceStatus");

            jObj.Add("deviceStatus", DeviceStatus.GetDescription());
            return jObj;
        }
    }
}