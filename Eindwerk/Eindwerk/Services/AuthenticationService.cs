using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Eindwerk.Exceptions;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.Forms;
using Eindwerk.Repository;
using Eindwerk.Tools;
using Xamarin.Essentials;

namespace Eindwerk.Services
{
    public class AuthenticationService
    {
        private readonly AuthenticationRepository _authenticationRepository;

        private Tokens _tokens;

        public AuthenticationService(Tokens tokens)
        {
            _tokens = tokens;
            _authenticationRepository = new AuthenticationRepository(tokens.AccessToken);
        }

        public AuthenticationService()
        {
            _authenticationRepository = new AuthenticationRepository();
        }

        public string GetOwnProfileId()
        {
            return JwtTokenTools.ExtractTokenSubject(_tokens.AccessToken);
        }

        public async Task<Tokens> LoginAsync(Credentials credentials)
        {
            Tokens tokens = await _authenticationRepository.LoginRequest(credentials);

            Debug.WriteLine(tokens);
            
            if (tokens == null) throw new WrongCredentialsException();

            SetTokens(tokens);
            return tokens;
        }

        public void Logout()
        {
            RemoveRefreshToken();
            _tokens = null;
        }

        /**
         * <summary>tries to refresh and return access token, returns null on failure</summary>
         */
        public async Task<Tokens> TryRefreshTokensAsync()
        {
            var refreshToken = GetRefreshToken();

            if (refreshToken == null) return null;

            Tokens tokens = await _authenticationRepository.RefreshTokens(refreshToken);

            if (tokens == null) return null;

            SetTokens(tokens);
            return tokens;
        }

        /**
         * <summary>creates a new Service using the access token this authentication service knows</summary>
         *
         * <typeparam name="TService">the service class to create</typeparam>
         */
        public TService CreateWithTokens<TService>() where TService : SecuredService
        {
            Debug.WriteLine($"tokens: {_tokens}" );
            return (TService) Activator.CreateInstance(typeof(TService), _tokens.AccessToken);
        }

        private void SetTokens(Tokens tokens)
        {
            _tokens = tokens;
            SetRefreshToken(tokens.RefreshToken);
        }


        private static void SetRefreshToken(string value)
        {
            Preferences.Set("refreshToken", value);
        }

        private static string GetRefreshToken(string defaultValue = null)
        {
            return Preferences.Get("refreshToken", defaultValue);
        }

        private static void RemoveRefreshToken()
        {
            Preferences.Remove("refreshToken");
        }
    }
}