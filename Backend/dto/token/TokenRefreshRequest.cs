using Newtonsoft.Json;

namespace Backend.dto.token
{
    public class TokenRefreshRequest
    {
        [JsonProperty(PropertyName = "refreshToken")]
        public string RefreshToken { get; set; }

        public override string ToString()
        {
            return $"{nameof(TokenRefreshRequest)}[{nameof(RefreshToken)}: {RefreshToken}]";
        }
    }
}