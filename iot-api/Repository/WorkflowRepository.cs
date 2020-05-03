using System;
using System.Collections.Generic;
using iot_api.Workflows;
using Newtonsoft.Json.Linq;

namespace iot_api.Repository
{
    public static class WorkflowRepository
    {
        private const string CollectionName = "workflows";

        private static DataAccess _dataAccessObj;

        private static DataAccess DataAccessObjObj => _dataAccessObj ??= new DataAccess(CollectionName);


        public static void Add(JObject json)
        {
            var id = json["id"]?.ToString();
            if (string.IsNullOrEmpty(id))
                throw new Exception("cannot add workflow: workflow id is required");

            if (DataAccessObjObj.Get(id) != null)
                throw new Exception($"cannot add workflow '{id}': a workflow with this id already exists");

            DataAccessObjObj.Insert(json);
        }

        public static void Delete(string id)
        {
            DataAccessObjObj.Delete(id);
        }

        public static Workflow Get(string id)
        {
            var json = DataAccessObjObj.Get(id);
            return json == null ? null : new Workflow(json);
        }

        public static List<Workflow> Get()
        {
            var workflowList = new List<Workflow>();

            foreach (var json in DataAccessObjObj.Get())
                workflowList.Add(new Workflow(json));

            return workflowList;
        }

        public static void Clear()
        {
            DataAccessObjObj.Clear();
        }
    }
}