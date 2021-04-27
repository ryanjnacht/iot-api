using System;
using System.Globalization;
using FluentAssertions;
using iot_api.Rules;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace iot_api_tests
{
    public class RulesTests
    {
        [Test]
        public void TimeRuleShouldRun()
        {
            var ruleId = "TimeRuleShouldRun";

            var startTime = DateTime.Now.AddMinutes(-5).ToString("HH:mm");
            var endTime = DateTime.Now.AddMinutes(5).ToString("HH:mm");

            var json = new JObject {{"id", ruleId}, {"type", "time"}, {"start_time", startTime}, {"end_time", endTime}};

            var ruleObj = new Rule(json);
            ruleObj.ShouldRun().Should().BeTrue();
        }

        [Test]
        public void TimeRuleShouldNotRun()
        {
            var ruleId = "TimeRuleShouldNotRun";

            var startTime = DateTime.Now.AddMinutes(-5).ToString("HH:mm");
            var endTime = DateTime.Now.AddMinutes(-1).ToString("HH:mm");
            Console.WriteLine($"endTime: {endTime}");

            var json = new JObject {{"id", ruleId}, {"type", "time"}, {"start_time", startTime}, {"end_time", endTime}};

            var ruleObj = new Rule(json);
            ruleObj.ShouldRun().Should().BeFalse();
        }

        [Test]
        public void DayOfWeekShouldRun()
        {
            var ruleId = "DayOfWeekShouldRun";

            var dayToAllow = (int) DateTime.Now.DayOfWeek;
            var days = new JArray {dayToAllow};

            var json = new JObject {{"id", ruleId}, {"type", "dayOfWeek"}, {"days", days}};

            var ruleObj = new Rule(json);
            ruleObj.ShouldRun().Should().BeTrue();
        }

        [Test]
        public void DayOfWeekShouldNotRun()
        {
            var ruleId = "DayOfWeekShouldNotRun";

            var dayToAllow = (int) DateTime.Now.AddDays(-1).DayOfWeek;
            var days = new JArray {dayToAllow};

            var json = new JObject {{"id", ruleId}, {"type", "dayOfWeek"}, {"days", days}};

            var ruleObj = new Rule(json);
            ruleObj.ShouldRun().Should().BeFalse();
        }
    }
}