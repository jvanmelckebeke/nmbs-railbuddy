using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Exceptions;
using Eindwerk.Models;
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
            CheckRefreshToken();
        }

        #region event handlers

        private async void OnBtnLoginClicked(object sender, EventArgs e)
        {
            IProgressDialog dialog = UserDialogs.Instance.Loading("logging in");
            Credentials credentials = new Credentials()
            {
                Email = EntEmail.Text,
                Password = EntPassword.Text
            };

            if (credentials.ValidateInputs())
            {
                await DoLogin(credentials);
            }
            else
            {
                UserDialogs.Instance.Toast("Email is invalid, check that your mail is written as 'example@domain.com'");
            }

            dialog.Hide();
        }

        private void OnBtnRegistrationClicked(object sender, EventArgs e)
        {
            GoToRegistrationPage();
        }

        #endregion
        private async Task DoLogin(Credentials credentials)
        {
            await HandleNetworkAsync(async () =>
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
            });
        }

       

        private async void CheckRefreshToken()
        {
            var refreshToken = Preferences.Get("refreshToken", null);

            if (refreshToken == null) return;

            Debug.WriteLine("a refresh token was present");


            await HandleNetworkAsync(async () =>
            {
                Tokens tokens = await _authenticationService.TryRefreshTokensAsync();

                Debug.WriteLine($"tokens: {tokens}");

                if (tokens != null)
                {
                    GoToMainPage(tokens);
                    return;
                }

                Debug.WriteLine("refresh token expired");
            });
        }


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