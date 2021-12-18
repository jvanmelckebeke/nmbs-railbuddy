using Newtonsoft.Json;

namespace Eindwerk.Models.BuddyApi.Friends
{
    public class FriendSeatRegistration
    {
        [JsonProperty("friend")] public Friend Friend { get; set; }

        [JsonProperty("seat")] public SeatRegistration SeatRegistration { get; set; }
    }
}