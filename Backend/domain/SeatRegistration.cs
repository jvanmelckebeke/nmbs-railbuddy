using System;
using Newtonsoft.Json;

namespace Backend.Domain
{
    public class SeatRegistration
    {
        [JsonProperty("id")] public Guid SeatRegistrationId { get; set; }

        public string TrainNumber { get; set; }

        public int WagonIndex { get; set; }

        public Guid ProfileId { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}