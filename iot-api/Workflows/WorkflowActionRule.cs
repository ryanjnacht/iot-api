using System.Collections.Generic;
using iot_api.Extensions;
using iot_api.Repository;
using iot_api.Rules;
using Newtonsoft.Json.Linq;

namespace iot_api.Workflows
{
    public class WorkflowActionRule
    {
        public WorkflowActionRule(JToken json)
        {
            Fields = json.ToObject<Dictionary<string, dynamic>>();
        }

        public string Action => Fields.GetValue<string>("action");

        public string RuleId => Fields.GetValue<string>("id");

        public Rule Rule => RulesRepository.Get(RuleId);

        private Dictionary<string, dynamic> Fields { get; }

        public JObject ToJObject()
        {
            return JObject.FromObject(Fields);
        }
    }
}