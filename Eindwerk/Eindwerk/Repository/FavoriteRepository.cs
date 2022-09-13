using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Eindwerk.Models.Rail.Requests;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Eindwerk.Repository
{
    public static class FavoriteRepository
    {
        public static List<BaseRouteRequest> GetFavorites()
        {
            string favsInPreferences = Preferences.Get("favorite_routes", "[]");

            List<BaseRouteRequest> favs = JsonConvert.DeserializeObject<List<BaseRouteRequest>>(favsInPreferences);

            Debug.WriteLine($"gotten favorites: {favsInPreferences}");

            return favs ?? new List<BaseRouteRequest>();
        }

        private static void UpdateFavorites(List<BaseRouteRequest> favorites)
        {
            Preferences.Set("favorite_routes", JsonConvert.SerializeObject(favorites));
        }


        public static bool IsFavorite(string routeHash)
        {
            return GetFavorites().Any(favorite => favorite.RouteHash.Equals(routeHash));
        }


        public static void AddToFavorites(BaseRouteRequest request)
        {
            var cpy = new BaseRouteRequest
            {
                FromStation = request.FromStation,
                ToStation = request.ToStation
            };

            Debug.WriteLine($"adding to favorites {cpy}");
            if (IsFavorite(request.RouteHash)) return;

            List<BaseRouteRequest> favs = GetFavorites();

            favs.Add(request);

            UpdateFavorites(favs);
        }


        public static void RemoveFromFavorites(string routeHash)
        {
            Debug.WriteLine($"removing from favorites route with hash {routeHash}");
            List<BaseRouteRequest> nFavs = GetFavorites()
                                           .Where(favorite => favorite.RouteHash != routeHash).ToList();

            UpdateFavorites(nFavs);
        }

        public static void ToggleFavorite(BaseRouteRequest request)
        {
            if (IsFavorite(request.RouteHash))
                RemoveFromFavorites(request.RouteHash);
            else
                AddToFavorites(request);
        }
    }
}