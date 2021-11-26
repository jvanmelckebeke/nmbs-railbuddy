using System;
using Eindwerk.Tools;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Eindwerk.Models.BuddyApi
{
    public class UserProfile : IDtoModel
    {
        [JsonProperty(PropertyName = "profileId")]
        public Guid ProfileId { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "email")] public string Email { get; set; }

        [JsonProperty(PropertyName = "targetCity")]
        public string TargetCity { get; set; }

        /// <summary>
        /// generated Gravatar url
        /// </summary>
        public string AvatarUrl
        {
            get
            {
                var emailLowercase = Email.ToLower().Trim();
                var emailHash = Crypto.ComputeMd5(emailLowercase);

                return $"https://www.gravatar.com/avatar/{emailHash}?d=wavatar&s=200&qzone=2";
            }
        }

        public string QrCodeUrl =>
            $"https://api.qrserver.com/v1/create-qr-code/?data={ProfileId.ToString()}&size=300x300&ecc=M";

        public bool IsFilled()
        {
            return !(Email.IsNullOrEmpty() || Username.IsNullOrEmpty());
        }
    }
}