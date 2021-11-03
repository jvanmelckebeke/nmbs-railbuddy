using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Models.Forms;

namespace Eindwerk.Repository
{
    public class AuthenticationRepository : RailBuddyApiRepository
    {
        public AuthenticationRepository(string accessToken) : base(accessToken)
        {
        }

        public AuthenticationRepository()
        {
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

        private async Task<Tokens> RequestTokens(string url, object payload)
        {
            Tokens tokens = await DoPostRequest<Tokens>(url, payload);
            return tokens;
        }
    }
}