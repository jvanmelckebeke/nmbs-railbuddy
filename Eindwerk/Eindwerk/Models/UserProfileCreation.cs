using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Eindwerk.Models
{
    public class UserProfileCreation : IDtoModel
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "email")] public string Email { get; set; }

        [JsonProperty(PropertyName = "targetCity")]
        public string TargetCity { get; set; }

        public bool IsFilled()
        {
            var emailAddressAttribute = new EmailAddressAttribute();

            if (Email.IsNullOrEmpty() || !emailAddressAttribute.IsValid(Email)) return false;

            return !(Username.IsNullOrEmpty() || Password.IsNullOrEmpty());
        }
    }
}