using System;
using System.Collections.Generic;
using iot_api.Security;
using Newtonsoft.Json.Linq;

namespace iot_api.Repository
{
    public static class AccessKeyRepository
    {
        private const string CollectionName = "accessKeys";

        private static DataAccess _dataAccessObj;

        private static DataAccess DataAccessObjObj => _dataAccessObj ??= new DataAccess(CollectionName);

        public static void Add(JObject json)
        {
            var id = json["id"]?.ToString();

            if (DataAccessObjObj.Get(id) != null)
                throw new Exception($"cannot add device '{id}': access key already exists");

            DataAccessObjObj.Insert(json);
        }

        public static void Delete(string id)
        {
            DataAccessObjObj.Delete(id);
        }

        private static AccessKey Get(JObject json)
        {
            return new AccessKey(json);
        }

        public static AccessKey Get(string id)
        {
            var json = DataAccessObjObj.Get(id);
            return json == null ? null : Get(json);
        }

        public static IEnumerable<AccessKey> Get()
        {
            var accessKeyList = new List<AccessKey>();

            foreach (var json in DataAccessObjObj.Get())
                accessKeyList.Add(Get(json));

            return accessKeyList;
        }

        public static void Clear()
        {
            DataAccessObjObj.Clear();
        }
    }
}