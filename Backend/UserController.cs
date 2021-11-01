using System;
using System.IO;
using System.Threading.Tasks;
using Backend.dto;
using Backend.exceptions;
using Backend.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                return new OkObjectResult(Authentication.LoginUser(credentials));
            }
            catch (WrongCredentialsException e)
            {
                log.LogWarning(e, $"Wrong credentials exception for {credentials.Email}");
                return new UnauthorizedResult();
            }
        }

        [FunctionName("TokenRefresh")]
        public static async Task<IActionResult> RefreshTokenAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/refresh")]
            TokenRefreshRequest refreshRequest, ILogger log)
        {
            return new OkObjectResult(Authentication.RefreshToken(refreshRequest));
        }
    }
}