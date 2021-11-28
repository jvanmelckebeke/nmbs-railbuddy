using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Domain;
using Backend.dto;
using Backend.dto.token;
using Backend.exceptions;
using Backend.repositories;
using Backend.tools;

namespace Backend.services
{
    public static class AuthenticationService
    {
        

        public static async Task<TokenResponse> LoginUserAsync(Credentials credentials)
        {
            UserProfile profile =
                await UserRepository.GetLoginProfileAsync(credentials.Email,
                    Crypto.ComputeSha256(credentials.Password));

            if (profile == null) throw new WrongCredentialsException();

            return TokenService.GenerateJwtTokens(profile.ProfileId.ToString(), profile.Email);
        }
        
        public static bool ValidateJwt(string token, bool acceptRefresh)
        {
            List<Claim> claims = TokenService.GetTokenClaims(token);

            return acceptRefresh || !bool.Parse(claims[2].Value);
        }

        public static string GetProfileIdFromToken(string token)
        {
            List<Claim> claims = TokenService.GetTokenClaims(token);

            return claims[0].Value;
        }

        public static TokenResponse RefreshToken(TokenRefreshRequest refreshRequest)
        {
            return TokenService.RefreshToken(refreshRequest);
        }
    }
}