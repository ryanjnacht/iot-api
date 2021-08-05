using System.Collections.Generic;
using iot_api.DataAccess;
using iot_api.Workflows;

namespace iot_api.Repository
{
    public static class WorkflowRepository
    {
        public static void Add(Workflow workflowObj)
        {
            // var id = json["id"]?.ToString();
            // if (string.IsNullOrEmpty(id))
            //     throw new Exception("cannot add workflow: workflow id is required");
            //
            // if (DataAccessObjObj.Get(id) != null)
            //     throw new Exception($"cannot add workflow '{id}': a workflow with this id already exists");
            //
            // DataAccessObjObj.Insert(json);
            DataAccess<Workflow>.Insert(workflowObj);
        }

        public static void Delete(Workflow workflowObj)
        {
            DataAccess<Workflow>.Delete(workflowObj.Id);
        }

        public static Workflow Get(string id)
        {
            var json = DataAccess<Workflow>.Get(id);
            return new Workflow(json);
        }

        public static IEnumerable<Workflow> Get()
        {
            var workflowList = new List<Workflow>();

            foreach (var doc in DataAccess<Workflow>.Get()) workflowList.Add(new Workflow(doc));

            return workflowList;
        }

        public static void Clear()
        {
            DataAccess<Workflow>.Clear();
        }
    }
}