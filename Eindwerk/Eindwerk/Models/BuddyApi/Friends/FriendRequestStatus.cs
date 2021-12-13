using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Eindwerk.Models.BuddyApi.Friends
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FriendRequestStatus
    {
        Sent,
        Accepted,
        Ignored
    }
}