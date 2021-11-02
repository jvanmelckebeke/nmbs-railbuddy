using System;
using Eindwerk.Tools;
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

        /**
         * <value>Generated Gravatar url</value>
         */
        [JsonProperty(PropertyName = "avatarUrl")]
        public string AvatarUrl
        {
            get
            {
                var emailLowercase = Email.ToLower().Trim();
                var emailHash = Crypto.ComputeMd5(emailLowercase);

                return $"https://www.gravatar.com/avatar/{emailHash}?d=wavatar&s=200";
            }
        }
    }
}