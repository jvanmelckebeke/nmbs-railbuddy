using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Exceptions;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Credentials = Eindwerk.Models.Forms.Credentials;

namespace Eindwerk.Views.Authentication
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : NetworkDependentPage
    {
        private readonly AuthenticationService _authenticationService;

        public LoginPage()
        {
            InitializeComponent();

            _authenticationService = new AuthenticationService();

            BtnLogin.Clicked += OnBtnLoginClicked;
            BtnRegistration.Clicked += OnBtnRegistrationClicked;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            HandleApi(CheckRefreshToken);
        }

        private async Task DoLogin(Credentials credentials)
        {
            try
            {
                Tokens tokens = await _authenticationService.LoginAsync(credentials);
                GoToMainPage(tokens);
            }
            catch (WrongCredentialsException)
            {
                UserDialogs.Instance.Alert("Sorry, email or password is wrong");

                LblEmail.TextColor = Color.PaleVioletRed;
                LblPassword.TextColor = Color.PaleVioletRed;
            }
        }


        private async Task CheckRefreshToken()
        {
            string refreshToken = Preferences.Get("refreshToken", null);

            if (refreshToken == null) return;

            Debug.WriteLine("a refresh token was present");


            Tokens tokens = await _authenticationService.TryRefreshTokensAsync();

            Debug.WriteLine($"tokens: {tokens}");

            if (tokens != null)
            {
                GoToMainPage(tokens);
                return;
            }

            Debug.WriteLine("refresh token expired");
        }

        #region event handlers

        private async void OnBtnLoginClicked(object sender, EventArgs e)
        {
            var credentials = new Credentials
            {
                Email = EntEmail.Text,
                Password = EntPassword.Text
            };

            if (credentials.ValidateInputs())
            {
                async Task DoLoginCall()
                {
                    await DoLogin(credentials);
                }

                await HandleApi(DoLoginCall, "logging in");
            }
            else
            {
                UserDialogs.Instance.Toast("Email is invalid, check that your mail is written as 'example@domain.com'");
            }
        }

        private void OnBtnRegistrationClicked(object sender, EventArgs e)
        {
            GoToRegistrationPage();
        }

        #endregion


        #region navigation

        private void GoToMainPage(Tokens tokens)
        {
            Navigation.PushAsync(new MainPage(tokens), true);
        }

        private void GoToRegistrationPage()
        {
            Navigation.PushAsync(new RegistrationPage(), true);
        }

        #endregion
    }
}