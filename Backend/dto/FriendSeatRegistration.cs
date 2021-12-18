using Backend.Domain;
using Newtonsoft.Json;

namespace Backend.dto
{
    public class FriendSeatRegistration
    {
        [JsonProperty("friend")]
        public Friend Friend { get; set; }

        [JsonProperty("seat")]
        public SeatRegistration SeatRegistration { get; set; }
    }
}