using System;
using System.Collections.Generic;
using Eindwerk.Models.Rail.Stations;
using Eindwerk.Tools;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.Connections
{
    public class BaseConnection
    {
        [JsonProperty(PropertyName = "delay")]
        [JsonConverter(typeof(RailConverter))]
        public TimeSpan Delay { get; set; }

        [JsonProperty(PropertyName = "time")]
        [JsonConverter(typeof(RailConverter))]
        public DateTime Time { get; set; }

        [JsonProperty("vehicleinfo")] public Vehicle Vehicle { get; set; }

        [JsonProperty("platforminfo")] public Platform Platform { get; set; }

        [JsonProperty("canceled")]
        [JsonConverter(typeof(RailConverter))]
        public bool Canceled { get; set; }

        [JsonProperty("stops")] private PackedStop _packedStops;

        public List<StationStop> Stops => _packedStops.Stops;

        [JsonProperty("direction")] private PackedDirection _packedDirection;

        public string Direction => _packedDirection.DirectionName;

        public override string ToString()
        {
            return
                $"BaseConnection[{nameof(Delay)}: {Delay:g}, " +
                $"{nameof(Time)}: {Time:g}, " +
                $"{nameof(Vehicle)}: {Vehicle}, " +
                $"{nameof(Platform)}: {Platform}, " +
                $"{nameof(Canceled)}: {Canceled}, " +
                $"{nameof(Stops)}: {(_packedStops == null ? "no stops" : $"{Stops.Count} stops")}, " +
                $"{nameof(Direction)}: {Direction}]";
        }
    }

    internal class PackedDirection
    {
        [JsonProperty("name")] public string DirectionName { get; set; }
    }

    internal class PackedStop
    {
        [JsonProperty("stop")] public List<StationStop> Stops;
    }
}