using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Eindwerk.Tools
{
    public static class JwtTokenTools
    {
        private static JwtSecurityToken ProcessJwtToken(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(jwt);
        }

        public static string ExtractTokenClaim(string jwt, string property)
        {
            JwtSecurityToken token = ProcessJwtToken(jwt);

            return token.Claims.First(claim => claim.Type == property).Value;
        }

        public static string ExtractTokenSubject(string jwt)
        {
            return ProcessJwtToken(jwt).Subject;
        }
    }
}