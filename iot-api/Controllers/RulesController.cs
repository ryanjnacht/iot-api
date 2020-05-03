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
    [Route("rules")]
    public class RulesController : ControllerBase
    {
        [HttpGet]
        public JArray GetRules(string accessKey = null)
        {
            if (!AccessKeyHelper.CanAdminRules(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            var jArray = new JArray();
            foreach (var ruleObj in RulesRepository.Get())
            {
                var jObj = ruleObj.ToJObject();
                jArray.Add(jObj);
            }

            return jArray;
        }

        [HttpPost]
        public JObject PostRule([FromBody] JObject body, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAdminRules(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            RulesRepository.Add(body);
            var id = body["id"]?.ToString();

            return RulesRepository.Get(id).ToJObject();
        }

        [HttpDelete("{id}")]
        public void DeleteRule(string id, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAdminRules(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var deviceObj = RulesRepository.Get(id);
            if (deviceObj == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            RulesRepository.Delete(id);
        }
    }
}