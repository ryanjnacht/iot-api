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
    public class WorkflowControllerTests
    {
        private HttpClient _client;
        private TestServer _server;
        
        [SetUp]
        public void SetupTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }
        
        [Test]
        public async Task WorkflowControllerCRUDTest()
        {
            //create a new record
            var workflowId = Guid.NewGuid().ToString();
            var json = new JObject
            {
                {"id", workflowId}
            };
            
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("workflows", payload);
            response.EnsureSuccessStatusCode();
            
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObj = JObject.Parse(responseString);
            
            responseObj.Should().NotBeEmpty();
            responseObj["id"]?.ToString().Should().Be(workflowId);
            
            //get list
            response = await _client.GetAsync("workflows");
            response.EnsureSuccessStatusCode();
            responseString = await response.Content.ReadAsStringAsync();
            
            var ruleList = JArray.Parse(responseString);
            ruleList.Count.Should().BeGreaterThan(0);
            ruleList.Any(token => token["id"]?.ToString() == workflowId).Should().BeTrue();
            
            //remove
            response = await _client.DeleteAsync($"workflows/{workflowId}");
            response.EnsureSuccessStatusCode();
            
            //get list
            response = await _client.GetAsync("workflows");
            response.EnsureSuccessStatusCode();
            responseString = await response.Content.ReadAsStringAsync();
            
            ruleList = JArray.Parse(responseString);
            ruleList.Any(token => token["id"]?.ToString() == workflowId).Should().BeFalse();
        }
    }
}