using System.Collections.Generic;
using iot_api.DataAccess;
using iot_api.Security;

namespace iot_api.Repository
{
    public static class AccessKeyRepository
    {
        public static void Add(AccessKey accessKeyObj)
        {
            DataAccess<AccessKey>.Insert(accessKeyObj);
        }

        public static void Delete(AccessKey accessKeyObj)
        {
            DataAccess<AccessKey>.Delete(accessKeyObj.Id);
        }

        public static AccessKey Get(string id)
        {
            var json = DataAccess<AccessKey>.Get(id);
            return new AccessKey(json);
        }

        public static IEnumerable<AccessKey> Get()
        {
            var accessKeyList = new List<AccessKey>();

            foreach (var json in DataAccess<AccessKey>.Get())
                accessKeyList.Add(new AccessKey(json));

            return accessKeyList;
        }

        public static void Clear()
        {
            DataAccess<AccessKey>.Clear();
        }
    }
}