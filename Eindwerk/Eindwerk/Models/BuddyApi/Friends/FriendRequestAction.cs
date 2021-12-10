using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Eindwerk.Models.BuddyApi.Friends
{
    public class FriendRequestAction
    {
        [JsonProperty(PropertyName = "action")]
        public FriendAction Action { get; set; }

        public FriendRequestAction(FriendAction action)
        {
            Action = action;
        }
    }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FriendAction
    {
        Request,
        Delete,
        Accept
    }
}