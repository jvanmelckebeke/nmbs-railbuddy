using Eindwerk.Models.Rail.Stations;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.Connections
{
    public class EndPointConnection : BaseConnection
    {
        [JsonProperty(PropertyName = "stationinfo")]
        public Station Station { get; set; }

        public override string ToString()
        {
            return $"EndPointConnection[{nameof(Station)}: {Station}, parent: {base.ToString()}]";
        }
    }
}