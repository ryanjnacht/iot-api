using System;
using System.Collections.Generic;
using System.ComponentModel;
using iot_api.Extensions;
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

        protected Dictionary<string, dynamic> Fields { get; }
        public string IpAddress => Fields.GetValue<string>("ipAddress");
        public string Id => Fields.GetValue<string>("id");
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