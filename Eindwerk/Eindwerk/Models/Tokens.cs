namespace Eindwerk.Models
{
    public class Tokens
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public override string ToString()
        {
            return $"Tokens[AccessToken='{AccessToken}', RefreshToken='{RefreshToken}']";
        }
    }
}