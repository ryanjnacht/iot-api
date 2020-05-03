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

        private static DataAccess _dataAccessObj;


        private static readonly string[] AllowedProperties = {"id", "type", "ipAddress"};
        private static readonly string[] SupportedTypes = {"dummy", "tasmota", "hs1xx", "hs100", "hs110"};

        private static DataAccess DataAccessObjObj => _dataAccessObj ??= new DataAccess(CollectionName);

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
            if (DataAccessObjObj.Get(id) != null)
                throw new Exception($"cannot add device '{id}': a device with this id already exists");

            //require a supported type
            var type = json["type"]?.ToString().ToLower();
            if (string.IsNullOrEmpty(type) || !SupportedTypes.Contains(type))
                throw new Exception("cannot add device: type not specified");

            DataAccessObjObj.Insert(json);
        }

        public static void Delete(string id)
        {
            DataAccessObjObj.Delete(id);
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
            var json = DataAccessObjObj.Get(id);
            return json == null ? null : Get(json);
        }

        public static IEnumerable<IDevice> Get()
        {
            var deviceList = new List<IDevice>();

            foreach (var json in DataAccessObjObj.Get())
                deviceList.Add(Get(json));

            return deviceList;
        }

        public static void Clear()
        {
            DataAccessObjObj.Clear();
        }
    }
}