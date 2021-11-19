using System;
using System.Threading.Tasks;
using Backend.Domain;
using Backend.dto;
using Backend.exceptions;
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
        /// <param name="handleRequest">service function to execute if authorization is ok, also the function that handles the request</param>
        /// <param name="req">the raw http request</param>
        /// <param name="successStatusCode">Which HTTP status code should be returned if the request is a success</param>
        /// <param name="acceptRefreshToken">whether a refresh token can be accepted in the authorization header</param>
        private static async Task<IActionResult> AuthorizedHelper<TReturn>(
            Func<Task<TReturn>> handleRequest,
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

                TReturn result = await handleRequest();
                return new ObjectResult(result) {StatusCode = successStatusCode};
            }
            catch (SecurityTokenExpiredException exp)
            {
                return new ObjectResult(new {Message = "Access Token expired"}) {StatusCode = 401};
            }
        }

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
            return await AuthorizedHelper(() => UserService.GetProfileByProfileId(profileid), req);
        }
    }
}