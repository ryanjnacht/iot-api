using iot_api.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace iot_api.Controllers
{
    [ApiController]
    [DisableCors]
    [Route("workflows")]
    public class WorkflowsController : ControllerBase
    {
        //TODO: access keys

        [HttpPost]
        public JObject PostWorkflow([FromBody] JObject body)
        {
            WorkflowRepository.Add(body);

            var id = body["id"]?.ToString();
            return WorkflowRepository.Get(id).ToJObject();
        }

        [HttpDelete("{id}")]
        public void DeleteWorkflow(string id)
        {
            var workflowObj = WorkflowRepository.Get(id);
            if (workflowObj == null)
            {
                Response.StatusCode = 404;
                return;
            }

            WorkflowRepository.Delete(id);
        }

        [HttpGet]
        public JArray GetWorkflows()
        {
            var jArray = new JArray();

            foreach (var workflowObj in WorkflowRepository.Get())
                jArray.Add(workflowObj.ToJObject());

            return jArray;
        }

        [HttpGet("{*url}", Order = int.MaxValue)]
        public JObject CatchAll()
        {
            var route = Request.Path.Value.ToLower().Split("/");
            var workflowId = route[2];
            var command = route[3];

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
                    Response.StatusCode = 200;
                    return new JObject {{"status", "completed"}};
                }
                case "stop":
                {
                    Response.StatusCode = 200;
                    workflowObj.Stop();
                    return new JObject {{"status", "cancelled"}};
                }
                default:
                    Response.StatusCode = 404;
                    return new JObject {{"route", string.Join("/", route)}};
            }
        }
    }
}