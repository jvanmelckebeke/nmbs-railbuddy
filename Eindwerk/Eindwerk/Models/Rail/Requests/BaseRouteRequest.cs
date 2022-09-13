using Eindwerk.Models.Rail.Stations;
using Eindwerk.Tools;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.Requests
{
    public class BaseRouteRequest
    {
        public Station FromStation { get; set; }

        public Station ToStation { get; set; }

        public string RouteHash => Crypto.ComputeMd5($"{FromStation.FormattedName}{ToStation.FormattedName}");

        public string Color => "#" + RouteHash.Substring(0, 6);


        public string Name => $"{FromStation.FormattedName} - {ToStation.FormattedName}";

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}