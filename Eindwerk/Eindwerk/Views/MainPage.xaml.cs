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
        private AuthenticationService _authenticationService;
        private UserService _userService;
        private UserProfile _userProfile;

        public MainPage(Tokens tokens)
        {
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

        private void OnStartTravelClicked(object sender, EventArgs e)
        {
            
        }

        private void LogoutTapped(object sender, EventArgs e)
        {
            _authenticationService.Logout();
            Navigation.PopToRootAsync();
        }

        #endregion
    }
}