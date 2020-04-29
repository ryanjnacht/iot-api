using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace iot_api.Workflows
{
    public class Workflow
    {
        private readonly List<WorkflowAction> _workflowActions;

        public readonly string Id;

        public Workflow(JToken json)
        {
            Id = json["id"]?.ToString();

            _workflowActions = new List<WorkflowAction>();

            if (json["actions"] == null) return;

            foreach (var j in json["actions"])
                try
                {
                    _workflowActions.Add(new WorkflowAction(j));
                }
                catch (Exception ex)
                {
                    throw new Exception($"[Workflow] Error adding workflow: {ex.Message}");
                }
        }

        public void Run()
        {
            foreach (var workflowAction in _workflowActions) workflowAction.Run();
        }

        public JObject ToJObject()
        {
            var jObj = new JObject {{"id", Id}};

            var workflowActionsArray = new JArray();

            foreach (var wa in _workflowActions)
                workflowActionsArray.Add(wa.ToJObject());

            jObj.Add("actions", workflowActionsArray);

            return jObj;
        }
    }
}