using System;
using System.Collections.Generic;
using iot_api.Devices;
using iot_api.Rules;
using iot_api.Scheduler;
using iot_api.Security;
using iot_api.Workflows;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace iot_api.DataAccess
{
    public static class MongoProvider<T> where T : IDocument
    {
        private static IMongoDatabase MongoDatabase =>
            MongoClient.GetDatabase(Configuration.Configuration.MongoDatabase);

        private static string CollectionName
        {
            get
            {
                if (typeof(T) == typeof(AccessKey))
                    return "accessKeys";

                if (typeof(T) == typeof(IDevice))
                    return "devices";

                if (typeof(T) == typeof(Rule))
                    return "rules";

                if (typeof(T) == typeof(Schedule))
                    return "schedules";

                if (typeof(T) == typeof(Workflow))
                    return "workflows";

                throw new NotImplementedException();
            }
        }

        private static MongoClient MongoClient => new MongoClient(Configuration.Configuration.MongoDbUrl);

        public static void Insert(T documentObj)
        {
            var json = documentObj.ToJObject;
            var document = BsonDocument.Parse(json.ToString());
            MongoDatabase.GetCollection<BsonDocument>(CollectionName).InsertOne(document);
        }

        public static void Replace(T documentObj)
        {
            var collection = MongoDatabase.GetCollection<T>(CollectionName);
            var filterDefinition = Builders<T>.Filter.Eq("id", documentObj.Id);
            collection.ReplaceOne(filterDefinition, documentObj);
        }

        public static JObject GetRecord(string id)
        {
            var collection = MongoDatabase.GetCollection<BsonDocument>(CollectionName);
            var filter = Builders<BsonDocument>.Filter.Eq("id", id);
            var document = collection.Find(filter).FirstOrDefault();

            if (document == null)
                return default;

            document.Remove("_id"); //remove bson id
            return JObject.Parse(document.ToString());
        }

        public static List<JObject> GetRecords()
        {
            var collection = MongoDatabase.GetCollection<BsonDocument>(CollectionName);
            var documentList = collection.Find(_ => true).ToList();

            var jObjectList = new List<JObject>();

            foreach (var document in documentList)
            {
                document.Remove("_id"); //remove bson id
                jObjectList.Add(JObject.Parse(document.ToString()));
            }

            return jObjectList;
        }

        public static void Delete(string id)
        {
            var collection = MongoDatabase.GetCollection<BsonDocument>(CollectionName);
            var filter = Builders<BsonDocument>.Filter.Eq("id", id);
            collection.DeleteMany(filter);
        }

        public static void Clear()
        {
            var collection = MongoDatabase.GetCollection<BsonDocument>(CollectionName);
            collection.DeleteMany(_ => true);
        }
    }
}