using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.PackedResponses
{
    public class VehicleResponse : IPackedResponse<Vehicle>
    {
        [JsonProperty(PropertyName = "vehicleInfo")]
        public Vehicle Vehicle { get; set; }

        public bool IsFilled()
        {
            return Vehicle != null;
        }

        public Vehicle Content => Vehicle;
    }
}