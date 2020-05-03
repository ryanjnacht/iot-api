using System;
using iot_api.Clients;
using Newtonsoft.Json.Linq;

namespace iot_api.Devices
{
    public class Tasmota : Device
    {
        public Tasmota(JToken json) : base(json)
        {
        }

        public override DeviceStatuses DeviceStatus => GetStatus();

        public override void TurnOn()
        {
            Console.WriteLine($"[Tasmota ({Id})] - TurnOn");

            var uri = $"http://{IpAddress}/cm?cmnd=Power%20On";
            var wc = new WebClient();

            wc.Get(uri);
        }

        public override void TurnOff()
        {
            Console.WriteLine($"[Tasmota ({Id})] - TurnOff");

            var uri = $"http://{IpAddress}/cm?cmnd=Power%20off";
            var wc = new WebClient();

            wc.Get(uri);
        }

        public override void Toggle()
        {
            Console.WriteLine($"[Tasmota ({Id})] - Toggle");

            var uri = $"http://{IpAddress}/cm?cmnd=Power%20TOGGLE";
            var wc = new WebClient();

            wc.Get(uri);
        }

        private DeviceStatuses GetStatus()
        {
            Console.WriteLine($"[Tasmota ({Id})] - GetStatus");

            var uri = $"http://{IpAddress}/cm?cmnd=status";

            try
            {
                var wc = new WebClient();
                var resp = wc.Get(uri);
                var jObj = JObject.Parse(resp);

                var power = (int) jObj["Status"]["Power"];

                switch (power)
                {
                    case 0:
                        return DeviceStatuses.Off;
                    case 1:
                        return DeviceStatuses.On;
                    default:
                        Console.WriteLine(
                            $"[Tasmota ({Id})] GetStatus - Could not determine 'power' from json: {jObj}");
                        return DeviceStatuses.Unknown;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Tasmota ({Id})] GetStatus - Error: {ex.Message}");
                return DeviceStatuses.Unavailable;
            }
        }
    }
}