using Newtonsoft.Json;

namespace Eindwerk.Models.BuddyApi.Friends
{
    public class FriendRequest : Friend
    {
        public FriendRequest() { }

        public FriendRequest(Friend other) : base(other) { }

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

        #region equality

        protected bool Equals(FriendRequest other)
        {
            return FriendRequestStatus == other.FriendRequestStatus;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((FriendRequest) obj);
        }

        public override int GetHashCode()
        {
            return (int) FriendRequestStatus;
        }

        public static bool operator ==(FriendRequest left, FriendRequest right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FriendRequest left, FriendRequest right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}