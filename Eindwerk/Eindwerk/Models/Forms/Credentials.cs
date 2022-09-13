using Newtonsoft.Json;

namespace Eindwerk.Models.Forms
{
    public class Credentials : FormModel
    {
        [JsonProperty(PropertyName = "email")] public string Email { get; set; }


        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        public override bool ValidateInputs()
        {
            return Password != null && ValidateEmail(Email);
        }
    }
}