using System;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.Domain;
using Backend.dto;
using Backend.exceptions;
using Backend.repositories;
using Backend.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using FriendRequest = Backend.Domain.FriendRequest;

namespace Backend
{
    public static class UserController
    {
        #region Authorized helper functions

        /// <summary>
        /// Helper function which does a couple of things:
        /// <list type="number">
        /// <item><description>Checks if the <c>Authorization</c> header is present and has valid JWT token</description></item>
        /// <item><description>if the <c>Authorization</c> header is invalid, then returns an appropriate <c>HTTP 401</c> response</description></item>
        /// <item><description>if the <c>Authorization</c> header is valid, executes the function passed in <paramref name="handleRequest"/>
        /// and converts it to an <c>ObjectResult</c> with statuscode in <paramref name="successStatusCode"/></description></item>
        /// </list>
        /// </summary>
        ///
        /// <typeparam name="TReturn">the return type of the <paramref name="handleRequest"/> function, also the non-JSON return type of the http function</typeparam>
        ///
        /// <param name="handleRequest">service function to execute if authorization is ok, also the function that handles the request (must be async)</param>
        /// <param name="req">the raw http request</param>
        /// <param name="successStatusCode">Which HTTP status code should be returned if the request is a success</param>
        /// <param name="acceptRefreshToken">whether a refresh token can be accepted in the authorization header</param>
        private static async Task<IActionResult> AuthorizedHelper<TReturn>(
            Func<UserProfile, Task<TReturn>> handleRequest,
            HttpRequest req,
            int successStatusCode = 200,
            bool acceptRefreshToken = false)
        {
            string accessToken = req.Headers["Authorization"];
            try
            {
                if (!AuthenticationService.ValidateJwt(accessToken, acceptRefreshToken))
                    return new ObjectResult(new {Message = "expected Access Token but Refresh Token was given"})
                        {StatusCode = 401};

                string currentUserId = AuthenticationService.GetProfileIdFromToken(accessToken);
                UserProfile profile = await UserRepository.FindOneByProfileIdAsync(Guid.Parse(currentUserId));

                TReturn result = await handleRequest(profile);
                return new ObjectResult(result) {StatusCode = successStatusCode};
            }
            catch (SecurityTokenExpiredException exp)
            {
                return new ObjectResult(new {Message = "Access Token expired"}) {StatusCode = 401};
            }
        }

        /// <summary>
        /// Helper function which does a couple of things:
        /// <list type="number">
        /// <item><description>Checks if the <c>Authorization</c> header is present and has valid JWT token</description></item>
        /// <item><description>if the <c>Authorization</c> header is invalid, then returns an appropriate <c>HTTP 401</c> response</description></item>
        /// <item><description>if the <c>Authorization</c> header is valid, executes the function passed in <paramref name="handleRequest"/>
        /// and converts it to an <c>ObjectResult</c> with statuscode in <paramref name="successStatusCode"/></description></item>
        /// </list>
        /// </summary>
        ///
        /// <typeparam name="TReturn">the return type of the <paramref name="handleRequest"/> function, also the non-JSON return type of the http function</typeparam>
        ///
        /// <param name="handleRequest">service function to execute if authorization is ok, also the function that handles the request (must not be async)</param>
        /// <param name="req">the raw http request</param>
        /// <param name="successStatusCode">Which HTTP status code should be returned if the request is a success</param>
        /// <param name="acceptRefreshToken">whether a refresh token can be accepted in the authorization header</param>
        private static async Task<IActionResult> AuthorizedHelper<TReturn>(
            Func<UserProfile, TReturn> handleRequest,
            HttpRequest req,
            int successStatusCode = 200,
            bool acceptRefreshToken = false)
        {
            return await AuthorizedHelper(
                profile => Task.FromResult(handleRequest(profile)),
                req, successStatusCode, acceptRefreshToken);
        }

        #endregion


        [FunctionName("UserLogin")]
        public static async Task<IActionResult> LoginUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/login")]
            Credentials credentials, ILogger log)
        {
            try
            {
                return new OkObjectResult(await AuthenticationService.LoginUserAsync(credentials));
            }
            catch (WrongCredentialsException e)
            {
                log.LogWarning(e, $"Wrong credentials for {credentials.Email}");
                return new ObjectResult(new {Message = "Wrong credentials", Authenticated = false}) {StatusCode = 401};
            }
        }

        [FunctionName("TokenRefresh")]
        public static IActionResult RefreshTokenAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/refresh")]
            TokenRefreshRequest refreshRequest, ILogger log)
        {
            return new OkObjectResult(AuthenticationService.RefreshToken(refreshRequest));
        }

        [FunctionName("UserCreate")]
        public static async Task<IActionResult> CreateUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/create")]
            UserProfile userProfile, ILogger log)
        {
            try
            {
                return new ObjectResult(await UserService.CreateProfileAsync(userProfile)) {StatusCode = 201};
            }
            catch (DuplicateProfileException e)
            {
                log.LogWarning(e, $"a profile with email {userProfile.Email} is already registered");
                return new ObjectResult(new
                {
                    Message = "This email is already registered"
                }) {StatusCode = 409};
            }
        }

        [FunctionName("UserGet")]
        public static async Task<IActionResult> GetUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{profileid}")]
            HttpRequest req, string profileid, ILogger log)
        {
            return await AuthorizedHelper(_ => UserService.GetProfileByProfileIdAsync(profileid), req);
        }

        [FunctionName("UserGetFriends")]
        public static async Task<IActionResult> GetFriendsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{profileid}/friends")]
            HttpRequest req, string profileid, ILogger log)
        {
            return await AuthorizedHelper(_ => UserService.GetFriendsAsync(profileid), req);
        }


        [FunctionName("AddFriendRequest")]
        public static async Task<IActionResult> PutFriendRequest(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/{profileid}/friend")]
            HttpRequest req, string profileId, ILogger log)
        {
            FriendRequestAction action = await JsonSerializer.DeserializeAsync<FriendRequestAction>(req.Body);
            
            return await AuthorizedHelper((currentProfile) =>
            {
                return action.Action switch
                {
                    FriendAction.Request => UserService.CreateFriendRequestAsync(currentProfile, profileId),
                    FriendAction.Accept => UserService.AcceptFriendRequestAsync(currentProfile, profileId),
                    FriendAction.Delete => UserService.DeleteFriendAsync(currentProfile, profileId),
                    _ => throw new NotImplementedException()
                };
            }, req);
        }

        [FunctionName("GetFriendStatus")]
        public static async Task<IActionResult> GetFriendRequest(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{profileid}/friend")]
            HttpRequest req, string profileId, ILogger log)
        {
            return await AuthorizedHelper(
                profile => UserService.GetFriendRequestStatus(profile, profileId),
                req);
        }
    }
}