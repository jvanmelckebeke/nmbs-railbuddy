using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Eindwerk.Models.BuddyApi.Friends
{
    public class FriendRequestAction
    {
        public FriendRequestAction(FriendAction action)
        {
            Action = action;
        }

        [JsonProperty(PropertyName = "action")]
        public FriendAction Action { get; set; }

        #region equality

        protected bool Equals(FriendRequestAction other)
        {
            return Action == other.Action;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((FriendRequestAction) obj);
        }

        public override int GetHashCode()
        {
            return (int) Action;
        }

        public static bool operator ==(FriendRequestAction left, FriendRequestAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FriendRequestAction left, FriendRequestAction right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FriendAction
    {
        Request,
        Delete,
        Accept
    }
}