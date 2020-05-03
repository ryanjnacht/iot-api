using System;
using System.Collections.Generic;
using iot_api.Rules;
using Newtonsoft.Json.Linq;

namespace iot_api.Repository
{
    public static class RulesRepository
    {
        private const string CollectionName = "rules";

        private static DataAccess _dataAccessObj;

        private static DataAccess DataAccessObjObj => _dataAccessObj ??= new DataAccess(CollectionName);


        public static void Add(JObject json)
        {
            var id = json["id"]?.ToString();
            if (string.IsNullOrEmpty(id))
                throw new Exception("cannot add rule: rule id is required");

            if (DataAccessObjObj.Get(id) != null)
                throw new Exception($"cannot add rule '{id}': a rule with this id already exists");

            DataAccessObjObj.Insert(json);
        }

        public static void Delete(string id)
        {
            DataAccessObjObj.Delete(id);
        }

        public static Rule Get(string id)
        {
            var json = DataAccessObjObj.Get(id);
            return new Rule(json);
        }

        public static List<Rule> Get()
        {
            var rulesList = new List<Rule>();

            foreach (var json in DataAccessObjObj.Get())
                rulesList.Add(new Rule(json));

            return rulesList;
        }

        public static void Clear()
        {
            DataAccessObjObj.Clear();
        }
    }
}