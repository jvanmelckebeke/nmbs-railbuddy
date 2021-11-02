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

        public MainPage(Tokens tokens)
        {
            InitializeComponent();
            _tokens = tokens;
            _authenticationService = new AuthenticationService(tokens.AccessToken);
            SetupProfile();
        }

        private async Task SetupProfile()
        {
            _userService = new UserService(_tokens.AccessToken);
            _userProfile = await _userService.GetUserProfile(_authenticationService.GetOwnProfileId());
            ShowProfile();
        }

        private void ShowProfile()
        {
            LblUser.Text = _userProfile.Username;
            ImgAvatar.Source = ImageSource.FromUri(new Uri(_userProfile.AvatarUrl));
        }
    }
}