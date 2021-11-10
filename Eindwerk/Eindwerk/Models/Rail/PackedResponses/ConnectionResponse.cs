using System.Collections.Generic;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.PackedResponses
{
    public class ConnectionResponse : IPackedResponse<List<Route>>
    {
        [JsonProperty(PropertyName = "connection")]
        public List<Route> Connections { get; set; }

        public List<Route> Content => Connections;

        public bool IsFilled()
        {
            return Connections != null && Connections.Count > 0;
        }

        public override string ToString()
        {
            return $"ConnectionResponse[number of connections: {Connections.Count}]";
        }
    }
}