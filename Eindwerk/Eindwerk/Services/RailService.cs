using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Models.Rail;
using Eindwerk.Models.Rail.Requests;
using Eindwerk.Models.Rail.Stations;
using Eindwerk.Repository;

namespace Eindwerk.Services
{
    public class RailService
    {
        private RailApiRepository _railApiRepository;

        private Station[] _cachedStations;

        public RailService()
        {
            _railApiRepository = new RailApiRepository();
        }

        public async Task<Station[]> GetStations()
        {
            if (_cachedStations != null) return _cachedStations;


            _cachedStations = (await _railApiRepository.GetStations()).ToArray();
            Array.Sort(_cachedStations);
            return _cachedStations;
        }


        public async Task<Station[]> FilterStations(string partOfName)
        {
            if (_cachedStations == null)
            {
                _cachedStations = await GetStations();
            }


            return Array.FindAll(_cachedStations,
                station => station.FormattedName.StartsWith(partOfName, true, CultureInfo.InvariantCulture));
        }

        public static SearchRoutesRequest CreateSearchRoutesRequest(Station from, Station to,
            TimeSelection timeSelection, DateTime date, TimeSpan time)
        {
            return new SearchRoutesRequest()
            {
                FromStation = from,
                ToStation = to,
                TimeSelection = timeSelection,
                Time = date.Add(time)
            };
        }

        public async Task<List<Route>> GetRoutes(Station from, Station to, TimeSelection timeSelection, DateTime date,
            TimeSpan time)
        {
            SearchRoutesRequest searchRoutesRequest = CreateSearchRoutesRequest(from, to, timeSelection, date, time);

            return await GetRoutes(searchRoutesRequest);
        }

        public async Task<List<Route>> GetRoutes(SearchRoutesRequest searchRoutesRequest)
        {
            List<Route> response = await _railApiRepository.GetRoutes(searchRoutesRequest);
            Debug.WriteLine($"first route: {response[0]}");
            return response;
        }
    }
}