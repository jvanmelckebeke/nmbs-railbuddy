using Backend.Domain;
using Newtonsoft.Json;

namespace Backend.dto
{
    public class FriendRequestResponse
    {
        [JsonProperty("status")]
        public FriendRequestStatus FriendRequestStatus { get; set; }


        public override string ToString()
        {
            return $"FriendRequestResponse[{nameof(FriendRequestStatus)}: {FriendRequestStatus}]";
        }
    }
}