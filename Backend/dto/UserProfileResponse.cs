using System;
using Backend.tools;
using Newtonsoft.Json;

namespace Backend.dto
{
    public class UserProfileResponse
    {
        [JsonProperty(PropertyName = "profileId")]
        public Guid ProfileId { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /**
         * <value>The city the user goes to most frequently (default: <c>"UNKNOWN"</c>)</value>
         *
         * <remarks>This is used for internal database <c>PartitionKey</c></remarks>
         */
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

                return $"https://www.gravatar.com/avatar/{emailHash}?d=wavatar";
            }
        }
    }
}