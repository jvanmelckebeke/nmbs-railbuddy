using System;
using Eindwerk.Tools;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.Stations
{
    public class StationStop
    {
        [JsonProperty(PropertyName = "stationinfo")]
        public Station Station { get; set; }

        [JsonProperty(PropertyName = "arrived")]
        [JsonConverter(typeof(RailConverter))]
        public bool Arrived { get; set; }

        [JsonProperty(PropertyName = "left")]
        [JsonConverter(typeof(RailConverter))]
        public bool Departed { get; set; }


        [JsonProperty(PropertyName = "arrivalCanceled")]
        [JsonConverter(typeof(RailConverter))]
        public bool ArrivalCanceled { get; set; }

        [JsonProperty(PropertyName = "departureCanceled")]
        [JsonConverter(typeof(RailConverter))]
        public bool DepartureCanceled { get; set; }

        [JsonProperty(PropertyName = "arrivalDelay")]
        [JsonConverter(typeof(RailConverter))]
        public TimeSpan ArrivalDelay { get; set; }

        [JsonProperty(PropertyName = "departureDelay")]
        [JsonConverter(typeof(RailConverter))]
        public TimeSpan DepartureDelay { get; set; }

        public override string ToString()
        {
            return $"StationStop[{nameof(Station)}: {Station}, " +
                   $"{nameof(Arrived)}: {Arrived}, " +
                   $"{nameof(Departed)}: {Departed}, " +
                   $"{nameof(ArrivalCanceled)}: {ArrivalCanceled}, " +
                   $"{nameof(DepartureCanceled)}: {DepartureCanceled}, " +
                   $"{nameof(ArrivalDelay)}: {ArrivalDelay:g}, " +
                   $"{nameof(DepartureDelay)}: {DepartureDelay:g}]";
        }
    }
}