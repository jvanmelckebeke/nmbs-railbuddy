using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Backend.Domain
{
    public class UserProfile
    {
        [JsonProperty("id")] public Guid ProfileId { get; set; }

        [JsonProperty("username")] public string Username { get; set; }

        [JsonProperty("email")] public string Email { get; set; }

        [JsonProperty("password")] public string Password { get; set; }

        [JsonProperty("targetCity")] public string TargetCity { get; set; }

        [JsonProperty("friends")] public List<Guid> Friends { get; set; } = new List<Guid>();

        [JsonProperty("friendRequestReceived")]
        public List<FriendRequest> FriendRequestsReceived { get; set; } = new List<FriendRequest>();


        [JsonProperty("friendRequestSent")]
        public List<FriendRequest> FriendRequestsSent { get; set; } = new List<FriendRequest>();

        public override string ToString()
        {
            return $"{nameof(UserProfile)}[{nameof(ProfileId)}: {ProfileId}, " +
                   $"{nameof(Username)}: {Username}, " +
                   $"{nameof(Email)}: {Email}, " +
                   $"{nameof(Password)}: {Password}, " +
                   $"{nameof(TargetCity)}: {TargetCity}, " +
                   $"{nameof(Friends)}: {Friends.Count} friends, " +
                   $"{nameof(FriendRequestsReceived)}: {FriendRequestsReceived.Count} requests, " +
                   $"{nameof(FriendRequestsSent)}: {FriendRequestsSent.Count} requests]";
        }
    }
}