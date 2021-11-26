using System;
using Backend.repositories;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Backend.Domain
{
    public class FriendRequest
    {
        [JsonProperty("status")]
        public FriendRequestStatus FriendRequestStatus { get; set; }

        [JsonProperty("userId")]
        public Guid UserId { get; set; }
    }
}