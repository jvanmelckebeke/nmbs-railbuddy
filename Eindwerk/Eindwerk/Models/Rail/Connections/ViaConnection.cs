using System;
using Eindwerk.Models.Rail.Stations;
using Eindwerk.Tools;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.Connections
{
    public class ViaConnection
    {
        /// <summary>
        /// this is the connection you arrive with at the via station
        /// </summary>
        [JsonProperty("arrival")]
        public BaseConnection Arrival { get; set; }

        /// <summary>
        /// this is the connection you leave with at the via station
        /// </summary>
        [JsonProperty("departure")]
        public BaseConnection Departure { get; set; }

        [JsonProperty("timeBetween")] 
        [JsonConverter(typeof(RailConverter))]
        public TimeSpan TimeBetween;

        [JsonProperty("stationinfo")] public Station Station;

        public override string ToString()
        {
            return $"ViaConnection[{nameof(TimeBetween)}: {TimeBetween:g}, " +
                   $"{nameof(Station)}: {Station}, " +
                   $"{nameof(Arrival)}: {Arrival}, " +
                   $"{nameof(Departure)}: {Departure}]";
        }
    }
}