using System.Collections.Generic;
using Newtonsoft.Json;

namespace Eindwerk.Models.DtoResponses
{
    public class StationResponse : IDtoModel
    {
        /**
         * yes, I kid u not, the response JSON has a property named 'station' (NOT 'stations' !!!) which contains a list
         */
        [JsonProperty("station")]
        public List<Station> Stations { get; set; }

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