using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Eindwerk.Models.BuddyApi
{
    public class SeatRegistration : IDtoModel
    {
        public string VehicleName { get; set; }

        public int WagonIndex { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public bool IsFilled()
        {
            return !VehicleName.IsNullOrEmpty() && WagonIndex > 0;
        }
    }
}