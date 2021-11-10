using System.Collections.Generic;
using Eindwerk.Models.Rail.Stations;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.PackedResponses
{
    public class StationResponse : IPackedResponse<List<Station>>
    {
        [JsonProperty("station")] public List<Station> Stations { get; set; }

        public List<Station> Content => Stations;

        public bool IsFilled()
        {
            return Stations != null && Stations.Count > 0;
        }

        public override string ToString()
        {
            return $"StationResponse[Stations count: {Stations.Count}]";
        }
    }
}