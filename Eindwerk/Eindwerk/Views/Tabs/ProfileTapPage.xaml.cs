using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerk.Views.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileTapPage : NetworkDependentPage
    {
        private Tokens _tokens;
        private AuthenticationService _authenticationService;
        private UserService _userService;

        private UserProfile _profile;

        public ProfileTapPage(Tokens tokens)
        {
            InitializeComponent();
            _tokens = tokens;

            _authenticationService = new AuthenticationService(_tokens);
            _userService = _authenticationService.CreateWithTokens<UserService>();

            SetupProfile();
            SetupListeners();
        }

        private async void SetupProfile()
        {
            await HandleNetworkAsync(async () =>
                _profile = await _userService.GetUserProfileAsync());
            SetupVisual();
        }

        private void SetupVisual()
        {
            LblUser.Text = $"Hi, {_profile.Username}!";
            ImgAvatar.Source = ImageSource.FromUri(new Uri(_profile.AvatarUrl));
        }

        private void SetupListeners()
        {
            BtnLogout.Clicked += OnLogoutClicked;
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            _authenticationService.Logout();
            Navigation.PopToRootAsync();
        }
    }
}