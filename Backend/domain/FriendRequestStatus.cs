using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Backend.Domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FriendRequestStatus
    {
        Sent,
        Accepted,
        Ignored
    }
}