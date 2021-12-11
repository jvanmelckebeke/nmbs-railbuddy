using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Http;
using Backend.Domain;
using Backend.dto;
using Backend.dto.token;
using Backend.exceptions;
using Backend.repositories;
using Backend.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

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
        /// <param name="log">the logger for the function</param>
        /// <param name="successStatusCode">Which HTTP status code should be returned if the request is a success</param>
        /// <param name="acceptRefreshToken">whether a refresh token can be accepted in the authorization header</param>
        private static async Task<IActionResult> AuthorizedHelper<TReturn>(
            Func<UserProfile, Task<TReturn>> handleRequest,
            HttpRequest req, ILogger log,
            int successStatusCode = 200,
            bool acceptRefreshToken = false)
        {
            string accessToken = req.Headers["Authorization"];
            EventId id = new EventId();

            if (accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken[7..];
            }

            try
            {
                AuthenticationService service = new AuthenticationService(log);

                log.LogDebug(id, "authorizing with token {}", accessToken);
                if (!service.ValidateJwt(accessToken, acceptRefreshToken))
                    return new ObjectResult(new {Message = "expected Access Token but Refresh Token was given"})
                        {StatusCode = 401};
                log.LogDebug(id, "validated jwt");

                var currentUserId = service.GetProfileIdFromToken(accessToken);
                UserProfile profile = await UserRepository.FindOneByProfileIdAsync(Guid.Parse(currentUserId));

                TReturn result = await handleRequest(profile);
                return new ObjectResult(result) {StatusCode = successStatusCode};
            }
            catch (SecurityTokenExpiredException)
            {
                return new ObjectResult(new {Message = "Access Token expired"}) {StatusCode = 401};
            }
            catch (Exception e)
            {
                log.LogError(id, e, "request failed with exception");
                return new ExceptionResult(e, false);
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
        /// <param name="log">the logger for the function</param>
        /// <param name="successStatusCode">Which HTTP status code should be returned if the request is a success</param>
        /// <param name="acceptRefreshToken">whether a refresh token can be accepted in the authorization header</param>
        private static async Task<IActionResult> AuthorizedHelper<TReturn>(
            Func<UserProfile, TReturn> handleRequest,
            HttpRequest req, ILogger log,
            int successStatusCode = 200,
            bool acceptRefreshToken = false)
        {
            return await AuthorizedHelper(
                profile => Task.FromResult(handleRequest(profile)),
                req, log, successStatusCode, acceptRefreshToken);
        }

        #endregion

        [FunctionName("UserLogin")]
        public static async Task<IActionResult> LoginUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/login")]
            Credentials credentials, ILogger log)
        {
            EventId id = new EventId();
            log.LogDebug(id, "logging user in with Credentials: {credentials}", credentials);
            try
            {
                return new OkObjectResult(await new AuthenticationService(log).LoginUserAsync(credentials));
            }
            catch (WrongCredentialsException e)
            {
                log.LogWarning(id, e, $"Wrong credentials for {credentials.Email}");
                return new ObjectResult(new {Message = "Wrong credentials", Authenticated = false}) {StatusCode = 401};
            }
            catch (Exception e)
            {
                log.LogError(id, e, "login failed with exception");
                return new ExceptionResult(e, false);
            }
        }

        [FunctionName("TokenRefresh")]
        public static IActionResult RefreshTokenAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/refresh")]
            TokenRefreshRequest refreshRequest, ILogger log)
        {
            EventId id = new EventId();
            log.LogDebug(id, "refreshing token for {refreshRequest}", refreshRequest);
            try
            {
                return new OkObjectResult(AuthenticationService.RefreshToken(refreshRequest));
            }
            catch (Exception e)
            {
                log.LogError(id, e, "refresh failed with exception");
                return new ExceptionResult(e, false);
            }
        }

        [FunctionName("UserCreate")]
        public static async Task<IActionResult> CreateUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/create")]
            UserProfile userProfile, ILogger log)
        {
            EventId id = new EventId();
            log.LogDebug(id, "creating new profile from {userProfile}", userProfile);
            try
            {
                return new ObjectResult(await new UserService(log).CreateProfileAsync(userProfile)) {StatusCode = 201};
            }
            catch (DuplicateProfileException e)
            {
                log.LogWarning(id, e, $"a profile with email {userProfile.Email} is already registered");
                return new ObjectResult(new
                {
                    Message = "This email is already registered"
                }) {StatusCode = 409};
            }
            catch (Exception e)
            {
                log.LogError(id, e, "profile creation failed with exception");
                return new ExceptionResult(e, false);
            }
        }

        [FunctionName("UserGet")]
        public static async Task<IActionResult> GetUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{profileid}")]
            HttpRequest req, string profileid, ILogger log)
        {
            EventId id = new EventId();
            log.LogInformation(id, "getting user with profileId {profileId}", profileid);
            return await AuthorizedHelper(_ => new UserService(log).GetProfileByProfileIdAsync(profileid), req, log);
        }

        [FunctionName("UserGetFriends")]
        public static async Task<IActionResult> GetFriendsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{profileid}/friends")]
            HttpRequest req, string profileid, ILogger log)
        {
            EventId id = new EventId();
            log.LogDebug(id, "getting friends of user with userid {profileid}", profileid);
            return await AuthorizedHelper(_ => new UserService(log).GetFriendsAsync(profileid), req, log);
        }


        [FunctionName("AddFriendRequest")]
        public static async Task<IActionResult> PutFriendRequest(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/{profileid}/friend")]
            HttpRequest req, string profileId, ILogger log)
        {
            EventId id = new EventId();

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            
            log.LogDebug(id, "incoming data is: {data}", body);
            
            FriendRequestAction action =  JsonSerializer.Deserialize<FriendRequestAction>(body);

            log.LogDebug(id, "running friend action {action} on user with profile id {profileid}", action, profileId);
            return await AuthorizedHelper((currentProfile) =>
            {
                UserService service = new UserService(log);
                return action.Action switch
                {
                    FriendAction.Request => service.CreateFriendRequestAsync(currentProfile, profileId),
                    FriendAction.Accept => service.AcceptFriendRequestAsync(currentProfile, profileId),
                    FriendAction.Delete => service.DeleteFriendAsync(currentProfile, profileId),
                    _ => throw new NotImplementedException()
                };
            }, req, log);
        }

        [FunctionName("GetFriendStatus")]
        public static async Task<IActionResult> GetFriendRequest(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{profileid}/friend")]
            HttpRequest req, string profileId, ILogger log)
        {
            EventId id = new EventId();
            log.LogDebug(id, "getting friend status of user with profile id {user}", profileId);
            return await AuthorizedHelper(
                profile => new UserService(log).GetFriendRequestStatus(profile, profileId),
                req, log);
        }
    }
}