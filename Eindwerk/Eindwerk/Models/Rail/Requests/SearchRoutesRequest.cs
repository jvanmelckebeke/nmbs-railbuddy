using System;
using Newtonsoft.Json;

namespace Eindwerk.Models.Rail.Requests
{
    public class SearchRoutesRequest : BaseRouteRequest, IGetRequest
    {
        public TimeSelection TimeSelection { get; set; }

        public string TimeSelectionText => TimeSelection == TimeSelection.Departure ? "departure" : "arrival";

        public DateTime Time { get; set; }

        public bool IsFilled()
        {
            return RouteHash != null;
        }

        public string ToGetParameters()
        {
            var fromStationPart = $"from={FromStation.Id}";
            var toStationPart = $"to={ToStation.Id}";
            var timeSelectionPart = $"timesel={(TimeSelection == TimeSelection.Arrival ? "arrival" : "departure")}";
            var datePart = $"date={Time:ddMMyy}";
            var timePart = $"time={Time:HHmm}";

            return $"{fromStationPart}&{toStationPart}&{timeSelectionPart}&{datePart}&{timePart}";
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}