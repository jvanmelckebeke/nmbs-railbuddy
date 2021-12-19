namespace Eindwerk.Models.BuddyApi
{
    public class Tokens : IDtoModel
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public bool IsFilled()
        {
            return !string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(RefreshToken);
        }

        public override string ToString()
        {
            return $"Tokens[AccessToken='{AccessToken}', RefreshToken='{RefreshToken}']";
        }
    }
}