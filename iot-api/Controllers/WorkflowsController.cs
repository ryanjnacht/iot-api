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
    [Route("workflows")]
    public class WorkflowsController : ControllerBase
    {
        [HttpPost]
        public JObject PostWorkflow([FromBody] JObject body, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAccessWorkflows(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            WorkflowRepository.Add(body);

            var id = body["id"]?.ToString();
            return WorkflowRepository.Get(id).ToJObject();
        }

        [HttpDelete("{id}")]
        public void DeleteWorkflow(string id, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAccessWorkflows(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var workflowObj = WorkflowRepository.Get(id);
            if (workflowObj == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            WorkflowRepository.Delete(id);
        }

        [HttpGet]
        public JArray GetWorkflows(string accessKey = null)
        {
            if (!AccessKeyHelper.CanAccessWorkflows(accessKey))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            var jArray = new JArray();

            foreach (var workflowObj in WorkflowRepository.Get())
                jArray.Add(workflowObj.ToJObject());

            return jArray;
        }

        [HttpGet("{id}")]
        public JObject GetWorkflow(string id, string accessKey = null)
        {
            if (!AccessKeyHelper.CanAccessWorkflow(accessKey, id))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            return WorkflowRepository.Get(id).ToJObject();
        }

        [HttpGet("{*url}", Order = int.MaxValue)]
        public JObject CatchAll(string accessKey = null)
        {
            var route = Request.Path.Value.ToLower().Split("/");
            var workflowId = route[2];
            var command = route[3];

            if (!AccessKeyHelper.CanAccessWorkflow(accessKey, workflowId))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            var workflowObj = WorkflowRepository.Get(workflowId);

            if (workflowObj == null)
            {
                Response.StatusCode = 404;
                var jObj = new JObject {{"route", string.Join("/", route)}};
                return jObj;
            }

            switch (command)
            {
                case "run":
                {
                    workflowObj.Run();
                    Response.StatusCode = StatusCodes.Status200OK;
                    return new JObject {{"status", "completed"}};
                }
                case "cancel":
                {
                    Response.StatusCode = StatusCodes.Status200OK;
                    workflowObj.Stop();
                    return new JObject {{"status", "cancelled"}};
                }
                default:
                    Response.StatusCode = StatusCodes.Status404NotFound;
                    return new JObject {{"route", string.Join("/", route)}};
            }
        }
    }
}