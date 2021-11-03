namespace Eindwerk.Services
{
    public abstract class SecuredService
    {
        public string AccessToken { get; set; }

        public SecuredService(string accessToken)
        {
            AccessToken = accessToken;
            SetupAfterTokenSet();
        }
        protected abstract void SetupAfterTokenSet();
    }
}