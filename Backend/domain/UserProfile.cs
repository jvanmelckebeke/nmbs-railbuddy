using System;
using System.Collections.Generic;
using Backend.tools;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Backend.Domain
{
    public class UserProfile
    {
        [JsonProperty("profileId")]
        public Guid ProfileId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("targetCity")]
        public string TargetCity { get; set; }
        
        [JsonProperty("friends")]
        public List<Guid> Friends { get; set; }
        
        [JsonProperty("friendRequestReceived")]
        public List<FriendRequest> FriendRequestsReceived { get; set; }
        
        
        [JsonProperty("friendRequestSent")]
        public List<FriendRequest> FriendRequestsSent { get; set; }
    }
}