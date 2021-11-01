using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.dto;
using Backend.exceptions;
using Backend.Models;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Backend.services
{
    public static class Authentication
    {
        private const string Issuer = "trainbuddy.jarivanmelckebeke.be";


        private static SigningCredentials GetSigningCredentials()
        {
            // this is: use the value in the environment variable, if there is none present, use 'Very$ecureK3y'
            var jwtKey = Environment.GetEnvironmentVariable("Jwt:Key") ?? "Very$ecureK3y";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }


        private static string GenerateJwtToken(UserProfile profile, bool isRefresh = false)
        {
            SigningCredentials credentials = GetSigningCredentials();

            Claim[] claims =
            {
                new Claim(JwtRegisteredClaimNames.Sub, profile.ProfileId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, profile.Email),
            };

            var token = new JwtSecurityToken(Issuer,
                Issuer,
                claims,
                expires: isRefresh ? DateTime.Now.AddDays(30) : DateTime.Now.AddMinutes(20),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static TokenResponse LoginUser(Credentials credentials)
        {
            // todo: login
            if (true)
            {
                var profile = new UserProfile
                {
                    Username = "foo", Email = "foo@bar.be", ProfileId = new Guid()
                };

                return new TokenResponse()
                {
                    AccessToken = GenerateJwtToken(profile),
                    RefreshToken = GenerateJwtToken(profile, true)
                };
            }
            else
            {
                throw new WrongCredentialsException();
            }
        }
    }
}