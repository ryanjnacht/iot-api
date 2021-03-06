﻿using System.Threading.Tasks;
using iot_api.Devices;
using iot_api.Repository;
using iot_api.Security;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace iot_api.Controllers
{
    [DisableCors]
    [ApiController]
    [Route("devices")]
    public class DeviceController : ControllerBase
    {
        // POST device
        [HttpPost]
        public JObject PostDevice([FromBody] JObject body, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAccessDevices(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            IDevice deviceObj = new Device(body);
            DeviceRepository.Add(deviceObj);

            deviceObj = DeviceRepository.Get(deviceObj.Id);
            return deviceObj.ToJObject();
        }

        // GET devices
        [HttpGet]
        public JArray GetDevices(string accessKey = null)
        {
            if (!AccessKeyHelper.CanAccessDevices(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            var jArray = new JArray();
            Parallel.ForEach(DeviceRepository.Get(), deviceObj =>
            {
                var jObj = deviceObj.ToJObject();
                lock (jArray)
                    jArray.Add(jObj);
            });

            return jArray;
        }

        // GET device
        [HttpGet("{id}")]
        public JObject GetDevice(string id, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAccessDevice(accessKey, id))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            var deviceObj = DeviceRepository.Get(id);

            if (deviceObj != null) return deviceObj.ToJObject();

            Response.StatusCode = StatusCodes.Status404NotFound;
            return null;
        }

        // GET device On
        [HttpGet("{id}/turnOn")]
        [HttpGet("{id}/On")]
        public JObject GetTurnOnDevice(string id, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAccessDevice(accessKey, id))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            var deviceObj = DeviceRepository.Get(id);

            if (deviceObj == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return null;
            }

            deviceObj.TurnOn();
            return deviceObj.ToJObject();
        }

        // GET device Off
        [HttpGet("{id}/turnOn")]
        [HttpGet("{id}/Off")]
        public JObject GetTurnOffDevice(string id, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAccessDevice(accessKey, id))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            var deviceObj = DeviceRepository.Get(id);

            if (deviceObj == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return null;
            }

            deviceObj.TurnOff();
            return deviceObj.ToJObject();
        }

        // GET device Toggle
        [HttpGet("{id}/Toggle")]
        public JObject GetToggleDevice(string id, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAccessDevice(accessKey, id))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            var deviceObj = DeviceRepository.Get(id);

            if (deviceObj == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return null;
            }

            deviceObj.Toggle();
            return deviceObj.ToJObject();
        }

        [HttpDelete("{id}")]
        public void DeleteDevice(string id, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAccessDevices(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var deviceObj = DeviceRepository.Get(id);
            if (deviceObj == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            DeviceRepository.Delete(deviceObj);
        }
    }
}