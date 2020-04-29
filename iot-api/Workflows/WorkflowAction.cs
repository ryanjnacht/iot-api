using System;
using System.Collections.Generic;
using System.Threading;
using iot_api.Devices;
using iot_api.Extensions;
using iot_api.Repository;
using Newtonsoft.Json.Linq;

namespace iot_api.Workflows
{
    public class WorkflowAction
    {
        private readonly List<IDevice> _devices;
        private readonly List<WorkflowActionRule> _workflowRules;

        public WorkflowAction(JToken json)
        {
            Fields = json.ToObject<Dictionary<string, dynamic>>();

            _workflowRules = new List<WorkflowActionRule>();
            if (Fields.GetValue<JArray>("rules") != null)
                foreach (var j in Fields.GetValue<JArray>("rules"))
                    _workflowRules.Add(new WorkflowActionRule(j));

            _devices = new List<IDevice>();
            if (Fields.GetValue<JArray>("devices") != null)
                foreach (var j in Fields.GetValue<JArray>("devices"))
                {
                    var id = j["id"]?.ToString();
                    var deviceObj = DeviceRepository.Get(id);

                    if (deviceObj == default(IDevice))
                        throw new Exception($"Could not find the workflow action device: '{id}'");

                    _devices.Add(deviceObj);
                }
        }

        private Dictionary<string, dynamic> Fields { get; }

        private string Id => Fields.GetValue<string>("id");

        private bool Blocking
        {
            get => Fields.GetValue<bool>("blocking");
            set => Fields.AddOrUpdate("blocking", value);
        }

        public JObject ToJObject()
        {
            var jObj = JObject.FromObject(Fields);

            //add devices
            jObj.Remove("devices");

            var deviceArrayObj = new JArray();
            
            foreach (var deviceObj in _devices)
                deviceArrayObj.Add(new JObject
                {
                    {"id", deviceObj.Id}
                });

            jObj.Add("devices", deviceArrayObj);

            //add rules
            jObj.Remove("rules");

            var ruleArrayObj = new JArray();
            
            foreach (var ruleObj in _workflowRules)
                ruleArrayObj.Add(new JObject
                {
                    {"id", ruleObj.RuleId}
                });

            jObj.Add("rules", ruleArrayObj);

            return jObj;
        }

        private bool ShouldRun()
        {
            if (_workflowRules.Count == 0)
            {
                Console.WriteLine($"[Workflow ({Id})] has no rules set, running workflow!");
                return true;
            }

            Console.WriteLine($"[Workflow ({Id})] running rules...");

            var allowed = false;
            var disallowed = false;

            foreach (var workflowRule in _workflowRules)
            {
                var ruleAllowed = false;

                var ruleResult = workflowRule.Rule.ShouldRunRule();

                if (ruleResult && workflowRule.Action == "allow")
                    ruleAllowed = true;

                if (ruleResult && workflowRule.Action == "disallow")
                    disallowed = true;

                if (!ruleResult && workflowRule.Action == "disallow")
                    //disallowed = false;
                    ruleAllowed = true;

                if (disallowed && !ruleAllowed)
                {
                    Console.WriteLine($"[Workflow ({Id})] disallowed by rule: {workflowRule.RuleId}");
                    return false;
                }

                if (!ruleAllowed) continue;

                Console.WriteLine($"[Workflow ({Id})] allowed by rule: {workflowRule.RuleId}");
                allowed = true;
            }

            if (allowed)
            {
                Console.WriteLine($"[Workflow ({Id})] allowed by rules");
                return true;
            }

            Console.WriteLine($"[Workflow ({Id})] rejected: did not match any rules");
            return false;
        }

        public void Run()
        {
            if (!ShouldRun())
                return;

            Thread t;

            switch (Id.ToLower())
            {
                case "flicker":
                    t = new Thread(Flicker);
                    break;
                case "turnon":
                    t = new Thread(TurnOn);
                    break;
                case "turnoff":
                    t = new Thread(TurnOff);
                    break;
                case "toggle":
                    t = new Thread(Toggle);
                    break;
                case "wait":
                    Blocking = true;
                    t = new Thread(Wait);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            t.Start();

            if (Blocking)
                while (t.IsAlive)
                    Thread.Sleep(1);
        }

//TODO: break actions out someplace else

        #region workflow actions

        private void Flicker()
        {
            var iterations = Fields.GetValue<int>("iterations");
            var timeOn = Fields.GetValue<int>("time_on");
            var timeOff = Fields.GetValue<int>("time_off");

            foreach (var deviceObj in _devices)
                for (var i = 0; i < iterations; i++)
                {
                    deviceObj.TurnOn();
                    Thread.Sleep(timeOn);
                    deviceObj.TurnOff();
                    Thread.Sleep(timeOff);
                }
        }

        private void TurnOn()
        {
            foreach (var deviceObj in _devices)
                deviceObj.TurnOn();
        }

        private void TurnOff()
        {
            foreach (var deviceObj in _devices)
                deviceObj.TurnOff();
        }

        private void Toggle()
        {
            foreach (var deviceObj in _devices)
                deviceObj.Toggle();
        }

        private void Wait()
        {
            var duration = Fields.GetValue<int>("duration");
            Thread.Sleep(duration);
        }

        #endregion
    }
}