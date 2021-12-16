using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Eindwerk.Models.Rail;
using Eindwerk.Models.Rail.PackedResponses;
using Eindwerk.Models.Rail.Requests;
using Eindwerk.Models.Rail.Stations;

namespace Eindwerk.Repository
{
    public class RailApiRepository : RestRepository
    {
        private const bool DEBUG_RAIL = true;
        public RailApiRepository() : base("https://api.irail.be") { }

        protected override HttpClient GetClient()
        {
            HttpClient client = base.GetClient();
            client.DefaultRequestHeaders.Add("User-Agent",
                "RailBuddy/1.0 (studentproject.mct.be/jari.vanmelckebeke@student.howest.be)");
            return client;
        }

        private async Task<TContent> GetRailData<TPackedResponse, TContent>(string path, IGetRequest parameters = null)
            where TPackedResponse : IPackedResponse<TContent>
        {
            string url = PrepareUrl(path, parameters);

            TPackedResponse packedResponse = await DoGetRequest<TPackedResponse>(url, DEBUG_RAIL);

            if (packedResponse != null && packedResponse.IsFilled())
            {
                return packedResponse.Content;
            }

            return default;
        }

        private string PrepareUrl(string path, IGetRequest request = null)
        {
            if (!path.StartsWith("/")) path = $"/{path}";

            if (request != null)
            {
                Debug.WriteLine($"get parameters: {request.ToGetParameters()}");
            }

            return request == null
                ? $"{BASEURI}{path}?format=json"
                : $"{BASEURI}{path}?format=json&{request.ToGetParameters()}";
        }

        public async Task<List<Station>> GetStations()
        {
            return await GetRailData<StationResponse, List<Station>>("/stations");
        }

        public async Task<List<Route>> GetRoutes(SearchRoutesRequest request)
        {
            return await GetRailData<ConnectionResponse, List<Route>>("/connections", request);
        }
    }
}