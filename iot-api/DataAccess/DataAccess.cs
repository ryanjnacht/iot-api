using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace iot_api
{
    public class DataAccess : IDataAccess
    {
        private readonly IDataAccess _dataAccessObj;

        public DataAccess(string collectionName)
        {
            if (Configuration.UseMongo)
                _dataAccessObj = new MongoDataAccess(collectionName);
            else
                _dataAccessObj = new MemoryDataAccess(collectionName);
        }

        public void Insert(JObject jObject)
        {
            _dataAccessObj.Insert(jObject);
        }

        public JObject Get(string id)
        {
            return _dataAccessObj.Get(id);
        }

        public IEnumerable<JObject> Get()
        {
            return _dataAccessObj.Get();
        }

        public void Delete(string id)
        {
            _dataAccessObj.Delete(id);
        }

        public void Clear()
        {
            _dataAccessObj.Clear();
        }
    }
}