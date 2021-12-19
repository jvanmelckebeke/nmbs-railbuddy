namespace Eindwerk.Services
{
    public abstract class SecuredService
    {
        public SecuredService(string accessToken)
        {
            AccessToken = accessToken;
            SetupAfterTokenSet();
        }

        public string AccessToken { get; set; }
        protected abstract void SetupAfterTokenSet();
    }
}