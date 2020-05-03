using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace iot_api
{
    public interface IDataAccess
    {
        void Insert(JObject jObject);
        JObject Get(string id);
        IEnumerable<JObject> Get();
        void Delete(string id);
        void Clear();
    }
}