using System;
using System.Linq;
using iot_api.Repository;
using iot_api.Security;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace iot_api
{
    public static class Configuration
    {
        public static int WebClientTimeout = 2000;
        public static string MongoDatabase = "iot-api";
        public static bool SecurityEnabled = true;
        private static string _mongoHost = "localhost";
        private static int _mongoPort = 27017;

        public static MongoUrl MongoDbUrl =>
            new MongoUrlBuilder
            {
                ApplicationName = $"{MongoDatabase}_app",
                Server = new MongoServerAddress(_mongoHost, _mongoPort)
            }.ToMongoUrl();

        public static void Load()
        {
            var isTesting = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test";

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGO_DB")))
                MongoDatabase = Environment.GetEnvironmentVariable("MONGO_DB");

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGO_HOST")))
                _mongoHost = Environment.GetEnvironmentVariable("MONGO_HOST");

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGO_PORT")))
                int.TryParse(Environment.GetEnvironmentVariable("MONGO_PORT"), out _mongoPort);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TIMEOUT")))
                int.TryParse(Environment.GetEnvironmentVariable("TIMEOUT"), out WebClientTimeout);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SECURITY")))
            {
                int.TryParse(Environment.GetEnvironmentVariable("SECURITY"), out var val);
                if (val == 0) SecurityEnabled = false;
            }

            if (!SecurityEnabled)
                Console.WriteLine("[Configuration] Security has been disabled!");

            if (SecurityEnabled && !AccessKeyRepository.Get().Any())
            {
                var json = new JObject {{"name", "default admin"}, {"admin", true}};

                var accessKeyObj = new AccessKey(json);
                AccessKeyRepository.Add(accessKeyObj.ToJObject());

                Console.WriteLine($"[Configuration] Security is enabled. Default admin access key: {accessKeyObj.Id}");

                if (isTesting)
                    Environment.SetEnvironmentVariable("ACCESSKEY", accessKeyObj.Id);
            }
        }
    }
}