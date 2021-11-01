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

namespace Backend
{
    public static class UserController
    {
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
                log.LogWarning(e, $"Wrong credentials exception for {credentials.Email}");
                return new ObjectResult(new {Message = "Wrong credentials", Authenticated = false}) {StatusCode = 401};
            }
        }

        [FunctionName("TokenRefresh")]
        public static async Task<IActionResult> RefreshTokenAsync(
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
            return new OkObjectResult(await UserService.GetProfileByProfileId(profileid));
        }
    }
}