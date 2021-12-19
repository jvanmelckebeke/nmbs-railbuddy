using Eindwerk.Tools;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail
{
    public class Vehicle
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("shortname")] public string ShortName { get; set; }

        [JsonProperty("type")] public string VehicleType { get; set; }

        [JsonProperty("number")] public string VehicleNumber { get; set; }


        public string FormattedName => $"{VehicleType} {VehicleNumber}";

        public string VehicleHash => Crypto.ComputeMd5(FormattedName);

        public string VehicleColor => "#" + VehicleHash.Substring(0, 6);

        public override string ToString()
        {
            return
                $"Vehicle[{nameof(Name)}: {Name}, {nameof(ShortName)}: {ShortName}, {nameof(VehicleType)}: {VehicleType}, {nameof(VehicleNumber)}: {VehicleNumber}]";
        }
    }
}