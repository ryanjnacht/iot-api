using System;
using iot_api.Clients;
using iot_api.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace iot_api.Devices
{
    public class TPLinkSmartPlugHS1xx : Device
    {
        private readonly int _port = 9999;

        public TPLinkSmartPlugHS1xx(JToken json) : base(json)
        {
            var port = Fields.GetValue<int>("port");
            if (port != 0) _port = port;
        }

        public override void TurnOn()
        {
            Console.WriteLine($"[HS1xx ({Id})] - TurnOn");

            var cmd = JsonConvert.SerializeObject(new
            {
                system = new
                {
                    set_relay_state = new
                    {
                        state = 1
                    }
                }
            });

            TPLinkSmartPlugHS1xxClient.SendToSmartPlugOrSwitch(IpAddress, cmd, _port);
        }

        public override void TurnOff()
        {
            Console.WriteLine($"[HS1xx ({Id})] - TurnOff");

            var cmd = JsonConvert.SerializeObject(new
            {
                system = new
                {
                    set_relay_state = new
                    {
                        state = 0
                    }
                }
            });

            TPLinkSmartPlugHS1xxClient.SendToSmartPlugOrSwitch(IpAddress, cmd, _port);
        }

        public override void Toggle()
        {
            Console.WriteLine($"[HS1xx ({Id})] - Toggle");

            var status = DeviceStatus;

            switch (status)
            {
                case DeviceStatuses.On:
                    TurnOff();
                    break;
                case DeviceStatuses.Off:
                    TurnOn();
                    break;
                case DeviceStatuses.Unavailable:
                    break;
                case DeviceStatuses.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override DeviceStatuses GetStatus()
        {
            Console.WriteLine($"[HS1xx ({Id})] - GetStatus");

            if (Disabled) return DeviceStatuses.Disabled;

            var cmd = JsonConvert.SerializeObject(new
            {
                system = new
                {
                    get_sysinfo = new
                    {
                    }
                }
            });

            try
            {
                var resp = TPLinkSmartPlugHS1xxClient.SendToSmartPlugOrSwitch(IpAddress, cmd, _port);
                var relayState = resp["system"]["get_sysinfo"]["relay_state"].ToString();

                if (string.IsNullOrEmpty(relayState))
                    return DeviceStatuses.Unknown;

                if (int.TryParse(relayState, out var i))
                {
                    switch (i)
                    {
                        case 0:
                            return DeviceStatuses.Off;
                        case 1:
                            return DeviceStatuses.On;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HS1xx ({Id})] - GetStatus Error: {ex.Message}");
                return DeviceStatuses.Unavailable;
            }

            return DeviceStatuses.Unknown;
        }
    }
}