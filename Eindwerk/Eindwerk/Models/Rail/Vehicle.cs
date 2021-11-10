﻿using Newtonsoft.Json;

namespace Eindwerk.Models.Rail
{
    public class Vehicle
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("shortname")] public string ShortName { get; set; }

        [JsonProperty("type")] public string VehicleType { get; set; }

        [JsonProperty("number")] public string VehicleNumber { get; set; }

        public override string ToString()
        {
            return
                $"Vehicle[{nameof(Name)}: {Name}, {nameof(ShortName)}: {ShortName}, {nameof(VehicleType)}: {VehicleType}, {nameof(VehicleNumber)}: {VehicleNumber}]";
        }
    }
}