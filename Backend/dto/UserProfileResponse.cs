﻿using System;
using System.Collections.Generic;
using Backend.Domain;
using Backend.tools;
using Newtonsoft.Json;

namespace Backend.dto
{
    public class UserProfileResponse
    {
        [JsonProperty(PropertyName = "id")] public Guid ProfileId { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "email")] public string Email { get; set; }

        /**
         * <value>The city the user goes to most frequently (default: <c>"UNKNOWN"</c>)</value>
         *
         * <remarks>This is used for internal database <c>PartitionKey</c></remarks>
         */
        [JsonProperty(PropertyName = "targetCity")]
        public string TargetCity { get; set; }


        [JsonProperty("friends")] public List<Friend> Friends { get; set; } = new List<Friend>();

        [JsonProperty("friendRequestReceived")]
        public List<FriendRequest> FriendRequestsReceived { get; set; } = new List<FriendRequest>();


        [JsonProperty("friendRequestSent")]
        public List<FriendRequest> FriendRequestsSent { get; set; } = new List<FriendRequest>();


        public override string ToString()
        {
            return
                $"UserProfileResponse[{nameof(ProfileId)}: {ProfileId}, {nameof(Username)}: {Username}, {nameof(Email)}: {Email}, {nameof(TargetCity)}: {TargetCity}]";
        }
    }
}