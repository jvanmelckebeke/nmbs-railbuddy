using Newtonsoft.Json;

namespace Eindwerk.Models.BuddyApi
{
    public class FriendRequest : IDtoModel
    {
        [JsonProperty("status")]
        public FriendRequestStatus FriendRequestStatus { get; set; }
        
        public override string ToString()
        {
            return $"FriendRequest[{nameof(FriendRequestStatus)}: {FriendRequestStatus}]";
        }

        public bool IsFilled()
        {
            return FriendRequestStatus != null;
        }
    }
}