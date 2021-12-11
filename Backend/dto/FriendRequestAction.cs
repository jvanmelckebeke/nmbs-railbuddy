using System;
using Newtonsoft.Json;

namespace Backend.dto
{
    public class FriendRequestAction
    {
        [JsonProperty("action")] public FriendAction Action { get; set; }

        public override string ToString()
        {
            return $"{nameof(Action)}: {Enum.GetName(typeof(FriendAction), Action)}";
        }
    }
}