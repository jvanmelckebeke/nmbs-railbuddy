using System;

namespace Eindwerk.Models
{
    public class Tokens : IDtoModel
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public override string ToString()
        {
            return $"Tokens[AccessToken='{AccessToken}', RefreshToken='{RefreshToken}']";
        }

        public bool IsFilled()
        {
            return !string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(RefreshToken);
        }
    }
}