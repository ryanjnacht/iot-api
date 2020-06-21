using System;
using iot_api.DataAccess;
using iot_api.Repository;
using iot_api.Security;
using NUnit.Framework;
using NUnit.Framework.Internal.Commands;

namespace iot_api_tests
{
    [SetUpFixture]
    public class TestSetupTeardown
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
            Environment.SetEnvironmentVariable("MONGO_DB", "iot-api-test");
            Environment.SetEnvironmentVariable("MONGO_HOST", "localhost");
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