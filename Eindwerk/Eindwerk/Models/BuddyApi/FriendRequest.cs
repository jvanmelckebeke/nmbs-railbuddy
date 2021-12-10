using Newtonsoft.Json;

namespace Eindwerk.Models.BuddyApi
{
    public class FriendRequest : Friend
    {
        public FriendRequest()
        {
            
        }

        public FriendRequest(Friend other) : base(other)
        {
            
        }

        public FriendRequest(FriendRequest other) : base(other)
        {
            FriendRequestStatus = other.FriendRequestStatus;
        }
        
        [JsonProperty("status")] public FriendRequestStatus FriendRequestStatus { get; set; }


        public override string ToString()
        {
            return
                $"FriendRequest[{base.ToString()}, {nameof(FriendRequestStatus)}: {FriendRequestStatus}]";
        }
    }
}