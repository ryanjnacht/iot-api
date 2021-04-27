using System;
using System.Linq;
using iot_api.DataAccess;
using iot_api.Devices;
using iot_api.Repository;
using iot_api.Rules;
using iot_api.Scheduler;
using iot_api.Security;
using iot_api.Workflows;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace iot_api.Configuration
{
    public static class Configuration
    {
        private static int _timeout = 2000;
        public static readonly int WebClientTimeout = _timeout;
        private static int _retryDelay = 1000;
        public static readonly int WebClientRetryDelay = _retryDelay;
        public static string MongoDatabase = "iot-api";
        public static bool SecurityEnabled = true;
        private static string _mongoHost;
        private static int _mongoPort = 27017;
        public static int DeviceRetries = 10;
        public static bool UseCache = true;
        private static int MongoClientTimeout => _timeout / 1000;
        public static bool UseMongo => !string.IsNullOrEmpty(_mongoHost);

        public static MongoUrl MongoDbUrl =>
            new MongoUrlBuilder
            {
                ApplicationName = $"{MongoDatabase}_app",
                Server = new MongoServerAddress(_mongoHost, _mongoPort),
                ConnectTimeout = new TimeSpan(0, 0, MongoClientTimeout),
                SocketTimeout = new TimeSpan(0, 0, MongoClientTimeout),
                ServerSelectionTimeout = new TimeSpan(0, 0, MongoClientTimeout)
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
                int.TryParse(Environment.GetEnvironmentVariable("TIMEOUT"), out _timeout);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RETRY_DELAY")))
                int.TryParse(Environment.GetEnvironmentVariable("RETRY_DELAY"), out _retryDelay);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SECURITY")))
            {
                int.TryParse(Environment.GetEnvironmentVariable("SECURITY"), out var val);
                if (val == 0) SecurityEnabled = false;
            }

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DEVICE_RETRIES")))
                int.TryParse(Environment.GetEnvironmentVariable("DEVICE_RETRIES"), out DeviceRetries);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CACHING")))
                if (int.TryParse(Environment.GetEnvironmentVariable("CACHING"), out var val) && val == 0)
                    UseCache = false;

            if (UseMongo)
                Console.WriteLine($"[Configuration] Configured to use mongo @ {MongoDbUrl}");
            else
                Console.WriteLine(
                    "[Configuration] Configured to use in-memory storage. Data will not persist across application restarts");

            if (UseCache || !UseMongo)
            {
                DataAccess<AccessKey>.Initialize();
                DataAccess<IDevice>.Initialize();
                DataAccess<Rule>.Initialize();
                DataAccess<Schedule>.Initialize();
                DataAccess<Workflow>.Initialize();
            }

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RESET_AUTH")))
                if (int.TryParse(Environment.GetEnvironmentVariable("RESET_AUTH"), out var val) && val == 1)
                {
                    Console.WriteLine("[auth reset flag set! clearing access keys!");
                    DataAccess<AccessKey>.Clear();
                }

            if (SecurityEnabled && !AccessKeyRepository.Get().Any())
            {
                var json = new JObject {{"name", "default admin"}, {"admin", true}};

                var accessKeyObj = new AccessKey(json);
                AccessKeyRepository.Add(accessKeyObj);

                Console.WriteLine($"[Configuration] Security is enabled. Default admin access key: {accessKeyObj.Id}");
            }

            if (isTesting && SecurityEnabled)
            {
                var accessKeyObj = AccessKeyRepository.Get().First(x => x.IsAdmin());
                Environment.SetEnvironmentVariable("ACCESSKEY", accessKeyObj.Id);
            }
        }
    }
}