using System;
using System.Collections.Generic;
using iot_api.Rules;
using Newtonsoft.Json.Linq;

namespace iot_api.Repository
{
    public static class RulesRepository
    {
        private const string CollectionName = "rules";

        public static void Add(JObject json)
        {
            var id = json["id"]?.ToString();
            if (string.IsNullOrEmpty(id))
                throw new Exception("cannot add rule: rule id is required");

            if (DataAccess.Get(CollectionName, id) != null)
                throw new Exception($"cannot add rule '{id}': a rule with this id already exists");

            DataAccess.Insert(CollectionName, json);
        }

        public static void Delete(string id)
        {
            DataAccess.Delete(CollectionName, id);
        }

        public static Rule Get(string id)
        {
            var json = DataAccess.Get(CollectionName, id);
            return new Rule(json);
        }

        public static List<Rule> Get()
        {
            var rulesList = new List<Rule>();

            foreach (var json in DataAccess.Get(CollectionName))
                rulesList.Add(new Rule(json));

            return rulesList;
        }

        public static void Clear()
        {
            DataAccess.Clear(CollectionName);
        }
    }
}