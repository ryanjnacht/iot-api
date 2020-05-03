using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace iot_api
{
    internal class MemoryObject
    {
        public string CollectionName;
        public JObject JObject;
    }

    public class MemoryDataAccess : IDataAccess
    {
        private static List<MemoryObject> _memoryObjects;
        private readonly string _collectionName;

        public MemoryDataAccess(string collectionName)
        {
            _collectionName = collectionName;
            if (_memoryObjects == null) _memoryObjects = new List<MemoryObject>();
        }

        public void Insert(JObject jObject)
        {
            var memoryObj = new MemoryObject {CollectionName = _collectionName, JObject = jObject};
            _memoryObjects.Add(memoryObj);
        }

        public JObject Get(string id)
        {
            return _memoryObjects
                .FirstOrDefault(x => x.CollectionName == _collectionName && x.JObject["id"]?.ToString() == id)?.JObject;
        }

        public IEnumerable<JObject> Get()
        {
            return _memoryObjects.FindAll(x => x.CollectionName == _collectionName)
                .Select(memoryObj => memoryObj.JObject).ToList();
        }

        public void Delete(string id)
        {
            _memoryObjects.RemoveAll(x => x.CollectionName == _collectionName && x.JObject["id"]?.ToString() == id);
        }

        public void Clear()
        {
            _memoryObjects.RemoveAll(x => x.CollectionName == _collectionName);
        }
    }
}