using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace iot_api.DataAccess
{
    public static class DataAccess<T> where T : IDocument
    {
        public static List<JObject> _documentCache;

        public static void Initialize()
        {
            if (Configuration.Configuration.UseMongo && Configuration.Configuration.UseCache)
                _documentCache = MongoProvider<T>.GetRecords();

            if (!Configuration.Configuration.UseMongo)
                _documentCache = new List<JObject>();
        }

        public static void Insert(T recordObj)
        {
            if (Configuration.Configuration.UseMongo)
                MongoProvider<T>.Insert(recordObj);

            if (Configuration.Configuration.UseCache || !Configuration.Configuration.UseMongo)
                _documentCache.Add(recordObj.ToJObject);
        }

        public static JObject Get(string id)
        {
            if (Configuration.Configuration.UseCache || !Configuration.Configuration.UseMongo)
                return _documentCache.FirstOrDefault(x => x["id"]?.ToString() == id);

            return MongoProvider<T>.GetRecord(id);
        }

        public static List<JObject> Get()
        {
            if (Configuration.Configuration.UseCache || !Configuration.Configuration.UseMongo)
                return _documentCache;

            return MongoProvider<T>.GetRecords();
        }

        public static void Delete(string id)
        {
            if (Configuration.Configuration.UseCache || !Configuration.Configuration.UseMongo)
                _documentCache.RemoveAll(x => x["id"]?.ToString() == id);

            if (Configuration.Configuration.UseMongo)
                MongoProvider<T>.Delete(id);
        }

        public static void Clear()
        {
            if (Configuration.Configuration.UseCache || !Configuration.Configuration.UseMongo)
                _documentCache.Clear();

            if (Configuration.Configuration.UseMongo)
                MongoProvider<T>.Clear();
        }
    }
}