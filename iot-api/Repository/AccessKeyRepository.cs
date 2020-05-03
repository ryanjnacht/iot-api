using System;
using System.Collections.Generic;
using iot_api.Security;
using Newtonsoft.Json.Linq;

namespace iot_api.Repository
{
    public static class AccessKeyRepository
    {
        private const string CollectionName = "accessKeys";

        public static void Add(JObject json)
        {
            var id = json["id"]?.ToString();

            if (DataAccess.Get(CollectionName, id) != null)
                throw new Exception($"cannot add device '{id}': access key already exists");

            DataAccess.Insert(CollectionName, json);
        }

        public static void Delete(string id)
        {
            DataAccess.Delete(CollectionName, id);
        }

        private static AccessKey Get(JObject json)
        {
            return new AccessKey(json);
        }

        public static AccessKey Get(string id)
        {
            var json = DataAccess.Get(CollectionName, id);
            return json == null ? null : Get(json);
        }

        public static IEnumerable<AccessKey> Get()
        {
            var accessKeyList = new List<AccessKey>();

            foreach (var json in DataAccess.Get(CollectionName))
                accessKeyList.Add(Get(json));

            return accessKeyList;
        }

        public static void Clear()
        {
            DataAccess.Clear(CollectionName);
        }
    }
}