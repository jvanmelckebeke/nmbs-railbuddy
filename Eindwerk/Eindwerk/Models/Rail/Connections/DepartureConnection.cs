using System.Collections.Generic;
using Eindwerk.Tools;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.Connections
{
    public class DepartureConnection : EndPointConnection
    {
        [JsonProperty("left")]
        [JsonConverter(typeof(RailConverter))]
        public bool Departed { get; set; }

        public override string ToString()
        {
            return $"DepartureConnection[{nameof(Departed)}: {Departed}, parent: {base.ToString()}]";
        }
    }

   
}