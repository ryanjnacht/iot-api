

# iot-api
This is a dotnet core webapi project that serves as an API gateway between local IoT devices and external services. 

The problem: I wanted to keep my IoT devices behind a walled garden but retain the ability to integrate with them from third party services such as IFTTT. I also found that most DIY IoT devices (such as the tasmota firmware) either didn't have some sort of native cloud support, or it had some cloud support that I simply didn't trust. Lastly, shelved IFTTT integrations lack finer-grain workflow, and even when building a new applet within IFTTT Platform, I was still left with solving for the walled-garden gap or lack of cloud support.

## Configuration
**Environment variables**
- MONGO_HOST (optional. default is localhost)
- MONGO_PORT (optional. default is 27017)
- MONGO_DB (optional. default is "iot-api")
- TIMEOUT (optional. default is 2000ms)
- TZ (optional, but strongly encouraged for time-based rules)

## Build & Deploy
```
docker build -t iot-api:latest .
docker run -p 8088:80 -e MONGO_HOST=10.0.0.5 iot-api:latest
```

## API

### {url}/devices

**Supported devices:**

- tasmota firmware for ESP8266 devices
- TPLink SmartPlug, models hs1xx, hs100, hs110

Example:

    {
      "id": "living-room-lamp", //device name. must be unique
      "ipAddress": "10.9.8.197", //ip address of device
      "type": "hs1xx" //device type
    }

---

### {url}/devices/{deviceId}/{action}

**Supported actions:**
- turnon
- turnoff
- toggle

---

### {url}/rules

**Rule properties:**
- id (string): (friendly name, unique)

**Rule types:**

- time
  - start_time
  - end_time

Example:

  {
      "id": "daylight", //name of rule. must be unique
      "type": "time", //rule type
      "start_time": "8:00", //time parameter
      "end_time": "18:00" //time parameter
    }
    
Note: Make sure to properly set the TZ environment variable in your docker image!
        
---
### {url}/workflows

**Workflow properties:**

- id (string): (friendly name, unique)
- actions (list \<action\>): list of actions

**Workflow Action properties:**

- id (string):id of workflow action to perform
- devices (list\<device\>): list of devices to perform action on
  - id (string): id of device
- rules (list\<rule\>): list of rules to govern the action
  - id (string): id of rule
  - action (string): ("allow"/"disallow") - what to do when rule matches
- blocking (boolean): whether the action should complete before performing the next action
- [workflow action parameters]

Note: if no rule.action is met, nothing will happen. Said another way, you must match on at least 1 "allow" rule and no "disallow" rules for a workflow action to be run.

**Workflow Actions:**

- turnon
- turnoff
- toggle
- flicker
  - time_on (ms)
  - time_off (ms)
  - iterations
- wait
  - duration (ms)

---

### {url}/workflows/{workflowId}/{action}

**Supported actions:**
- run
- stop
---

### Examples:

You have a front yard camera that can make an API call when it detects motion (such as an Arlo camera via IFTTT). You want for the front porch light to turn on for 5 minutes whenever motion is detected, but only at night.

```
//create the device
{
  "id": "front-porch-light",
  "ipAddress": "10.0.0.13",
  "type": "hs1xx"
}

//define a "night" rule that spans from 8pm to 5am
{
  "id": "night",
  "type": "time",
  "start_time": "20:00"
  "end_time": "5:00"
}

//define the workflow
{
  "id": "front-porch-motion",
  "actions": [
    {
      "id": "turnon",
      "devices": [
        {
          "id": "front-porch-light"
        }
      ],
      "rules": [
        {
          "id": "night",
          "action": "allow"
        }
      ]
    },
    {
      "id": "wait",
      "duration": "300000"
    },
    {
      "id": "turnoff",
      "devices": [
        {
          "id": "front-porch-light"
        }
      ]
    }
  ]
}


```

Executing a workflow is a simple GET:
```curl -v {url}/workflows/{workflow}/run```

## TODO:
- Add authorization (likely access keys) to each controller
- Explore additional rule types
  - DayOfWeek