using System;
using System.Collections.Generic;
using FluentAssertions;
using iot_api.Scheduler;
using NUnit.Framework;

namespace iot_api_tests
{
    public class ScheduleLogicTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ValidScheduleNoDays()
        {
            var startTime = DateTime.Now.TimeOfDay.ToString();
            var scheduleObj = new Schedule(startTime, null, new List<SchedulerDevice>());
            scheduleObj.ShouldRun().Should().BeTrue();
        }

        [Test]
        public void ValidScheduleWithDays()
        {
            var startTime = DateTime.Now.TimeOfDay.ToString();
            var dayOfWeek = (int) DateTime.Now.DayOfWeek;
            int[] days = {dayOfWeek};

            var scheduleObj = new Schedule(startTime, days, new List<SchedulerDevice>());
            scheduleObj.ShouldRun().Should().BeTrue();
        }

        [Test]
        public void InvalidScheduleNoDays()
        {
            var startTime = DateTime.Now.AddMinutes(5).TimeOfDay.ToString();

            var scheduleObj = new Schedule(startTime, null, new List<SchedulerDevice>());
            scheduleObj.ShouldRun().Should().BeFalse();
        }

        [Test]
        public void InvalidValidScheduleWithDays()
        {
            var startTime = DateTime.Now.AddMinutes(-5).TimeOfDay.ToString();
            var dayOfWeek = (int) DateTime.Now.AddDays(-1).DayOfWeek;
            int[] days = {dayOfWeek};

            var scheduleObj = new Schedule(startTime, days, new List<SchedulerDevice>());
            scheduleObj.ShouldRun().Should().BeFalse();
        }
    }
}