using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.Rail.Stations;
using Eindwerk.Repository;
using Eindwerk.Services;
using Microsoft.IdentityModel.Tokens;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Credentials = Eindwerk.Models.Forms.Credentials;

namespace Eindwerk.Views.Authentication
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : NetworkDependentPage
    {
        private const int StationLimitToShow = 8;

        private readonly RailService _railService;
        private Station _selectedStation;


        private List<Station> _stationsToShow;

        public RegistrationPage()
        {
            _railService = new RailService();


            InitializeComponent();
            ShowStationsAsync();
            SetupListeners();
        }


        private void SetupListeners()
        {
            EntSearchStation.Focused += OnSearchStationFocussed;
            EntSearchStation.Unfocused += OnSearchStationUnfocussed;
            EntSearchStation.TextChanged += OnSearchStationInput;

            LstStation.ItemSelected += OnStationSelected;

            BtnCreateAccount.Clicked += OnCreateAccountClicked;
        }

        private void UpdateStationsToShow()
        {
            LstStation.ItemsSource = _stationsToShow;
            LstStation.HeightRequest = Math.Min(_stationsToShow.Count, 5) * LstStation.RowHeight;
        }

        private bool PerformValidation()
        {
            // validate email
            var emailAddressAttribute = new EmailAddressAttribute();
            if (EntEmail.Text.IsNullOrEmpty() || !emailAddressAttribute.IsValid(EntEmail.Text))
            {
                LblEmail.TextColor = Color.IndianRed;
                LblEmailError.IsVisible = true;

                return false;
            }

            // validate username
            if (EntUsername.Text.IsNullOrEmpty())
            {
                LblUsername.TextColor = Color.IndianRed;
                LblUsernameError.IsVisible = true;

                return false;
            }

            // validate passwords
            if (EntPasswordCheck.Text == EntPassword.Text) return true;


            LblPassword.TextColor = Color.IndianRed;
            LblPasswordCheck.TextColor = Color.IndianRed;

            LblPasswordError.IsVisible = true;

            return false;
        }

        #region event handlers

        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            await HandleApi(CreateProfile, "creating profile");
        }

        private async void OnSearchStationInput(object sender, TextChangedEventArgs e)
        {
            await FilterStationWithInput(e.NewTextValue);
        }


        private void OnStationSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selectedStation = (Station) LstStation.SelectedItem;

            EntSearchStation.Text = _selectedStation.FormattedName;
        }

        private void OnSearchStationUnfocussed(object sender, FocusEventArgs e)
        {
            // dirty hack to fix keyboard clearspace
            FrStation.HeightRequest -= 50;
            FrClearSpace.HeightRequest = 0;
        }

        private async void OnSearchStationFocussed(object sender, FocusEventArgs e)
        {
            // dirty hack to make content below keyboard visible
            FrStation.HeightRequest += 50;
            FrClearSpace.HeightRequest = 180;

            await SvMain.ScrollToAsync(EntSearchStation, ScrollToPosition.Start, true);
        }

        #endregion

        #region network calls

        private async Task CreateProfile()
        {
            bool allGood = PerformValidation();

            if (!allGood) return;


            var creationProfile = new UserProfileCreation
            {
                Email = EntEmail.Text,
                Username = EntUsername.Text,
                Password = EntPassword.Text,
                TargetCity = _selectedStation == null ? "UNKNOWN" : _selectedStation.FormattedName
            };

            // yes, this is actually a bit dirty
            // correction: never have I ever written even more disgusting code, im sorry
            var tempAuthenticationRepository = new AuthenticationRepository();
            UserProfile profile = await tempAuthenticationRepository.CreateProfile(creationProfile);


            if (profile.IsFilled())
            {
                await UserDialogs.Instance.AlertAsync("profile has been created", "Profile created", "Let's go");


                Tokens tokens = await tempAuthenticationRepository.LoginRequest(new Credentials
                    {Email = creationProfile.Email, Password = creationProfile.Password});

                Preferences.Set("refreshToken", tokens.RefreshToken);
                await Navigation.PopAsync(true);
                return;
            }

            UserDialogs.Instance.Alert("a profile with that email already exists");
        }

        private async Task FilterStationWithInput(string newText)
        {
            Station[] stations = await _railService.FilterStations(newText);

            if (_selectedStation != null)
            {
                _stationsToShow = new List<Station>(stations.Take(StationLimitToShow - 1));

                if (_stationsToShow.Contains(_selectedStation)) _stationsToShow.Remove(_selectedStation);

                _stationsToShow.Insert(0, _selectedStation);
            }
            else
            {
                _stationsToShow = new List<Station>(stations.Take(StationLimitToShow));
            }

            UpdateStationsToShow();
        }

        private async void ShowStationsAsync()
        {
            Station[] stations = await _railService.GetStations();

            _stationsToShow = new List<Station>(stations.Take(StationLimitToShow));

            UpdateStationsToShow();
        }

        #endregion
    }
}