using System;
using System.Linq;
using Eindwerk.Models.Rail.Stations;

namespace Eindwerk.Models.Rail.Requests
{
    public class SearchRoutesRequest : IGetRequest
    {
        public Station FromStation { get; set; }

        public Station ToStation { get; set; }

        public TimeSelection TimeSelection { get; set; }

        public DateTime Time { get; set; }

        public bool IsFilled()
        {
            throw new System.NotImplementedException();
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
    }
}