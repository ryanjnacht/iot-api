using System;
using System.Collections.Generic;
using iot_api.Workflows;
using Newtonsoft.Json.Linq;

namespace iot_api.Repository
{
    public static class WorkflowRepository
    {
        private const string CollectionName = "workflows";

        public static void Add(JObject json)
        {
            var id = json["id"]?.ToString();
            if (string.IsNullOrEmpty(id))
                throw new Exception("cannot add workflow: workflow id is required");

            if (DataAccess.Get(CollectionName, id) != null)
                throw new Exception($"cannot add workflow '{id}': a workflow with this id already exists");

            DataAccess.Insert(CollectionName, json);
        }

        public static void Delete(string id)
        {
            DataAccess.Delete(CollectionName, id);
        }

        public static Workflow Get(string id)
        {
            var json = DataAccess.Get(CollectionName, id);
            return json == null ? null : new Workflow(json);
        }

        public static List<Workflow> Get()
        {
            var workflowList = new List<Workflow>();

            foreach (var json in DataAccess.Get(CollectionName))
                workflowList.Add(new Workflow(json));

            return workflowList;
        }

        public static void Clear()
        {
            DataAccess.Clear(CollectionName);
        }
    }
}