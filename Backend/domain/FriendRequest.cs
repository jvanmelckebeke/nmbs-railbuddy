using System;
using Backend.repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Backend.Domain
{
    public class FriendRequest : Friend
    {
        [JsonProperty("status")] public FriendRequestStatus FriendRequestStatus { get; set; }


        public override string ToString()
        {
            return
                $"FriendRequest[{base.ToString()}, {nameof(FriendRequestStatus)}: {FriendRequestStatus}]";
        }
    }
}