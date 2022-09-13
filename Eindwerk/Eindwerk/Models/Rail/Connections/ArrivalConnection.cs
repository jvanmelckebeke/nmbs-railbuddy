using Eindwerk.Tools;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.Connections
{
    public class ArrivalConnection : EndPointConnection
    {
        [JsonProperty("arrived")]
        [JsonConverter(typeof(RailConverter))]
        public bool Arrived { get; set; }

        public override string ToString()
        {
            return $"ArrivalConnection[{nameof(Arrived)}: {Arrived}, parent: {base.ToString()}]";
        }
    }
}