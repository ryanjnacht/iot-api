using System.Collections.Generic;
using iot_api.DataAccess;
using iot_api.Devices;
using Newtonsoft.Json.Linq;

namespace iot_api.Repository
{
    public static class DeviceRepository
    {
        private static readonly string[] AllowedProperties = {"id", "type", "ipAddress"};
        private static readonly string[] SupportedTypes = {"dummy", "tasmota", "hs1xx", "hs100", "hs110"};

        public static void Add(IDevice deviceObj)
        {
            DataAccess<IDevice>.Insert(deviceObj);
            /*
            
            
            //sanitize the json
            foreach (var key in json.Properties().ToList().Where(key => !AllowedProperties.Contains(key.Name)))
                json.Remove(key.Name);

            //require an id
            var id = json["id"]?.ToString();
            if (string.IsNullOrEmpty(id))
                throw new Exception("cannot add device: device id is required");

            //require unique id
            
            if (DataAccess.DataAccess<Device>.Get(id) != null)
                throw new Exception($"cannot add device '{id}': a device with this id already exists");

            //require a supported type
            var type = json["type"]?.ToString().ToLower();
            if (string.IsNullOrEmpty(type) || !SupportedTypes.Contains(type))
                throw new Exception("cannot add device: type not specified");

            var deviceObj = new Device(json);
            DataAccess<Device>.Insert(deviceObj);
            */
        }

        public static void Delete(IDevice deviceObj)
        {
            DataAccess<IDevice>.Delete(deviceObj.Id);
        }

        private static IDevice Get(JObject json)
        {
            var type = json["type"]?.ToString().ToLower();

            if (type == "tasmota")
                return new Tasmota(json);

            if (type == "hs1xx" || type == "hs100" || type == "hs110")
                return new TPLinkSmartPlugHS1xx(json);

            return new Device(json);
        }

        public static IDevice Get(string id)
        {
            var json = DataAccess<IDevice>.Get(id);
            if (json == null) return null;

            //var json = JObject.FromObject();
            return Get(json);
        }

        public static IEnumerable<IDevice> Get()
        {
            var deviceList = new List<IDevice>();

            foreach (var doc in DataAccess<IDevice>.Get())
            {
                deviceList.Add(Get(doc));
            }

            return deviceList;
        }

        public static void Clear()
        {
            DataAccess<IDevice>.Clear();
        }
    }
}