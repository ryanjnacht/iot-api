using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace iot_api
{
    public static class DataAccess
    {
        private static IMongoDatabase MongoDatabase => MongoClient.GetDatabase(Configuration.MongoDatabase);
        private static MongoClient MongoClient => new MongoClient(Configuration.MongoDbUrl);

        public static void Insert(string collectionName, JObject jObject)
        {
            var document = BsonDocument.Parse(jObject.ToString());
            MongoDatabase.GetCollection<BsonDocument>(collectionName).InsertOne(document);
        }

        public static JObject Get(string collectionName, string id)
        {
            var collection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq("id", id);
            var document = collection.Find(filter).FirstOrDefault();

            if (document == null)
                return null;

            document.Remove("_id"); //
            return JObject.Parse(document.ToString());
        }

        public static IEnumerable<JObject> Get(string collectionName)
        {
            var collection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
            var documentList = collection.Find(_ => true).ToList();

            var jObjectList = new List<JObject>();

            foreach (var document in documentList)
            {
                document.Remove("_id"); //
                jObjectList.Add(JObject.Parse(document.ToString()));
            }

            return jObjectList;
        }

        public static void Delete(string collectionName, string id)
        {
            var collection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq("id", id);
            collection.DeleteMany(filter);
        }
    }
}