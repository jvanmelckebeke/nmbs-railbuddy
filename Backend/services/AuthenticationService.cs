using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Domain;
using Backend.dto;
using Backend.dto.token;
using Backend.exceptions;
using Backend.repositories;
using Backend.tools;
using Microsoft.Extensions.Logging;

namespace Backend.services
{
    public class AuthenticationService
    {
        private ILogger _log;
        public AuthenticationService(ILogger logger)
        {
            _log = logger;
        }
        
        

        public async Task<TokenResponse> LoginUserAsync(Credentials credentials)
        {
            EventId id = new EventId();
            _log.LogDebug(id, "logging user in with credentials {}", credentials);
            UserProfile profile =
                await UserRepository.GetLoginProfileAsync(credentials.Email,
                    Crypto.ComputeSha256(credentials.Password));
            _log.LogDebug(id, "gotten profile: {}", profile);

            if (profile == null)
            {
                _log.LogWarning(id, "wrong credentials");
                throw new WrongCredentialsException();
            }
            
            

            _log.LogDebug("generating token response");
            TokenResponse resp = TokenService.GenerateJwtTokens(profile.ProfileId.ToString(), profile.Email);
            _log.LogDebug("generated token response: {}", resp);
            return resp;
        }
        
        public bool ValidateJwt(string token, bool acceptRefresh)
        {
            _log.LogDebug("validating JWT token {0} and accepting refresh: {1}", token, acceptRefresh);
            List<Claim> claims = TokenService.GetTokenClaims(token);
            
            _log.LogDebug("JWT token claims: {}", StringTools.FormatList(claims));
            return acceptRefresh || !bool.Parse(claims[2].Value);
        }

        public string GetProfileIdFromToken(string token)
        {
            _log.LogDebug("getting profile from token {}", token);
            List<Claim> claims = TokenService.GetTokenClaims(token);

            return claims[0].Value;
        }

        public static TokenResponse RefreshToken(TokenRefreshRequest refreshRequest)
        {
            return TokenService.RefreshToken(refreshRequest);
        }
    }
}