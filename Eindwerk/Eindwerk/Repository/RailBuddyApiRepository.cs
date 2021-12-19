using System.Net.Http;

namespace Eindwerk.Repository
{
    public class RailBuddyApiRepository : RestRepository
    {
        public RailBuddyApiRepository(string accessToken) : base("https://railbuddy.azurewebsites.net/api")
        {
            AccessToken = accessToken;
        }

        public RailBuddyApiRepository() : base("https://railbuddy.azurewebsites.net/api") { }


        /**
         * <value>the JWT Access Token to use in requests</value>
         */
        public string AccessToken { get; set; }

        protected override HttpClient GetClient()
        {
            HttpClient client = base.GetClient();
            if (AccessToken != null)
                client.DefaultRequestHeaders.Add("Authorization", AccessToken);
            return client;
        }
    }
}