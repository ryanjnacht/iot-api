using iot_api.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace iot_api.Controllers
{
    [DisableCors]
    [ApiController]
    [Route("devices")]
    public class DevicesController : ControllerBase
    {
        // POST device
        [HttpPost]
        public JObject PostDevice([FromBody] JObject body)
        {
            DeviceRepository.Add(body);

            var id = body["id"]?.ToString();
            return DeviceRepository.Get(id).ToJObject();
        }

        // GET devices
        [HttpGet]
        public JArray GetDevices()
        {
            var jArray = new JArray();
            foreach (var deviceObj in DeviceRepository.Get())
            {
                var jObj = deviceObj.ToJObject();
                jArray.Add(jObj);
            }

            return jArray;
        }

        // GET device
        [HttpGet("{id}")]
        public JObject GetDevice(string id)
        {
            var deviceObj = DeviceRepository.Get(id);

            if (deviceObj != null) return deviceObj.ToJObject();

            Response.StatusCode = 404;
            return null;
        }

        // GET device On
        [HttpGet("{id}/On")]
        public JObject GetTurnOnDevice(string id)
        {
            var deviceObj = DeviceRepository.Get(id);

            if (deviceObj == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            deviceObj.TurnOn();
            return deviceObj.ToJObject();
        }

        // GET device Off
        [HttpGet("{id}/Off")]
        public JObject GetTurnOffDevice(string id)
        {
            var deviceObj = DeviceRepository.Get(id);

            if (deviceObj == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            deviceObj.TurnOff();
            return deviceObj.ToJObject();
        }

        // GET device Toggle
        [HttpGet("{id}/Toggle")]
        public JObject GetToggleDevice(string id)
        {
            var deviceObj = DeviceRepository.Get(id);

            if (deviceObj == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            deviceObj.Toggle();
            return deviceObj.ToJObject();
        }

        [HttpDelete("{id}")]
        public void DeleteDevice(string id)
        {
            var deviceObj = DeviceRepository.Get(id);
            if (deviceObj == null)
            {
                Response.StatusCode = 404;
                return;
            }

            DeviceRepository.Delete(id);
        }
    }
}