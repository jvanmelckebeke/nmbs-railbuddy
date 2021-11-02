using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Models.Forms;
using Eindwerk.Tools;
using Xamarin.Essentials;

namespace Eindwerk.Services
{
    public class AuthenticationService : ApiService
    {
        public AuthenticationService(string accessToken) : base(accessToken)
        {
        }

        public AuthenticationService() : base(null)
        {
        }

        public string GetOwnProfileId()
        {
            return JwtTokenTools.ExtractTokenSubject(AccessToken);
        }

        private async Task<Tokens> RequestTokens(string url, object payload)
        {
            var tokens = await DoPostRequest<Tokens>(url, payload);

            if (tokens == null) return null;

            Preferences.Set("refreshToken", tokens.RefreshToken);
            AccessToken = tokens.AccessToken;
            return tokens;
        }


        public async Task<Tokens> LoginRequest(Credentials credentials)
        {
            var url = $"{BASEURI}/auth/login";
            return await RequestTokens(url, credentials);
        }

        public async Task<Tokens> RefreshTokens(string refreshToken)
        {
            var url = $"{BASEURI}/auth/refresh";

            var payload = new {RefreshToken = refreshToken};
            return await RequestTokens(url, payload);
        }
    }
}