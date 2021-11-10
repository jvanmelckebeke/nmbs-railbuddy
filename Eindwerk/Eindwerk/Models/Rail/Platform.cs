using Eindwerk.Tools;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail
{
    public class Platform
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("normal")]
        [JsonConverter(typeof(RailConverter))]
        public bool IsUsual { get; set; }

        public override string ToString()
        {
            return $"Platform[{Name}, which is {(IsUsual ? "normal" : "unusual")}]";
        }
    }
}