using Microsoft.IdentityModel.Tokens;

namespace Eindwerk.Models.Rail.Requests
{
    public class VehicleRequest : IGetRequest
    {
        public string VehicleName { get; set; }

        public bool IsFilled()
        {
            return !VehicleName.IsNullOrEmpty();
        }

        public string ToGetParameters()
        {
            return $"id={VehicleName}";
        }
    }
}