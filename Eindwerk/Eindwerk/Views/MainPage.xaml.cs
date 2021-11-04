using System;
using System.Diagnostics;
using Eindwerk.Models;
using Eindwerk.Services;
using Eindwerk.Tools;
using Xamarin.Forms;

namespace Eindwerk.Views
{
    public partial class MainPage : EncapsulatedPage
    {
        private AuthenticationService _authenticationService;
        private UserService _userService;
        private UserProfile _userProfile;

        public MainPage()
        {
            _authenticationService = new AuthenticationService();
            InitializeComponent();
            SetupTokens();
            SetupListeners();
        }

        public MainPage(Tokens tokens)
        {
            _authenticationService = new AuthenticationService(tokens);
            InitializeComponent();
            SetupProfile();
            SetupListeners();
        }

        private async void SetupTokens()
        {
            Tokens tokens = await _authenticationService.TryRefreshTokensAsync();

            if (tokens == null)
            {
                Debug.WriteLine("there was no refresh token");
                await Navigation.PopToRootAsync();
                return;
            }

            SetupProfile();
        }

        private async void SetupProfile()
        {
            _userService = _authenticationService.CreateWithTokens<UserService>();
            _userProfile = await _userService.GetUserProfileAsync(_authenticationService.GetOwnProfileId());
            ShowProfile();
        }

        private void ShowProfile()
        {
            LblUser.Text = $"Hi, {_userProfile.Username}!";
            ImgAvatar.Source = ImageSource.FromUri(new Uri(_userProfile.AvatarUrl));
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            ImgLogoutIcon.Source = AssetHelper.GetIcon("logout.png");
        }

        private void SetupListeners()
        {
            Debug.WriteLine("setup listener mainpage");
            TapGestureRecognizer clickGestureRecognizer = new TapGestureRecognizer();
            clickGestureRecognizer.Tapped += LogoutTapped;
            FrLogout.GestureRecognizers.Add(clickGestureRecognizer);
        }

        private void LogoutTapped(object sender, EventArgs e)
        {
            _authenticationService.Logout();
            Navigation.PopToRootAsync();
        }
    }
}