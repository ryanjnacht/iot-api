using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using iot_api;
using iot_api.Configuration;
using iot_api.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace iot_api_tests
{
    [TestFixture]
    public class AccessKeyControllerTests
    {
        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("CACHING", "1");
            Environment.SetEnvironmentVariable("SECURITY", "1");

            Configuration.Load();

            _server = new TestServer(new WebHostBuilder().UseStartup<WebApiStartup>());
            _client = _server.CreateClient();

            _defaultAccessKey = Environment.GetEnvironmentVariable("ACCESSKEY");
            Console.WriteLine($"[TEST] using access key {_defaultAccessKey}");
        }

        //[TearDown]
        public void TearDown()
        {
            DeviceRepository.Clear();
            AccessKeyRepository.Clear();
            WorkflowRepository.Clear();
            RulesRepository.Clear();
            ScheduleRepository.Clear();
        }

        private string _defaultAccessKey;

        private HttpClient _client;
        private TestServer _server;

        private async Task CreateDevice(string deviceId, string accessKey)
        {
            var json = new JObject {{"id", deviceId}, {"type", "dummy"}};
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"devices?accessKey={accessKey}", payload);
            response.EnsureSuccessStatusCode();
        }

        private async Task CreateWorkflow(string workflowId, string accessKey)
        {
            var json = new JObject {{"id", workflowId}};
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"workflows?accessKey={accessKey}", payload);
            response.EnsureSuccessStatusCode();
        }

        private async Task<string> CreateAccessKey(bool admin, string[] devices, string[] workflows)
        {
            devices ??= new string[] { };
            workflows ??= new string[] { };

            var devicesObj = JArray.FromObject(devices);
            var workflowsObj = JArray.FromObject(workflows);

            var json = new JObject
            {
                {"name", Guid.NewGuid().ToString()},
                {"admin", admin},
                {"devices", devicesObj},
                {"workflows", workflowsObj}
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"auth?accessKey={_defaultAccessKey}", payload);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseObj = JObject.Parse(responseString);

            return responseObj["id"]?.ToString();
        }

        [Test]
        public async Task AccessKeyControllerCRUDTest()
        {
            //create new access key
            var deviceId = "device-auth-test";
            var workflowId = "workflow-auth-test";

            var devicesObj = new JArray {deviceId};
            var workflowsObj = new JArray {workflowId};

            var json = new JObject
            {
                {"name", "test-access-key"},
                {"admin", false},
                {"devices", devicesObj},
                {"workflows", workflowsObj}
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"auth?accessKey={_defaultAccessKey}", payload);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseObj = JObject.Parse(responseString);

            responseObj.Should().NotBeEmpty();

            var accessKey = responseObj["id"];
            responseObj["name"]?.ToString().Should().Be(json["name"]?.ToString());

            //remove access key
            response = await _client.DeleteAsync($"auth/{accessKey}?accessKey={_defaultAccessKey}");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task AuthorizedAccessToDevice()
        {
            var deviceId = Guid.NewGuid().ToString();
            await CreateDevice(deviceId, _defaultAccessKey);

            string[] devices = {deviceId};
            var accessKey = CreateAccessKey(false, devices, null).Result;
            accessKey.Should().NotBeNullOrEmpty();

            var response = await _client.GetAsync($"devices/{deviceId}?accessKey={accessKey}");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task AuthorizedAccessToDevices()
        {
            var response = await _client.GetAsync($"devices?accessKey={_defaultAccessKey}");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task AuthorizedAccessToWorkflow()
        {
            var workflowId = Guid.NewGuid().ToString();
            await CreateWorkflow(workflowId, _defaultAccessKey);

            string[] workflows = {workflowId};
            var accessKey = CreateAccessKey(false, null, workflows).Result;
            accessKey.Should().NotBeNullOrEmpty();

            var response = await _client.GetAsync($"workflows/{workflowId}?accessKey={accessKey}");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task AuthorizedAccessToWorkflows()
        {
            var response = await _client.GetAsync($"workflows?accessKey={_defaultAccessKey}");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task CreateNewAdminAccessKeyAndGetDevicesAndWorkflows()
        {
            var json = new JObject
            {
                {"name", "test-access-key"},
                {"admin", true}
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"auth?accessKey={_defaultAccessKey}", payload);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseObj = JObject.Parse(responseString);

            responseObj.Should().NotBeEmpty();

            var accessKey = responseObj["id"];
            responseObj["name"]?.ToString().Should().Be(json["name"]?.ToString());

            response = await _client.GetAsync($"devices?accessKey={accessKey}");
            response.EnsureSuccessStatusCode();

            response = await _client.GetAsync($"workflows?accessKey={accessKey}");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task UnauthorizedAccessToDevice()
        {
            var deviceId = Guid.NewGuid().ToString();
            await CreateDevice(deviceId, _defaultAccessKey);

            var response = await _client.GetAsync($"devices/{deviceId}");
            response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Test]
        public async Task UnauthorizedAccessToDevices()
        {
            var response = await _client.GetAsync("devices");
            response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Test]
        public async Task UnauthorizedAccessToWorkflow()
        {
            var workflowId = Guid.NewGuid().ToString();
            await CreateWorkflow(workflowId, _defaultAccessKey);

            var response = await _client.GetAsync($"workflows/{workflowId}");
            response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Test]
        public async Task UnauthorizedAccessToWorkflows()
        {
            var response = await _client.GetAsync("workflows");
            response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }
    }
}