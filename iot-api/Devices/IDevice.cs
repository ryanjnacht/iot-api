using Newtonsoft.Json.Linq;

namespace iot_api.Devices
{
    public interface IDevice
    {
        string IpAddress { get; }
        string Id { get; }
        Device.DeviceStatuses DeviceStatus { get; }
        void TurnOn();
        void TurnOff();
        void Toggle();
        JObject ToJObject();
    }
}