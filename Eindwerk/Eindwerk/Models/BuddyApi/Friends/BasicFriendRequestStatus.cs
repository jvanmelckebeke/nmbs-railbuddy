using Newtonsoft.Json;

namespace Eindwerk.Models.BuddyApi.Friends
{
    public class BasicFriendRequestStatus : IDtoModel
    {
        [JsonProperty("status")] public FriendRequestStatus FriendRequestStatus { get; set; }


        public bool IsFilled()
        {
            return true;
        }
    }
}