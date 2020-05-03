using System;
using System.Collections.Generic;
using System.Linq;
using iot_api.Extensions;
using Newtonsoft.Json.Linq;

namespace iot_api.Security
{
    public class AccessKey
    {
        public AccessKey(JToken json)
        {
            Fields = json.ToObject<Dictionary<string, dynamic>>();

            if (string.IsNullOrEmpty(Name))
                throw new Exception("'Name' is required");

            if (Devices == null)
                Fields.AddOrUpdate("devices", new JArray());

            if (Workflows == null)
                Fields.AddOrUpdate("workflows", new JArray());

            if (string.IsNullOrEmpty(Id))
                Id = Guid.NewGuid().ToString().Replace("-", "");
        }

        public string Id
        {
            get => Fields.GetValue<string>("id");
            private set => Fields.AddOrUpdate("id", value);
        }

        private bool Admin => Fields.GetValue<bool>("admin");
        private string Name => Fields.GetValue<string>("name");
        private JArray Devices => Fields.GetValue<JArray>("devices");

        private JArray Workflows => Fields.GetValue<JArray>("workflows");

        private Dictionary<string, dynamic> Fields { get; }

        public bool CanAccessDevice(string deviceId)
        {
            return Admin || Devices.Values().Contains(deviceId);
        }

        public bool CanAccessWorkflow(string workflowId)
        {
            return Admin || Workflows.Values().Contains(workflowId);
        }

        public bool IsAdmin()
        {
            return Admin;
        }

        public JObject ToJObject()
        {
            JArray devices = null;
            if (JArray.FromObject(Devices).Any())
                devices = JArray.FromObject(Devices);

            JArray workflows = null;
            if (JArray.FromObject(Workflows).Any())
                workflows = JArray.FromObject(Workflows);

            var jObj = new JObject
            {
                {"id", Id},
                {"name", Name},
                {"admin", Admin},
                {"devices", devices},
                {"workflows", workflows}
            };

            return jObj;
        }
    }
}