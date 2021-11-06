using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Services;
using Eindwerk.Tools;
using Xamarin.Forms;

namespace Eindwerk.Views
{
    public partial class MainPage : NetworkDependentPage
    {
        private readonly AuthenticationService _authenticationService;
        private UserService _userService;
        private UserProfile _userProfile;
        private readonly Tokens _tokens;

        public MainPage(Tokens tokens)
        {
            _tokens = tokens;
            _authenticationService = new AuthenticationService(tokens);
            InitializeComponent();

            SetupListeners();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SetupProfileAsync();
        }

        private async void SetupProfileAsync()
        {
            await HandleNetworkAsync(UnsafeSetupProfileAsync);
            SetupVisual();
        }

        private void SetupVisual()
        {
            #region static images

            ImgLogoutIcon.Source = AssetHelper.GetIcon("logout.png");
            ImgAddFavorite.Source = AssetHelper.GetIcon("plus.png");
            ImgAddBuddy.Source = AssetHelper.GetIcon("plus.png");

            #endregion

            #region user text and images

            LblUser.Text = $"Hi, {_userProfile.Username}!";
            ImgAvatar.Source = ImageSource.FromUri(new Uri(_userProfile.AvatarUrl));

            #endregion
        }

        private async Task UnsafeSetupProfileAsync()
        {
            _userService = _authenticationService.CreateWithTokens<UserService>();
            _userProfile = await _userService.GetUserProfileAsync(_authenticationService.GetOwnProfileId());
        }


        private void SetupListeners()
        {
            TapGestureRecognizer clickGestureRecognizer = new TapGestureRecognizer();
            clickGestureRecognizer.Tapped += LogoutTapped;
            FrLogout.GestureRecognizers.Add(clickGestureRecognizer);

            BtnStartTravel.Clicked += OnStartTravelClicked;
        }

        #region event handlers

        private async void OnStartTravelClicked(object sender, EventArgs e)
        {
            const string actionPrepareRoute = "Prepare route";

            var action =
                await DisplayActionSheet("What would you like to do?", "Cancel", null,
                    actionPrepareRoute,
                    "Send seat to a buddy",
                    "Request seat from a buddy");


            Debug.WriteLine("Action: " + action);
            switch (action)
            {
                case actionPrepareRoute:
                    GoToPrepareRoute();
                    return;
                default:
                    Debug.WriteLine("cancel clicked");
                    return;
            }
        }

        private void LogoutTapped(object sender, EventArgs e)
        {
            _authenticationService.Logout();
            Navigation.PopToRootAsync();
        }

        #endregion

        #region navigation

        private void GoToPrepareRoute()
        {
            Navigation.PushAsync(new PrepareRoutePage(_tokens));
        }

        #endregion
    }
}