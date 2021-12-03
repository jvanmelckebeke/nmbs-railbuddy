using Newtonsoft.Json;

namespace Eindwerk.Models.BuddyApi
{
    public class FriendRequest : Friend
    {
        [JsonProperty("status")] public FriendRequestStatus FriendRequestStatus { get; set; }


        public override string ToString()
        {
            return
                $"FriendRequest[{base.ToString()}, {nameof(FriendRequestStatus)}: {FriendRequestStatus}]";
        }
    }
}