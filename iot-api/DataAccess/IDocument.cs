using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace iot_api.DataAccess
{
    public interface IDocument
    { 
        [BsonElement("id")]
        string Id { get; }
        JObject ToJObject { get; }
    }
}