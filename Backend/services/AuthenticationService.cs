using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Backend.Domain;
using Backend.dto;
using Backend.exceptions;
using Backend.repositories;
using Backend.tools;
using Microsoft.IdentityModel.Tokens;

namespace Backend.services
{
    public static class AuthenticationService
    {
        private const string Issuer = "railbuddy.jarivanmelckebeke.be";
        private const string SubClaimName = "sub";
        private const string EmailClaimName = "email";

        private static SymmetricSecurityKey GetSecurityKey()
        {
            // this is: use the value in the environment variable, if there is none present, use 'Very$ecureK3y'
            var jwtKey = Environment.GetEnvironmentVariable("Jwt:Key") ?? "Very$ecureK3y";

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
            };

            var token = new JwtSecurityToken(Issuer,
                Issuer,
                claims,
                expires: isRefresh ? DateTime.Now.AddDays(30) : DateTime.Now.AddMinutes(20),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static TokenResponse GenerateJwtTokens(string profileId, string email)
        {
            return new TokenResponse()
            {
                AccessToken = GenerateSingleJwtToken(profileId, email),
                RefreshToken = GenerateSingleJwtToken(profileId, email, true)
            };
        }

        public static async Task<TokenResponse> LoginUserAsync(Credentials credentials)
        {
            UserProfile profile =
                await UserRepository.GetLoginProfileAsync(credentials.Email, Crypto.ComputeSha256(credentials.Password));

            if (profile == null) throw new WrongCredentialsException();

            return GenerateJwtTokens(profile.ProfileId.ToString(), profile.Email);
        }

        public static TokenResponse RefreshToken(TokenRefreshRequest refreshRequest)
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
                tokenHandler.ValidateToken(refreshRequest.RefreshToken, validationParameters, out SecurityToken _);

            // if the token was invalid, an exception was thrown, so just refresh the token (generate new access and refresh token)

            List<Claim> claims = new List<Claim>(claimsPrincipal.Claims);

            // fixme: this is a really really dirty fix 
            return GenerateJwtTokens(claims[0].Value, claims[1].Value);
        }
    }
}