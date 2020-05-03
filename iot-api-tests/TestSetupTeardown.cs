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
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            DeviceRepository.Clear();
            AccessKeyRepository.Clear();
            WorkflowRepository.Clear();
            RulesRepository.Clear();
        }
    }
}