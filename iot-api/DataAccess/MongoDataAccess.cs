using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace iot_api
{
    public class MongoDataAccess : IDataAccess
    {
        private readonly string _collectionName;
        private readonly IMongoDatabase _mongoDatabase;

        public MongoDataAccess(string collectionName)
        {
            _collectionName = collectionName;
            var mongoClient = new MongoClient(Configuration.MongoDbUrl);
            _mongoDatabase = mongoClient.GetDatabase(Configuration.MongoDatabase);
        }


        public void Insert(JObject jObject)
        {
            var document = BsonDocument.Parse(jObject.ToString());
            _mongoDatabase.GetCollection<BsonDocument>(_collectionName).InsertOne(document);
        }

        public JObject Get(string id)
        {
            var collection = _mongoDatabase.GetCollection<BsonDocument>(_collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq("id", id);
            var document = collection.Find(filter).FirstOrDefault();

            if (document == null)
                return null;

            document.Remove("_id"); //remove bson id
            return JObject.Parse(document.ToString());
        }

        public IEnumerable<JObject> Get()
        {
            var collection = _mongoDatabase.GetCollection<BsonDocument>(_collectionName);
            var documentList = collection.Find(_ => true).ToList();

            var jObjectList = new List<JObject>();

            foreach (var document in documentList)
            {
                document.Remove("_id"); //remove bson id
                jObjectList.Add(JObject.Parse(document.ToString()));
            }

            return jObjectList;
        }

        public void Delete(string id)
        {
            var collection = _mongoDatabase.GetCollection<BsonDocument>(_collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq("id", id);
            collection.DeleteMany(filter);
        }

        public void Clear()
        {
            var collection = _mongoDatabase.GetCollection<BsonDocument>(_collectionName);
            collection.DeleteMany(_ => true);
        }
    }
}