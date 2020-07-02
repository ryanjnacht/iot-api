using System;
using iot_api.Repository;
using NUnit.Framework;

namespace iot_api_tests
{
    [SetUpFixture]
    public class TestSetupTeardown
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

            //if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGO_HOST")))
                //Environment.SetEnvironmentVariable("MONGO_HOST", "localhost");

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGO_DB")))
                Environment.SetEnvironmentVariable("MONGO_DB", "iot-api-test");
        }

        //[OneTimeTearDown]
        public void TearDown()
        {
            DeviceRepository.Clear();
            AccessKeyRepository.Clear();
            WorkflowRepository.Clear();
            RulesRepository.Clear();
            ScheduleRepository.Clear();
        }
    }
}