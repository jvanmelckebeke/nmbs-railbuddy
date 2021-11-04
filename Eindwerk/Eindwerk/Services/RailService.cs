using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Eindwerk.Models;
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
                station => station.StandardName.StartsWith(partOfName, true, CultureInfo.InvariantCulture));
        }
    }
}