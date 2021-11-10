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
        private const int STATION_LIMIT_TO_SHOW = 8;

        private RailService _railService;


        private List<Station> _stationsToShow;
        private Station _selectedStation;

        public RegistrationPage()
        {
            _railService = new RailService();


            InitializeComponent();
            ShowStations();
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

        #region event handlers

        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            await HandleNetworkAsync(UnsafeCreateProfile);
        }

        private async void OnSearchStationInput(object sender, TextChangedEventArgs e)
        {
            await HandleNetworkAsync(UnsafeFilterStationWithInput(e.NewTextValue));
        }


        private void OnStationSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selectedStation = (Station) LstStation.SelectedItem;

            EntSearchStation.Text = _selectedStation.StandardName;
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

            await SVMain.ScrollToAsync(EntSearchStation, ScrollToPosition.Start, true);
        }

        #endregion

        #region unsafe network calls
        private async Task UnsafeCreateProfile()
        {
            var allGood = PerformValidation();

            if (!allGood) return;
            IProgressDialog loadingDialog = UserDialogs.Instance.Loading("creating profile");

            UserProfileCreation creationProfile = new UserProfileCreation
            {
                Email = EntEmail.Text,
                Username = EntUsername.Text,
                Password = EntPassword.Text,
                TargetCity = _selectedStation == null ? "UNKNOWN" : _selectedStation.StandardName
            };

            // yes, this is actually a bit dirty
            // correction: never have I ever written even more disgusting code, im sorry
            AuthenticationRepository tempAuthenticationRepository = new AuthenticationRepository();
            UserProfile profile = await tempAuthenticationRepository.CreateProfile(creationProfile);
            loadingDialog.Hide();
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

        private Func<Task> UnsafeFilterStationWithInput(string newText)
        {
            return async () =>
            {
                Station[] stations = await _railService.FilterStations(newText);

                if (_selectedStation != null)
                {
                    _stationsToShow = new List<Station>(stations.Take(STATION_LIMIT_TO_SHOW - 1));

                    if (_stationsToShow.Contains(_selectedStation))
                    {
                        _stationsToShow.Remove(_selectedStation);
                    }

                    _stationsToShow.Insert(0, _selectedStation);
                }
                else
                {
                    _stationsToShow = new List<Station>(stations.Take(STATION_LIMIT_TO_SHOW));
                }

                UpdateStationsToShow();
            };
        }

        private async Task UnsafeShowStationsAsync()
        {
            Station[] stations = await _railService.GetStations();

            _stationsToShow = new List<Station>(stations.Take(STATION_LIMIT_TO_SHOW));

            UpdateStationsToShow();
        }

        #endregion

        private async void ShowStations()
        {
            await HandleNetworkAsync(UnsafeShowStationsAsync);
        }

        private void UpdateStationsToShow()
        {
            LstStation.ItemsSource = _stationsToShow;
            LstStation.HeightRequest = Math.Min(_stationsToShow.Count, 5) * LstStation.RowHeight;
        }

        private bool PerformValidation()
        {
            // validate email
            EmailAddressAttribute emailAddressAttribute = new EmailAddressAttribute();
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
            if (EntPasswordCheck.Text != EntPassword.Text)
            {
                LblPassword.TextColor = Color.IndianRed;
                LblPasswordCheck.TextColor = Color.IndianRed;

                LblPasswordError.IsVisible = true;

                return false;
            }

            return true;
        }
    }
}