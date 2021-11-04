using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Models.DtoResponses;

namespace Eindwerk.Repository
{
    public class RailApiRepository : ApiRepository
    {
        public RailApiRepository() : base("https://api.irail.be")
        {
        }

        protected override HttpClient GetClient()
        {
            HttpClient client = base.GetClient();
            client.DefaultRequestHeaders.Add("User-Agent",
                "RailBuddy/1.0 (studentproject.mct.be/jari.vanmelckebeke@student.howest.be)");
            return client;
        }

        private string PrepareUrl(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = $"/{path}";
            }

            return path.Contains("?") ? $"{BASEURI}{path}&format=json" : $"{BASEURI}{path}?format=json";
        }


        public async Task<List<Station>> GetStations()
        {
            StationResponse response = await DoGetRequest<StationResponse>(PrepareUrl("/stations"), false);

            if (response != null && response.IsFilled())
            {
                return response.Stations;
            }

            return null;
        }
    }
}