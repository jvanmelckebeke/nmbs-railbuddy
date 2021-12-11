using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Backend.dto
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FriendAction
    {
        Request,
        Accept,
        Delete
    }
}