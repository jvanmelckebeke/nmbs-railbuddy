using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Eindwerk.Models;
using Eindwerk.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Credentials = Eindwerk.Models.Forms.Credentials;

namespace Eindwerk.Views.Authentication
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private AuthenticationService _authenticationService;

        public LoginPage()
        {
            InitializeComponent();
            _authenticationService = new AuthenticationService();

            BtnLogin.Clicked += BtnLoginClicked;
        }

        private async void BtnLoginClicked(object sender, EventArgs e)
        {
            Credentials credentials = new Credentials()
            {
                Email = EntEmail.Text,
                Password = EntPassword.Text
            };

            if (credentials.ValidateInputs())
            {
                Tokens tokens = await _authenticationService.LoginAsync(credentials);

                if (tokens != null)
                {
                    GoToMainPage(tokens);
                }
                else
                {
                    var toastConfig = new ToastConfig("toasting...");
                    toastConfig.SetDuration(3000);

                    UserDialogs.Instance.Toast(toastConfig);
                }
            }
            else
            {
                UserDialogs.Instance.Toast("input invalid");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckAuthentication();
        }

        private async Task CheckAuthentication()
        {
            var refreshToken = Preferences.Get("refreshToken", null);

            if (refreshToken == null)
            {
                return;
            }

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


        private void GoToMainPage(Tokens tokens)
        {
            Navigation.PushAsync(new MainPage(tokens));
        }
    }
}