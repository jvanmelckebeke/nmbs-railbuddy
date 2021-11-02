using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.dto;
using Microsoft.IdentityModel.Tokens;

namespace Backend.services
{
    public static class TokenService
    {
        private const string Issuer = "railbuddy.jarivanmelckebeke.be";
        private const string SubClaimName = "sub";
        private const string EmailClaimName = "email";

        private static SymmetricSecurityKey GetSecurityKey()
        {
            // this is: use the value in the environment variable, if there is none present, use 'Very$ecureK3y'
            var jwtKey = Environment.GetEnvironmentVariable("JwtKey") ?? "Very$ecureK3y";

            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        }

        private static SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256);
        }


        private static string GenerateSingleJwtToken(string profileId, string email, bool isRefresh = false)
        {
            SigningCredentials credentials = GetSigningCredentials();

            Claim[] claims =
            {
                new Claim(SubClaimName, profileId),
                new Claim(EmailClaimName, email),
                new Claim("refresh", isRefresh.ToString())
            };

            var token = new JwtSecurityToken(Issuer,
                Issuer,
                claims,
                expires: isRefresh ? DateTime.Now.AddDays(30) : DateTime.Now.AddMinutes(20),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static TokenResponse GenerateJwtTokens(string profileId, string email)
        {
            return new TokenResponse()
            {
                AccessToken = GenerateSingleJwtToken(profileId, email),
                RefreshToken = GenerateSingleJwtToken(profileId, email, true)
            };
        }

        public static TokenResponse RefreshToken(TokenRefreshRequest refreshRequest)
        {
            List<Claim> claims = GetTokenClaims(refreshRequest.RefreshToken);

            // fixme: this is a really really dirty fix 
            return GenerateJwtTokens(claims[0].Value, claims[1].Value);
        }
        
        public static List<Claim> GetTokenClaims(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSecurityKey(),
                ValidIssuer = Issuer,
                ValidAudience = Issuer
            };

            ClaimsPrincipal claimsPrincipal =
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken _);

            return new List<Claim>(claimsPrincipal.Claims);
        }
    }
}