using System;
using Newtonsoft.Json;

namespace Eindwerk.Models
{
    public class UserProfile
    {
        [JsonProperty(PropertyName = "profileId")]
        public Guid ProfileId { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "email")] public string Email { get; set; }
        
        [JsonProperty(PropertyName = "targetCity")]
        public string TargetCity { get; set; }

        [JsonProperty(PropertyName = "avatarUrl")]
        public string AvatarUrl { get; set; }
    }
}