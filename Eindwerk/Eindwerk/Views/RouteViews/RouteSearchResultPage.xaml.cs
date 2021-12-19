using System;
using System.Collections.Generic;
using System.Diagnostics;
using Acr.UserDialogs;
using Eindwerk.Assets;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.Rail;
using Eindwerk.Models.Rail.Requests;
using Eindwerk.Repository;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.RouteViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectionsResultPage : LoggedInPage
    {
        private readonly List<Route> _connections;
        private readonly SearchRoutesRequest _originalRoutesRequest;

        public ConnectionsResultPage(Tokens tokens, SearchRoutesRequest originalRoutesRequest, List<Route> connections)
            : base(tokens)
        {
            InitializeComponent();
            _connections = connections;
            _originalRoutesRequest = originalRoutesRequest;

            BindingContext = _originalRoutesRequest;

            var tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Tapped += ToggleFavorite;
            ImFav.GestureRecognizers.Add(tapRecognizer);
        }

        protected override void SetupVisual()
        {
            LstRoutes.ItemsSource = _connections;
            ImFav.Source = FavoriteRepository.IsFavorite(_originalRoutesRequest.RouteHash)
                ? BlackIcon.Star
                : BlackIcon.StarOutline;
        }

        private async void OnRouteSelected(object sender, ItemTappedEventArgs e)
        {
            var r = (Route) e.Item;

            if (r != null)
            {
                await Navigation.PushAsync(new RouteOverviewPage(Tokens, r), true);
                Debug.WriteLine("opening new page");
            }

            LstRoutes.SelectedItem = null;
        }

        private void ToggleFavorite(object sender, EventArgs eventArgs)
        {
            FavoriteRepository.ToggleFavorite(_originalRoutesRequest);
            SetupVisual();

            UserDialogs.Instance.Toast(
                FavoriteRepository.IsFavorite(_originalRoutesRequest.RouteHash)
                    ? "added route to favorites"
                    : "removed favorite");
        }
    }
}