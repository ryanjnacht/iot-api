using System;
using System.Collections.Generic;
using iot_api.DataAccess;
using Newtonsoft.Json.Linq;

namespace iot_api.Workflows
{
    public class Workflow : IDocument
    {
        private readonly List<WorkflowAction> _workflowActions;

        public string Id { get; }

        public Workflow(JToken json)
        {
            Id = json["id"]?.ToString();

            _workflowActions = new List<WorkflowAction>();

            if (json["actions"] == null) return;

            foreach (var j in json["actions"])
                try
                {
                    _workflowActions.Add(new WorkflowAction(Id, j));
                }
                catch (Exception ex)
                {
                    throw new Exception($"[Workflow] Error adding workflow: {ex.Message}");
                }
        }

        private string FriendlyName => $"[Workflow ({Id})]";

        public void Run()
        {
            WorkflowThreads.Add(Id);
            Console.WriteLine($"{FriendlyName} has {_workflowActions.Count} workflow actions. Running...");

            foreach (var workflowAction in _workflowActions)
            {
                if (WorkflowThreads.IsCancelled(Id))
                {
                    Console.WriteLine($"{FriendlyName} workflow has been canceled");
                    return;
                }

                workflowAction.Run();
            }

            WorkflowThreads.Remove(Id);
            Console.WriteLine($"{FriendlyName} completed");
        }

        public void Stop()
        {
            WorkflowThreads.Cancel(Id);
            Console.WriteLine($"{FriendlyName} canceling workflow actions...");
        }

        JObject IDocument.ToJObject => ToJObject();

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