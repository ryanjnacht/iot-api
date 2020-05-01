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
        private readonly string _workflowId;
        private readonly List<WorkflowActionRule> _workflowRules;

        public WorkflowAction(string workflowId, JToken json)
        {
            _workflowId = workflowId;

            Fields = json.ToObject<Dictionary<string, dynamic>>();

            _workflowRules = new List<WorkflowActionRule>();
            if (Fields.GetValue<JArray>("rules") != null)
                foreach (var j in Fields.GetValue<JArray>("rules"))
                    _workflowRules.Add(new WorkflowActionRule(j));

            _devices = new List<IDevice>();
            if (Fields.GetValue<JArray>("devices") != null)
                foreach (var j in Fields.GetValue<JArray>("devices"))
                {
                    var deviceId = j["id"]?.ToString();
                    var deviceObj = DeviceRepository.Get(deviceId);

                    if (deviceObj == default(IDevice))
                        throw new Exception($"{FriendlyName} Could not find the workflow action device: '{deviceId}'");

                    _devices.Add(deviceObj);
                }
        }

        private string FriendlyName => $"[Workflow ({_workflowId}), Workflow Action ({Id.ToLower()})]";

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
                Console.WriteLine($"{FriendlyName} has no rules set, defaulting to allow");
                return true;
            }

            Console.WriteLine($"{FriendlyName} evaluating rules...");

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
                    Console.WriteLine($"{FriendlyName} disallowed by rule: {workflowRule.RuleId}");
                    return false;
                }

                if (!ruleAllowed) continue;

                Console.WriteLine($"{FriendlyName} allowed by rule: {workflowRule.RuleId}");
                allowed = true;
            }

            if (allowed)
            {
                Console.WriteLine($"{FriendlyName} allowed by rules");
                return true;
            }

            Console.WriteLine($"{FriendlyName} rejected: did not match any rules");
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

            Console.WriteLine($"{FriendlyName} running workflow action");
            t.Start();

            if (!Blocking) return;

            if (Id.ToLower() == "wait")
            {
                var duration = Fields.GetValue<int>("duration");
                Console.WriteLine($"{FriendlyName} waiting for {duration}ms");
            }
            else
            {
                Console.WriteLine($"{FriendlyName} waiting for action to complete");
            }

            while (t.IsAlive)
                Thread.Sleep(1);
        }

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