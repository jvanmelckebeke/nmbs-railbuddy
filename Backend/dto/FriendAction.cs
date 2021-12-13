using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Backend.dto
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FriendAction
    {
        [EnumMember(Value = "Request")]
        Request,
        [EnumMember(Value = "Accept")]
        Accept,
        [EnumMember(Value = "Delete")]
        Delete,
        Unknown
    }
}