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
    public partial class LoginPage : EncapsulatedPage
    {
        private AuthenticationService _authenticationService;

        public LoginPage()
        {
            InitializeComponent();
            _authenticationService = new AuthenticationService();

            BtnLogin.Clicked += BtnLoginClicked;
            BtnRegistration.Clicked += BtnRegistrationClicked;
        }

        private void BtnRegistrationClicked(object sender, EventArgs e)
        {
            GoToRegistrationPage();
        }

        private async void BtnLoginClicked(object sender, EventArgs e)
        {
            IProgressDialog dialog = UserDialogs.Instance.Loading("logging in");
            Credentials credentials = new Credentials()
            {
                Email = EntEmail.Text,
                Password = EntPassword.Text
            };

            if (credentials.ValidateInputs())
            {
                try
                {
                    await EncapsulateExceptionsAsync(async () =>
                    {
                        Tokens tokens = await _authenticationService.LoginAsync(credentials);

                        GoToMainPage(tokens);
                    });
                }
                catch (WrongCredentialsException)
                {
                    UserDialogs.Instance.Alert("Sorry, email or password is wrong");

                    LblEmail.TextColor = Color.PaleVioletRed;
                    LblPassword.TextColor = Color.PaleVioletRed;
                }
            }
            else
            {
                UserDialogs.Instance.Toast("Email is invalid, check that your mail is written as 'example@domain.com'");
            }

            dialog.Hide();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckAuthentication();
        }

        private async void CheckAuthentication()
        {
            var refreshToken = Preferences.Get("refreshToken", null);

            if (refreshToken == null)
            {
                return;
            }

            Debug.WriteLine("a refresh token was present");

            Tokens tokens = await EncapsulateExceptions(() => _authenticationService.TryRefreshTokensAsync());

            Debug.WriteLine($"tokens: {tokens}");
            if (tokens != null)
            {
                GoToMainPage(tokens);
                return;
            }

            Debug.WriteLine("refresh token expired");
        }


        private void GoToMainPage(Tokens tokens)
        {
            Navigation.PushAsync(new MainPage(tokens), true);
        }

        private void GoToRegistrationPage()
        {
            Navigation.PushAsync(new RegistrationPage(), true);
        }
    }
}