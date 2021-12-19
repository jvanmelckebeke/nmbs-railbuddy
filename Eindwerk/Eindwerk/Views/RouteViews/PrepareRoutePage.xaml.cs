using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.Rail;
using Eindwerk.Models.Rail.Requests;
using Eindwerk.Models.Rail.Stations;
using Eindwerk.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.RouteViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrepareRoutePage : NetworkDependentPage
    {
        private const int MaxListStations = 6;
        private static readonly Color PrimaryBgColor = Color.FromHex("#0169b2");
        private static readonly Color SecondaryBgColor = Color.Gray;

        private List<Station> _stationsFrom;
        private List<Station> _stationsTo;

        private readonly RailService _railService;
        private readonly Tokens _tokens;

        private BaseRouteRequest _baseRouteRequest;
        private Station _stationFrom;
        private Station _stationTo;
        private TimeSelection _timeSelection = TimeSelection.Departure;


        public PrepareRoutePage(Tokens tokens, BaseRouteRequest routeRequest = null)
        {
            InitializeComponent();

            _tokens = tokens;
            _railService = new RailService();
            _baseRouteRequest = routeRequest;

            SetupVisual();
            SetupListeners();
        }

        protected override async Task SetupData()
        {
            await SetupStations();
            Debug.WriteLine("HEREEEE 1");

            if (_baseRouteRequest != null)
            {
                EntFromStation.Text = _baseRouteRequest.FromStation.FormattedName;
                EntToStation.Text = _baseRouteRequest.ToStation.FormattedName;

                _stationFrom = _baseRouteRequest.FromStation;
                _stationTo = _baseRouteRequest.ToStation;

                await SearchBaseConnection(_baseRouteRequest);
                _baseRouteRequest = null; // cuz otherwise problems
            }
        }

        private async Task SetupStations()
        {
            _stationsFrom = new List<Station>((await _railService.GetStations()).Take(MaxListStations));
            _stationsTo = new List<Station>((await _railService.GetStations()).Take(MaxListStations));
            Debug.WriteLine("HEREEEE 2");
        }

        private void SetupVisual()
        {
            PckDate.Date = DateTime.Now;
            PckTime.Time = DateTime.Now.TimeOfDay;
        }

        private void SetupListeners()
        {
            EntFromStation.Focused += OnFromStationFocus;
            EntFromStation.Unfocused += OnFromStationUnfocus;
            EntFromStation.TextChanged += OnFromStationText;
            LstFromStation.ItemSelected += OnFromStationSelection;

            EntToStation.Focused += OnToStationFocus;
            EntToStation.Unfocused += OnToStationUnfocus;
            EntToStation.TextChanged += OnToStationText;
            LstToStation.ItemSelected += OnToStationSelection;

            BtnDeparture.Clicked += OnBtnDepartureClicked;
            BtnArrival.Clicked += OnBtnArrivalClicked;

            BtnSearchRoute.Clicked += OnBtnSearchRouteClicked;
        }

        private async void OnBtnSearchRouteClicked(object sender, EventArgs e)
        {
            //validate inputs
            bool validInputs = ValidateInputs();

            if (validInputs)
            {
                await HandleApi(SearchConnections, "searching routes");
            }
        }

        private async Task SearchConnections()
        {
            SearchRoutesRequest routesRequest = RailService.CreateSearchRoutesRequest(_stationFrom, _stationTo,
                _timeSelection,
                PckDate.Date, PckTime.Time);
            List<Route> connections = await _railService.GetRoutes(routesRequest);

            await Navigation.PushAsync(new ConnectionsResultPage(_tokens, routesRequest, connections));
        }

        private async Task SearchBaseConnection(BaseRouteRequest request)
        {
            SearchRoutesRequest routesRequest = RailService.CreateSearchRoutesRequest(request.FromStation,
                request.ToStation,
                TimeSelection.Departure, PckDate.Date, PckTime.Time);

            Debug.WriteLine($"route request: {routesRequest}");
            List<Route> connections = await _railService.GetRoutes(routesRequest);


            await Navigation.PushAsync(new ConnectionsResultPage(_tokens, routesRequest, connections));
        }

        private bool ValidateInputs()
        {
            bool valid = true;


            if (_stationFrom == null)
            {
                LblFromStationError.IsVisible = true;
                valid = false;
            }

            if (_stationTo == null)
            {
                LblToStationError.IsVisible = true;
                valid = false;
            }

            return valid;
        }


        #region time selection handlers

        private void OnBtnArrivalClicked(object sender, EventArgs e)
        {
            _timeSelection = TimeSelection.Arrival;
            UpdateTimeSelectionButtons();
        }

        private void OnBtnDepartureClicked(object sender, EventArgs e)
        {
            _timeSelection = TimeSelection.Departure;
            UpdateTimeSelectionButtons();
        }

        private void UpdateTimeSelectionButtons()
        {
            switch (_timeSelection)
            {
                case TimeSelection.Departure:
                    BtnDeparture.BackgroundColor = PrimaryBgColor;
                    BtnArrival.BackgroundColor = SecondaryBgColor;
                    break;
                case TimeSelection.Arrival:
                    BtnDeparture.BackgroundColor = SecondaryBgColor;
                    BtnArrival.BackgroundColor = PrimaryBgColor;
                    break;
            }
        }

        #endregion

        #region from station event handlers

        private void OnFromStationSelection(object sender, SelectedItemChangedEventArgs e)
        {
            _stationFrom = (Station) e.SelectedItem;
            _stationsFrom = new List<Station>(new[] {_stationFrom});
            EntFromStation.Text = _stationFrom.FormattedName;
            EntFromStation.Unfocus();
        }

        private async void OnFromStationText(object sender, TextChangedEventArgs e)
        {
            _stationsFrom =
                new List<Station>((await _railService.FilterStations(e.NewTextValue)).Take(MaxListStations));
            LstFromStation.ItemsSource = _stationsFrom;
        }

        private void OnFromStationUnfocus(object sender, FocusEventArgs e)
        {
            FrFromStation.IsVisible = false;
        }

        private void OnFromStationFocus(object sender, FocusEventArgs e)
        {
            FrFromStation.IsVisible = true;
            LstFromStation.ItemsSource = _stationsFrom;
        }

        #endregion

        #region to station event handlers

        private void OnToStationSelection(object sender, SelectedItemChangedEventArgs e)
        {
            _stationTo = (Station) e.SelectedItem;
            _stationsTo = new List<Station>(new[] {_stationTo});
            EntToStation.Text = _stationTo.FormattedName;
            EntToStation.Unfocus();
        }

        private async void OnToStationText(object sender, TextChangedEventArgs e)
        {
            _stationsTo =
                new List<Station>((await _railService.FilterStations(e.NewTextValue)).Take(MaxListStations));
            LstToStation.ItemsSource = _stationsTo;
        }

        private void OnToStationUnfocus(object sender, FocusEventArgs e)
        {
            FrClearSpaceTo.IsVisible = false;
            FrToStation.IsVisible = false;
        }

        private void OnToStationFocus(object sender, FocusEventArgs e)
        {
            FrToStation.IsVisible = true;
            LstToStation.ItemsSource = _stationsTo;

            // apply same trick as in registration page
            FrClearSpaceTo.IsVisible = true;
            SVPage.ScrollToAsync(EntToStation, ScrollToPosition.Start, true);
        }

        #endregion
    }
}