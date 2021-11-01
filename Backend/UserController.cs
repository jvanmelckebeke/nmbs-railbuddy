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
                Console.WriteLine(e);
                throw;
            }
        }
    }
}