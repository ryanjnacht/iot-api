using System;
using FluentAssertions;
using iot_api.Extensions;
using NUnit.Framework;

namespace iot_api_tests
{
    [TestFixture]
    internal class DateTimeExtensionTests
    {
        [SetUp]
        public void SetupTests()
        {
        }

        [TearDown]
        public void TearDownTests()
        {
        }

        [Test]
        public void TestInBetween()
        {
            var start = new TimeSpan(2, 0, 0);
            var end = new TimeSpan(4, 30, 0);

            var testTime = new DateTime(2019, 1, 1, 22, 30, 00);
            var result = testTime.IsBetween(start, end);

            testTime = new DateTime(2019, 1, 1, 2, 30, 00);
            result = testTime.IsBetween(start, end);

            result.Should().Be(true);
        }
    }
}
