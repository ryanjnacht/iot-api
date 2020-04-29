﻿using iot_api.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace iot_api.Controllers
{
    [ApiController]
    [DisableCors]
    [Route("rules")]
    public class RulesController : ControllerBase
    {
        [HttpGet]
        public JArray GetRules()
        {
            var jArray = new JArray();
            foreach (var ruleObj in RulesRepository.Get())
            {
                var jObj = ruleObj.ToJObject();
                jArray.Add(jObj);
            }

            return jArray;
        }

        [HttpPost]
        public JObject PostRule([FromBody] JObject body)
        {
            RulesRepository.Add(body);
            var id = body["id"]?.ToString();

            return RulesRepository.Get(id).ToJObject();
        }

        [HttpDelete("{id}")]
        public void DeleteRule(string id)
        {
            var deviceObj = RulesRepository.Get(id);
            if (deviceObj == null)
            {
                Response.StatusCode = 404;
                return;
            }

            RulesRepository.Delete(id);
        }
    }
}