using iot_api.Repository;
using iot_api.Security;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace iot_api.Controllers
{
    [ApiController]
    [DisableCors]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public JObject CreateAccessKey([FromBody] JObject body, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAdminAccessKeys(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            var name = body["name"]?.ToString();
            if (string.IsNullOrEmpty(name))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return new JObject {"error", "'name' is required"};
            }

            var accessKeyObj = new AccessKey(body);
            AccessKeyRepository.Add(accessKeyObj.ToJObject());

            return AccessKeyRepository.Get(accessKeyObj.Id).ToJObject();
        }

        [HttpGet]
        public JArray Get(string accessKey)
        {
            if (!AccessKeyHelper.CanAdminAccessKeys(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            var jArray = new JArray();
            foreach (var accessKeyObj in AccessKeyRepository.Get())
                jArray.Add(accessKeyObj.ToJObject());

            return jArray;
        }

        [HttpDelete("{id}")]
        public void DeleteAccessKey(string id, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAdminAccessKeys(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var accessKeyObj = AccessKeyRepository.Get(id);
            if (accessKeyObj != null) AccessKeyRepository.Delete(id);

            Response.StatusCode = StatusCodes.Status200OK;
        }
    }
}