using System;
using Eindwerk.Tools;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Eindwerk.Models.BuddyApi
{
    public class Friend : IDtoModel
    {
        [JsonProperty("userId")] public Guid UserId { get; set; }

        [JsonProperty("email")] public string Email { get; set; }

        [JsonProperty("username")] public string Username { get; set; }

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

        public ImageSource Avatar => ImageSource.FromUri(new Uri(AvatarUrl));

        public override string ToString()
        {
            return $"Friend[{nameof(UserId)}: {UserId}, {nameof(Email)}: {Email}, {nameof(Username)}: {Username}]";
        }

        public bool IsFilled()
        {
            return Email != null;
        }
    }
}