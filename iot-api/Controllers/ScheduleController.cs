//#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using iot_api.Extensions;
using iot_api.Repository;
using iot_api.Scheduler;
using iot_api.Security;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

//TODO: end_time could produce second schedule record
//TODO: schedule workflows

namespace iot_api.Controllers
{
    [DisableCors]
    [ApiController]
    [Route("schedules")]
    public class ScheduleController : ControllerBase
    {
        [HttpGet]
        public JArray GetSchedules(string accessKey = null)
        {
            if (!AccessKeyHelper.CanAdminSchedules(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            var jArray = new JArray();
            foreach (var scheduleObj in ScheduleRepository.Get())
            {
                var jObj = scheduleObj.ToJObject();
                jArray.Add(jObj);
            }

            return jArray;
        }

        [HttpPost]
        public Schedule CreateSchedule([FromBody] JObject json, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAdminSchedules(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            var startTime = json["start_time"]?.ToString();

            if (string.IsNullOrEmpty(startTime))
                throw new Exception("start_time is required");

            startTime = TimeSpan.Parse(startTime).WithoutMilliseconds().ToString();

            var jObj = new JObject {{"start_time", startTime}, {"days", new JArray()}};

            if (json["days"] != null)
                try
                {
                    var days = ((JArray) json["days"]!).Select(jv => (int) jv).ToArray();
                    jObj["days"] = JToken.FromObject(days);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw new Exception($"could not parse days: {ex.Message}");
                }

            if (json["devices"] == null)
                throw new Exception("devices is required but was not defined");

            var devices = json["devices"]?.Value<JArray>().ToObject<List<SchedulerDevice>>();

            if (devices == null || !devices.Any())
                throw new Exception("could not deserialize devices");

            jObj.Add("devices", JArray.FromObject(devices));
            var scheduleObj = new Schedule(jObj);

            ScheduleRepository.Insert(scheduleObj);
            return ScheduleRepository.Get(scheduleObj.Id);
        }

        [HttpDelete("{id}")]
        public void Delete(string id, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAdminSchedules(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var scheduleObj = ScheduleRepository.Get(id);
            ScheduleRepository.DeleteRecord(scheduleObj);
        }
    }
}