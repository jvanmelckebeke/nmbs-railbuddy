using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Services;
using Eindwerk.Views.Authentication;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Eindwerk.Views
{
    public partial class MainPage : ContentPage
    {
        private Tokens _tokens;
        private AuthenticationService _authenticationService;
        private UserService _userService;
        private UserProfile _userProfile;

        public MainPage()
        {
            InitializeComponent();
            
            string refreshToken = Preferences.Get("refreshToken", null);

            if (refreshToken == null)
            {
                Debug.WriteLine("there was no refresh token");
                Navigation.PopToRootAsync();
                return;
            }

            RefreshPageWithRefreshToken(refreshToken);
        }

        public MainPage(Tokens tokens)
        {
            InitializeComponent();
            _tokens = tokens;
            _authenticationService = new AuthenticationService(tokens.AccessToken);
            SetupProfile();
        }

        private async Task RefreshPageWithRefreshToken(string refreshToken)
        {
            _authenticationService = new AuthenticationService();
            _tokens = await _authenticationService.RefreshTokens(refreshToken);
            _authenticationService = new AuthenticationService(_tokens.AccessToken);

            await SetupProfile();
        }

        private async Task SetupProfile()
        {
            _userService = new UserService(_tokens.AccessToken);
            _userProfile = await _userService.GetUserProfile(_authenticationService.GetOwnProfileId());
            ShowProfile();
        }

        private void ShowProfile()
        {
            LblUser.Text = $"Hi, {_userProfile.Username}!";
            ImgAvatar.Source = ImageSource.FromUri(new Uri(_userProfile.AvatarUrl));
        }
    }
}