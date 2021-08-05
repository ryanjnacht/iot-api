using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using iot_api;
using iot_api.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace iot_api_tests
{
    [TestFixture]
    public class DeviceControllerTests
    {
        [SetUp]
        public void SetupTests()
        {
            Environment.SetEnvironmentVariable("SECURITY", "0");
            Configuration.Load();

            _server = new TestServer(new WebHostBuilder().UseStartup<WebApiStartup>());
            _client = _server.CreateClient();
        }

        private HttpClient _client;
        private TestServer _server;

        [Test]
        public async Task DeviceControllerCRUDTest()
        {
            //create a new device record
            var deviceId = Guid.NewGuid().ToString();
            //var deviceId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            var json = new JObject
            {
                {"id", deviceId},
                {"ipAddress", "127.0.0.1"},
                {"type", "hs1xx"}
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("devices", payload);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseObj = JObject.Parse(responseString);

            responseObj.Should().NotBeEmpty();
            responseObj["id"]?.ToString().Should().Be(deviceId);

            //get list of devices
            response = await _client.GetAsync("devices");
            response.EnsureSuccessStatusCode();
            responseString = await response.Content.ReadAsStringAsync();

            var deviceList = JArray.Parse(responseString);
            deviceList.Count.Should().BeGreaterThan(0);
            deviceList.Any(token => token["id"]?.ToString() == deviceId).Should().BeTrue();

            //disable device
            response = await _client.GetAsync($"devices/{deviceId}/disable");
            response.EnsureSuccessStatusCode();

            //get list of devices
            response = await _client.GetAsync("devices");
            response.EnsureSuccessStatusCode();
            responseString = await response.Content.ReadAsStringAsync();
            deviceList = JArray.Parse(responseString);
            deviceList.Any(token =>
                    token["id"]?.ToString() == deviceId && token["deviceStatus"]?.ToString() == "disabled").Should()
                .BeTrue();

            //enable device
            response = await _client.GetAsync($"devices/{deviceId}/enable");
            response.EnsureSuccessStatusCode();

            //get list of devices
            response = await _client.GetAsync("devices");
            response.EnsureSuccessStatusCode();
            responseString = await response.Content.ReadAsStringAsync();
            deviceList = JArray.Parse(responseString);
            deviceList.Any(token =>
                    token["id"]?.ToString() == deviceId && token["deviceStatus"]?.ToString() != "disabled").Should()
                .BeTrue();

            //remove device
            response = await _client.DeleteAsync($"devices/{deviceId}");
            response.EnsureSuccessStatusCode();

            //get list of devices
            response = await _client.GetAsync("devices");
            response.EnsureSuccessStatusCode();
            responseString = await response.Content.ReadAsStringAsync();

            deviceList = JArray.Parse(responseString);
            deviceList.Any(token => token["id"]?.ToString() == deviceId).Should().BeFalse();
        }
    }
}