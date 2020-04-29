using System;
using MongoDB.Driver;

namespace iot_api
{
    public static class Configuration
    {
        public static int WebClientTimeout = 2000;
        public static string MongoDatabase = "iot-api";
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
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGO_DB")))
                MongoDatabase = Environment.GetEnvironmentVariable("MONGO_DB");

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGO_HOST")))
                _mongoHost = Environment.GetEnvironmentVariable("MONGO_HOST");

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGO_PORT")))
                int.TryParse(Environment.GetEnvironmentVariable("MONGO_PORT"), out _mongoPort);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TIMEOUT")))
                int.TryParse(Environment.GetEnvironmentVariable("TIMEOUT"), out WebClientTimeout);
        }
    }
}