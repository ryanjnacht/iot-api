using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using iot_api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace iot_api_tests
{
    [TestFixture]
    public class RuleControllerTests
    {
        [SetUp]
        public void SetupTests()
        {
            Environment.SetEnvironmentVariable("SECURITY", "0");

            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        private HttpClient _client;
        private TestServer _server;

        [Test]
        public async Task RuleControllerCRUDTest()
        {
            //create a new record
            var ruleId = Guid.NewGuid().ToString();
            var json = new JObject
            {
                {"id", ruleId}
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("rules", payload);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseObj = JObject.Parse(responseString);

            responseObj.Should().NotBeEmpty();
            responseObj["id"]?.ToString().Should().Be(ruleId);

            //get list
            response = await _client.GetAsync("rules");
            response.EnsureSuccessStatusCode();
            responseString = await response.Content.ReadAsStringAsync();

            var ruleList = JArray.Parse(responseString);
            ruleList.Count.Should().BeGreaterThan(0);
            ruleList.Any(token => token["id"]?.ToString() == ruleId).Should().BeTrue();

            //remove
            response = await _client.DeleteAsync($"rules/{ruleId}");
            response.EnsureSuccessStatusCode();

            //get list
            response = await _client.GetAsync("rules");
            response.EnsureSuccessStatusCode();
            responseString = await response.Content.ReadAsStringAsync();

            ruleList = JArray.Parse(responseString);
            ruleList.Any(token => token["id"]?.ToString() == ruleId).Should().BeFalse();
        }
    }
}