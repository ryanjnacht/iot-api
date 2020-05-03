using System;
using System.Collections.Generic;
using System.Linq;
using iot_api.Devices;
using Newtonsoft.Json.Linq;

namespace iot_api.Repository
{
    public static class DeviceRepository
    {
        private const string CollectionName = "devices";
        private static readonly string[] AllowedProperties = {"id", "type", "ipAddress"};
        private static readonly string[] SupportedTypes = {"dummy", "tasmota", "hs1xx", "hs100", "hs110"};

        public static void Add(JObject json)
        {
            //sanitize the json
            foreach (var key in json.Properties().ToList().Where(key => !AllowedProperties.Contains(key.Name)))
                json.Remove(key.Name);

            //require an id
            var id = json["id"]?.ToString();
            if (string.IsNullOrEmpty(id))
                throw new Exception("cannot add device: device id is required");

            //require unique id
            if (DataAccess.Get(CollectionName, id) != null)
                throw new Exception($"cannot add device '{id}': a device with this id already exists");

            //require a supported type
            var type = json["type"]?.ToString().ToLower();
            if (string.IsNullOrEmpty(type) || !SupportedTypes.Contains(type))
                throw new Exception("cannot add device: type not specified");

            DataAccess.Insert(CollectionName, json);
        }

        public static void Delete(string id)
        {
            DataAccess.Delete(CollectionName, id);
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
            var json = DataAccess.Get(CollectionName, id);
            return json == null ? null : Get(json);
        }

        public static IEnumerable<IDevice> Get()
        {
            var deviceList = new List<IDevice>();

            foreach (var json in DataAccess.Get(CollectionName))
                deviceList.Add(Get(json));

            return deviceList;
        }

        public static void Clear()
        {
            DataAccess.Clear(CollectionName);
        }
    }
}