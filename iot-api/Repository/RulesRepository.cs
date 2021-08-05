using System.Collections.Generic;
using iot_api.DataAccess;
using iot_api.Rules;

namespace iot_api.Repository
{
    public static class RulesRepository
    {
        public static void Add(Rule ruleObj)
        {
            // var id = json["id"]?.ToString();
            // if (string.IsNullOrEmpty(id))
            //     throw new Exception("cannot add rule: rule id is required");
            //
            // if (DataAccessObjObj.Get(id) != null)
            //     throw new Exception($"cannot add rule '{id}': a rule with this id already exists");
            //
            // DataAccessObjObj.Insert(json);
            DataAccess<Rule>.Insert(ruleObj);
        }

        public static void Delete(Rule ruleObj)
        {
            DataAccess<Rule>.Delete(ruleObj.Id);
        }

        public static Rule Get(string id)
        {
            var json = DataAccess<Rule>.Get(id);
            return new Rule(json);
        }

        public static IEnumerable<Rule> Get()
        {
            var ruleList = new List<Rule>();

            foreach (var doc in DataAccess<Rule>.Get()) ruleList.Add(new Rule(doc));

            return ruleList;
        }

        public static void Clear()
        {
            DataAccess<Rule>.Clear();
        }
    }
}