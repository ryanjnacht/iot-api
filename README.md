# iot-api

iot-api is a dotnet core webapi project that serves as an API gateway between local IoT devices and the internet.

**The problem**: I wanted to walled garden my IoT devices, simplify an interface to interact with them, retain 3rd party
integration with IFTTT, and add fine-grain functionality such as workflows and schedules.

## Features ##

- Logical workflows that can toggle devices and leverage rules and timers for execution
- Simple schedules to turn devices on and off
- Optional security keys per device or workflow

## Build & Deploy

```
docker build -t iot-api:latest .
docker run -p 8088:80 -e MONGO_HOST=10.0.0.5 iot-api:latest
```

## Configuration

**Environment variables**

- SECURITY: defaults to 1. set to '0' to disable accessKeys
- MONGO_HOST: required if you want to use mongo as a persistent data store
- MONGO_PORT: defaults to 27017
- MONGO_DB: defaults to "iot-api"
- CACHING: defaults to 1. disable if you are going to scale iot-api
- DEVICE_RETRIES: defaults to 10. number of retries for each IoT device interaction
- TIMEOUT: mongo and web client timeout. defaults to 2000(ms)
- TZ: optional container timezone. encouraged for time-based rules

Note: If you do not define a MONGO_HOST, the application will keep everything in memory and nothing will be persisted
across application restarts.

## Security

Security is implemented in the form of access keys, which are to be provided to each endpoint via query string
parameter (```?accessKey={accessKey}```)

When the application starts, if no access keys are found, a default admin access key is created and printed to stdout.

Access keys should be limited to specific devices and/or workflows. Admin access keys can enumerate all resources,
modify rules, and create and delete access keys.

Granting access to a workflow is not mutually inclusive of granting access to the workflow's devices; a workflow does
not require an access key to also have access to its devices in order to run. In this way, you can have an access key
for a workflow that turns on a light, but that same access key cannot be used to directly interface with the light to
turn it off.

To reset (clear) the access keys, set the `RESET_AUTH` env var to 1 at startup. All access keys will be cleared and a
new admin access key printed to standard output.

## API Endpoints

### accessKeys (POST, GET, DELETE)

- id (string): randomly generated
- name (string): friendly name
- admin (bool): admin flag
- devices (string[]): array of deviceIds that this access key should have access to
- workflows: (string[]): array of workflowIds that this access key should have access to

Examples:

```
//default admin key
{
    "id": "4b8ed0a5cc86497c977dad76156b4d41",
    "name": "default admin",
    "admin": true,
    "devices": null,
    "workflows": null
}
  
//restricted to device
{
  "id": "d3ba1b6271a148f8a1d0a4ae7bd6bd4a",
  "name": "ifttt_living-room-lamp",
  "admin": false,
  "devices" :["living-room-lamp"],
  "workflows": null
}

//restricted to workflow
{
  "id": "5112659815924a0eb4dd04a6259c3718",
  "name": "ifttt_turn-on-inside-lights",
  "admin": false,
  "devices": null,
  "workflows" :["turn-on-inside-lights"]
}
```

--

### devices (POST, GET, DELETE)

- id (string): friendly name
- ipAddress (string): ip address of the device
- type (string): device type

**Supported devices:**

- tasmota firmware for ESP8266 devices
    - type: "tasmota"
- TPLink SmartPlug, models hs100 & hs110
    - type: "hs1xx"

Example:

```
{
  "id": "living-room-lamp",
  "ipAddress": "10.9.8.197",
  "type": "hs1xx"
}
```

--

### devices/{deviceId}/{action} (GET)

**Supported actions:**

- on / turnOn
- off / turnOff
- toggle
- disable
- enable

```
curl -v {url}/devices/{deviceId}/on?accessKey={accessKey}
```

--

### schedules (POST, GET, DELETE)

- start_time (string, hh:mm)
- devices (array)
    - deviceId (string, id of device)
    - action (string, action to take (on, off, toggle)

Example:

```
{
  "start_time": "19:30",
  "devices": [
      {
          "deviceId": "living-room-lamp",
          "action": "off"
      }
  ]
}
```

--

### rules (POST, GET, DELETE)

**Rule properties:**

- id (string): (friendly name, unique)

**Rule types:**

- time
    - start_time (string, hh:mm)
    - end_time (string, hh:mm)
- dayOfWeek
    - days ( int[], 0-6)

Example:

```
//define daylight as 7am to 6pm
{
  "id": "daylight",
  "type": "time",
  "start_time": "7:00",
  "end_time": "18:00"
}
```

Note: Make sure to properly set the TZ environment variable in your docker container!
        
--

### workflows (POST, GET, DELETE)

**Workflow properties:**

- id (string): (friendly name, unique)
- actions (list \<action\>): list of actions

**Workflow Action properties:**

- id (string):id of workflow action to perform
- devices (list\<device\>): list of devices to perform action on
    - id (string): id of device
- rules (list\<rule\>): list of rules to govern the action. null to always run.
    - id (string): id of rule
    - condition (string): ("allow"/"disallow") - what to do when rule matches
- blocking (boolean): whether the action should complete before performing the next action
- [workflow action parameters]

Note: If a workflow action rule is specified, it must satisfy at least one "allow" condition and no "disallow"
conditions in order to run.

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

--

### workflows/{workflowId}/{action} (GET)

**Supported actions:**

- run: begin running the workflow
- cancel: the workflow will cancel after the current action completes

```
curl -v {url}/workflows/{workflow}/run?accessKey={accessKey}
```

--

### Examples:

You have a front yard camera that can make an API call when it detects motion (such as an Arlo camera via IFTTT). You
want for the front porch light to turn on for 5 minutes whenever motion is detected, but only at night (between 8pm and
5am):

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
          "condition": "allow"
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

This example is why I built this API in the first place. I live in busy neighborhood in a very big city. A string of
small crimes nearby convinced my landlord to install Arlo cameras on the front and back of the property. It wasn't long
before we captured video footage of thieves on our front porch at night, rummaging through our belongings and taking
anything of interest. I decided that a booby trap was in order. I bought a 12v car horn and wired it up to a wireless
relay controlled by a ESP8266 chip running tasmota firmware. I could easily talk to the tasmota API on my LAN, but there
was no way to connect it to the IFTTT integration with Arlo.

What I wanted to do was have the front porch camera trigger a workflow where if motion were detected between 2am and 5am
on the front porch, the horn would start blaring and the front porch light would start flashing. Not only would this
inform us of the intrusion happening outside, but it would surely discourage repeat offenses! Here's the workflow for
that:

```
  //devices:
  {
    "id": "horn",
    "ipAddress": "10.0.0.3",
    "type": "tasmota"
  }

  {
    "id": "front-porch-light",
    "ipAddress": "10.0.0.2",
    "type": "hs1xx"
  }
  
//rules:
  {
    "id": "witching-hour",
    "type": "time",
    "start_time": "2:05",
    "end_time": "4:50"
  }

  {
    "id": "daylight",
    "type": "time",
    "start_time": "8:00",
    "end_time": "18:00"
  }

//workflow
{
  "id": "honkalarm",
  "actions": [
    {
      "id": "flicker",
      "iterations": 8,
      "time_on": 750,
      "time_off": 500,
      "devices": [
        {
          "id": "horn"
        }
      ],
      "rules": [
        {
          "id": "witching-hour",
          "action": "allow"
        }
      ]
    },
    {
      "id": "flicker",
      "iterations": 10,
      "time_on": 500,
      "time_off": 100,
      "devices": [
        {
          "id": "front-porch-light"
        }
      ],
      "rules": [
        {
          "id": "witching-hour",
          "action": "allow"
        }
      ],
      "blocking": true
    },
    {
      "id": "turnon",
      "devices": [
        {
          "id": "front-porch-light"
        }
      ],
      "rules": [
        {
          "id": "daylight",
          "action": "disallow"
        }
      ]
    }
  ]
}
```
