using System.Collections.Generic;
using iot_api.DataAccess;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;

namespace iot_api.Devices
{
    [BsonSerializer(typeof(ImpliedImplementationInterfaceSerializer<IDevice, Device>))]
    public interface IDevice : IDocument
    {
        [BsonIgnore] string IpAddress { get; }
        [BsonIgnore] bool Disabled { get; set; }
        [BsonIgnore] Device.DeviceStatuses DeviceStatus { get; }

        Device.DeviceStatuses GetStatus();
        Dictionary<string, dynamic> Fields { get; }
        void TurnOn();
        void TurnOff();
        void Toggle();

        new JObject ToJObject();
    }
}