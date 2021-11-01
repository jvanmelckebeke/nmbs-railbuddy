using Newtonsoft.Json;

namespace Backend.dto
{
    public class TokenRefreshRequest
    {
        [JsonProperty(PropertyName = "refreshToken")]
        public string RefreshToken { get; set; }
    }
}