using System;
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

        //todo: add access keys
        [HttpGet("{*url}", Order = int.MaxValue)]
        public JObject CatchAll()
        {
            //TODO: return workflow output?

            var route = Request.Path.Value;

            if (!route.EndsWith("run") && !route.EndsWith("run/"))
            {
                Response.StatusCode = 404;
                return null;
            }

            var workflowId = route.Replace("/workflows/", "").Replace("/run", "");

            var workflowObj = WorkflowRepository.Get(workflowId);

            if (workflowObj == null)
            {
                Response.StatusCode = 404;
                var jObj = new JObject {{"route", route}};
                return jObj;
            }

            try
            {
                workflowObj.Run();
                Response.StatusCode = 200;
                var jObj = new JObject {{"status", "ok"}};
                return jObj;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                var jObj = new JObject {{"error", ex.Message}};
                return jObj;
            }
        }
    }
}