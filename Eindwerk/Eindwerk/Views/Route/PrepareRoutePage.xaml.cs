using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.Rail;
using Eindwerk.Models.Rail.Stations;
using Eindwerk.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrepareRoutePage : NetworkDependentPage
    {
        private const int MAX_LIST_STATIONS = 6;
        private static readonly Color PRIMARY_BG_COLOR = Color.FromHex("#0169b2");
        private static readonly Color SECONDARY_BG_COLOR = Color.Gray;

        private List<Station> _stationsFrom;
        private List<Station> _stationsTo;

        private readonly RailService _railService;
        private Tokens _tokens;

        private Station _stationFrom;
        private Station _stationTo;
        private TimeSelection _timeSelection = TimeSelection.Departure;

        public PrepareRoutePage(Tokens tokens)
        {
            InitializeComponent();

            _tokens = tokens;
            _railService = new RailService();

            SetupVisual();
            SetupListeners();
            SetupStations();
        }

        private void SetupVisual()
        {
            PckDate.Date = DateTime.Now;
            PckTime.Time = DateTime.Now.TimeOfDay;
        }

        private async void SetupStations()
        {
            _stationsFrom = new List<Station>((await _railService.GetStations()).Take(MAX_LIST_STATIONS));
            _stationsTo = new List<Station>((await _railService.GetStations()).Take(MAX_LIST_STATIONS));
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
            var validInputs = ValidateInputs();

            if (validInputs)
            {
                await HandleNetworkAsync(SearchConnectionsUnsafe);
            }
        }

        private async Task SearchConnectionsUnsafe()
        {
            List<Route> connections = await _railService.GetRoutes(_stationFrom, _stationTo, _timeSelection,
                PckDate.Date, PckTime.Time);
            await Navigation.PushAsync(new ConnectionsResultPage(connections));
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
                    BtnDeparture.BackgroundColor = PRIMARY_BG_COLOR;
                    BtnArrival.BackgroundColor = SECONDARY_BG_COLOR;
                    break;
                case TimeSelection.Arrival:
                    BtnDeparture.BackgroundColor = SECONDARY_BG_COLOR;
                    BtnArrival.BackgroundColor = PRIMARY_BG_COLOR;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region from station event handlers

        private void OnFromStationSelection(object sender, SelectedItemChangedEventArgs e)
        {
            _stationFrom = (Station) e.SelectedItem;
            _stationsFrom = new List<Station>(new[] {_stationFrom});
            EntFromStation.Text = _stationFrom.StandardName;
            EntFromStation.Unfocus();
        }

        private async void OnFromStationText(object sender, TextChangedEventArgs e)
        {
            _stationsFrom =
                new List<Station>((await _railService.FilterStations(e.NewTextValue)).Take(MAX_LIST_STATIONS));
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
            EntToStation.Text = _stationTo.StandardName;
            EntToStation.Unfocus();
        }

        private async void OnToStationText(object sender, TextChangedEventArgs e)
        {
            _stationsTo =
                new List<Station>((await _railService.FilterStations(e.NewTextValue)).Take(MAX_LIST_STATIONS));
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